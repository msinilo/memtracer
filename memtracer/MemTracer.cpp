#include "MemTracer.h"

#if RDE_MEMTRACER_ENABLED

#include "RdeWrapper.h"
#include "DefaultFunctions.h"
#include "SocketWrapper.h"
#if !RDE_MEMTRACER_STANDALONE
#	include "core/Atomic.h"
#	include "core/Thread.h"
#	include "thread/SPSCQueue.h"
#else
#	include "SPSCQueue.h"
#endif

#include <cstring>	// strncpy

// Setting this to true makes sure that messages sent in one batch
// are ordered same as they were generated (when they come from different threads).
// It has a slight memory/performance overhead.
#define RDE_MEMTRACER_SEQUENTIAL	1

#if RDE_MEMTRACER_SEQUENTIAL
#include <cstdlib>
#endif

namespace
{
using MemTracer::Address;
using rde::uint32;
using rde::uint8;

const size_t	kMaxPacketSize			= 256;
const size_t	kMaxRuntimePacketSize	= 128;
const size_t	kMaxCallStackDepth		= 20;
const size_t	kMaxTagLen				= 64;
const size_t	kMaxSnapshotNameLen		= 32;
const size_t	kMaxTagStackDepth		= 4;
const size_t	kSocketBufferSize		= 16384;
const size_t	kCallStackEntriesToSkip	= 1;
const size_t	kMaxTracedVarNameLen	= 16;
const size_t	kGlobalQueueSize		= 8192;

typedef char MemTag[kMaxTagLen];
typedef char VarName[kMaxTracedVarNameLen];

inline uint32 ByteSwap32(uint32 i)
{
	return (0xFF & i) << 24 | (0xFF00 & i) << 8 | (0xFF0000 & i) >> 8 | (0xFF000000 & i) >> 24;
}

template<size_t N> struct PtrSizeHelper {};
template<> struct PtrSizeHelper<4>
{
	static uintptr_t ByteSwapAddress(uintptr_t a)
	{
		return ByteSwap32((uint32)a);
	}
};
inline Address ByteSwapAddress(Address a)
{
	return (Address)PtrSizeHelper<sizeof(Address)>::ByteSwapAddress((uintptr_t)a);
}

// We assume that receiving application is little endian (PC).

#if RDE_LITTLE_ENDIAN
	inline uint32 ByteSwapToNet32(uint32 i) { return i; }
	inline Address ByteSwapAddressToNet(Address a) { return a; }
#else
	inline uint32 ByteSwapToNet32(uint32 i) { return ByteSwap32(i); }
	inline Address ByteSwapAddressToNet(Address a) { return ByteSwapAddress(a); }
#endif

#pragma pack(push, 1)
struct InitialSettingsPacket
{
	InitialSettingsPacket() 
	:	messageSize(0),
		command(MemTracer::CommandId::INITIAL_SETTINGS),
		platform(MemTracer::Platform::WINDOWS),
		maxTagLen(kMaxTagLen),
		maxSnapshotNameLen(kMaxSnapshotNameLen),
		maxTracedVarNameLen(kMaxTracedVarNameLen)
	{}

	size_t PrepareToSend()
	{
		messageSize = sizeof(InitialSettingsPacket) - 1;
		return messageSize + 1;
	}

	uint8	messageSize;
	uint8	command;
	uint8	platform;
	uint8	maxTagLen;
	uint8	maxSnapshotNameLen;
	uint8	maxTracedVarNameLen;
};

struct ModuleInfoPacket
{	
	ModuleInfoPacket()
	:	messageSize(0),
		command(MemTracer::CommandId::MODULE_INFO) 
	{}

	size_t PrepareToSend()
	{
		info.moduleBase = ByteSwapToNet32(info.moduleBase);
		info.moduleSize = ByteSwapToNet32(info.moduleSize);
		messageSize = sizeof(ModuleInfoPacket) - 1;
		return messageSize + 1;
	}

	uint8					messageSize;
	uint8					command;
	MemTracer::ModuleInfo	info;
};
RDE_COMPILE_CHECK(sizeof(ModuleInfoPacket) == 2 + sizeof(MemTracer::ModuleInfo));
RDE_COMPILE_CHECK(sizeof(ModuleInfoPacket) < kMaxPacketSize);

struct AllocInfo
{
	void ReverseCallStack()
	{
		const uint32 half = depth >> 1;
		for (uint32 i = 0; i < half; ++i)
		{
			const Address temp = callStack[i];
			callStack[i] = callStack[depth - i - 1];
			callStack[depth - i - 1] = temp;
		}
	}

	void SetTag(const char* stag)
	{
		RDE_ASSERT(strlen(stag) >= 4);
		tag = (stag[0] << 24) | (stag[1] << 16) | (stag[2] << 8) | stag[3];
		tag = ByteSwapToNet32(tag);
	}
	
	Address	address;
	uint32	bytes;
	uint32	tag;
	uint8	depth;
	Address	callStack[kMaxCallStackDepth];
};
RDE_COMPILE_CHECK(kMaxCallStackDepth < 256);
RDE_COMPILE_CHECK(sizeof(AllocInfo) == 13 + sizeof(Address) * kMaxCallStackDepth);
RDE_COMPILE_CHECK(sizeof(AllocInfo) < kMaxPacketSize);
RDE_COMPILE_CHECK(sizeof(AllocInfo) < kMaxRuntimePacketSize);

struct FreeInfo
{
	Address	address;
};
RDE_COMPILE_CHECK(sizeof(FreeInfo) < kMaxPacketSize);

struct TagInfo
{
	Address	address;
	MemTag	tag;
};
RDE_COMPILE_CHECK(sizeof(TagInfo) < kMaxPacketSize);

struct SnapshotName
{
	char	name[kMaxSnapshotNameLen];
};
RDE_COMPILE_CHECK(sizeof(SnapshotName) < kMaxPacketSize);

struct TracedVar
{
	void Set(const char* varName, int varValue)
	{
		strncpy(name, varName, kMaxTracedVarNameLen - 1);
		value = varValue;
	}

	int		value;
	VarName	name;
};
RDE_COMPILE_CHECK(sizeof(TracedVar) < kMaxPacketSize);

#if RDE_MEMTRACER_SEQUENTIAL
struct Seq
{
	Seq() : m_seq(0) {}
	rde::Atomic32 Next()
	{
		return rde::AtomicInc(m_seq) - 1;
	}
	volatile rde::Atomic32 m_seq;
};
#else
struct Seq
{
	rde::Atomic32 Next() { return 0; }
};
#endif // RDE_MEMTRACER_SEQUENTIAL
Seq s_seq;

struct Packet
{
	explicit Packet(uint8 commandId = 0xFF, long seq_ = 0)
	:	messageSize(0),
		command(commandId)
#if RDE_MEMTRACER_SEQUENTIAL
		, seq(seq_)
	{
	}
#else
	{
		(void)sizeof(seq_);
	}
#endif


	uint32 CalcPacketSize() const
	{
		uint32 packetSize(0);
		if (command == MemTracer::CommandId::ALLOC)
		{
			packetSize = offsetof(AllocInfo, callStack) + sizeof(Address) * data.alloc.depth;
		}
		else if (command == MemTracer::CommandId::FREE)
		{
			packetSize = sizeof(FreeInfo);
		}
		else if (command == MemTracer::CommandId::TAG)
		{
			packetSize = sizeof(TagInfo);			
		}
		else if (command == MemTracer::CommandId::ADD_SNAPSHOT)
		{
			packetSize = sizeof(SnapshotName);
		}
		else if (command == MemTracer::CommandId::FRAME_END)
		{
			packetSize = 0;
		}
		else if (command == MemTracer::CommandId::TRACED_VAR)
		{
			packetSize = sizeof(TracedVar);
		}
		else
		{
			RDE_ASSERT(!"Unknown command");
		}
		packetSize += sizeof(command);
		return packetSize;
	}
	size_t PrepareToSend()
	{
		const uint32 packetSize = CalcPacketSize();
		RDE_ASSERT(packetSize < kMaxPacketSize);
		messageSize = static_cast<uint8>(packetSize);
		return messageSize + 1;
	}
#if RDE_MEMTRACER_SEQUENTIAL
	bool operator<(const Packet& rhs) const
	{
		return seq < rhs.seq;
	}
#endif

	uint8			messageSize;
	uint8			command;
	union
	{
		AllocInfo		alloc;
		FreeInfo		free;
		TagInfo			tag;
		SnapshotName	snapshotName;
		TracedVar		tracedVar;
	} data;
	// IMPORTANT: has to be the last member variable, it's not being sent
	// to tracer application.
#if RDE_MEMTRACER_SEQUENTIAL
	rde::Atomic32	seq;
#endif
};
RDE_COMPILE_CHECK(sizeof(Packet) < kMaxPacketSize);
RDE_COMPILE_CHECK(sizeof(Packet) < kMaxRuntimePacketSize);
#if RDE_MEMTRACER_SEQUENTIAL
RDE_COMPILE_CHECK(sizeof(Packet) == sizeof(AllocInfo) + 2 + 4);
RDE_COMPILE_CHECK(offsetof(Packet, seq) == sizeof(Packet) - 4);	// seq must be the last member
int PacketCompare(const void* lhs, const void* rhs)
{
	const Packet& pa = *(const Packet*)lhs;
	const Packet& pb = *(const Packet*)rhs;
	if (pa.seq < pb.seq)
		return -1;
	if (pa.seq > pb.seq)
		return 1;
	return 0;
}
#else
RDE_COMPILE_CHECK(sizeof(Packet) == sizeof(AllocInfo) + 2);
#endif

#pragma pack(pop)

#if RDE_MEMTRACER_SEQUENTIAL
class PacketCollection
{
public:
	PacketCollection()
	:	m_packets(0),
		m_packetsEnd(0),
		m_packetsCapacityEnd(0)
	{
	}
	~PacketCollection()
	{
		Close();
	}

	void Init(size_t capacity)
	{
		RDE_ASSERT(m_packets == 0);
		m_packets = new Packet[capacity];
		m_packetsEnd = m_packets;
		m_packetsCapacityEnd = m_packets + capacity;
	}
	void Close()
	{
		if (m_packets)
		{
			delete[] m_packets;
		}
		m_packets = 0;
	}
	
	Packet* Begin()	{ return m_packets; }
	Packet* End()	{ return m_packetsEnd; }
	size_t GetMemoryOverhead() const
	{
		return (m_packetsCapacityEnd - m_packets) * sizeof(Packet);
	}

	bool Add(const Packet& packet)
	{
		if (m_packetsEnd == m_packetsCapacityEnd)
			return false;

		*m_packetsEnd++ = packet;
		return true;
	}

	void Reset()
	{
		m_packetsEnd = m_packets;
	}
	void Sort()
	{
		qsort(m_packets, m_packetsEnd - m_packets, sizeof(m_packets[0]), PacketCompare);
	}

private:
	Packet*	m_packets;
	Packet*	m_packetsEnd;
	Packet*	m_packetsCapacityEnd;
};
#else
struct PacketCollection
{
	void Init(size_t) {}
	void Close() {}
	size_t GetMemoryOverhead() const { return 0; }
};
#endif // #if RDE_MEMTRACER_SEQUENTIAL

class PacketBuffer
{
public:
	PacketBuffer()
	:	m_free(true)
	{
		// Queue size must be power of two.
		RDE_COMPILE_CHECK((MAX_PACKETS_IN_BUFFER & (MAX_PACKETS_IN_BUFFER - 1)) == 0);
	}

	void AddPacket(const Packet& packet)
	{
		// Spin loop until there is a place for new packet.
		// If this happens to often, it means we are producing packets
		// quicker than consuming them, increasing max # of packets in buffer 
		// could help.
		while (m_packetQueue.IsFull())
		{
			rde::Thread::Sleep(1);
		}
		m_packetQueue.Push(packet);
	}
#if RDE_MEMTRACER_SEQUENTIAL
	bool CollectPackets(PacketCollection& coll)
	{
		Packet* packet = m_packetQueue.Peek();
		while (packet)
		{
			if (coll.Add(*packet))
			{
				m_packetQueue.Pop();
				packet = m_packetQueue.Peek();
			}
			else
			{
				return false;
			}
		}
		return true;
	}
#endif
	void SendPackets(MemTracer::Socket::Handle& h)
	{
		Packet* packet = m_packetQueue.Peek();
		while (packet)
		{
			const size_t bytesToSend = packet->PrepareToSend();
			size_t bytesWritten(0);
			const bool success = MemTracer::Socket::Write(h, packet, bytesToSend, bytesWritten); 
	
			// Failed to send data. Most probably sending too much, break and
			// hope for the best next time
			if (!success || bytesWritten != bytesToSend)
			{
				break;
			}
			m_packetQueue.Pop();	// 'Commit' pop if data sent.
			packet = m_packetQueue.Peek();
		}
	}

	bool	m_free;

private:
	enum { MAX_PACKETS_IN_BUFFER	= 8192 };

	rde::SPSCQueue<Packet, MAX_PACKETS_IN_BUFFER>	m_packetQueue;
};

RDE_THREADLOCAL int	t_packetBufferIndex = -1;

struct MemTracerImpl
{
	MemTracerImpl()
	:	m_port(0),
		m_writeSocket(0),
		m_packetBuffersLock(0),
		m_packetBuffers(0),
		m_maxTracedThreads(0),
		m_isConnected(false),
		m_terminating(false)
	{
	}

	void Init(unsigned short port, int maxTracedThreads, const MemTracer::FunctionHooks& hooks)
	{
		m_port = port;		
		m_hooks = hooks;
		m_packetBuffers = new PacketBuffer[maxTracedThreads];
		m_maxTracedThreads = maxTracedThreads;

		m_packetCollection.Init(kGlobalQueueSize);

		m_packetBuffersLock = m_hooks.m_pfnMutexCreate();
		RDE_ASSERT(m_packetBuffersLock);
	}
	void Close(MemTracer::ThreadHandle hThread)
	{
		m_terminating = true;
		while (IsConnected())
		{
			rde::Thread::Sleep(1);
		}
		m_hooks.m_pfnMutexAcquire(m_packetBuffersLock);
		delete[] m_packetBuffers;
		m_packetBuffers = 0;
		m_hooks.m_pfnMutexRelease(m_packetBuffersLock);
		m_hooks.m_pfnMutexDestroy(m_packetBuffersLock);
		m_packetBuffersLock = 0;
		m_hooks.m_pfnThreadJoin(hThread);
		m_packetCollection.Close();
	}

	bool IsConnected() const	{ return m_isConnected; }

	void AddPacket(const Packet& packet)
	{
		if (t_packetBufferIndex == -1)
		{
			t_packetBufferIndex = ReserveThreadPacketBuffer();
			if (t_packetBufferIndex < 0)
			{
				Log("Couldn't reserve packet buffer, too many active threads.\n");
				RDE_ASSERT(false);
				return;
			}
		}
		if (t_packetBufferIndex >= 0)
		{
			m_packetBuffers[t_packetBufferIndex].AddPacket(packet);
		}
	}

	void SendInitialSettings() 
	{
		InitialSettingsPacket initialPacket;
		const size_t bytesToSend = initialPacket.PrepareToSend();
		size_t bytesWritten(0);
		if (!MemTracer::Socket::Write(m_writeSocket, &initialPacket, bytesToSend, bytesWritten) ||
			bytesWritten != bytesToSend)
		{
			Log("MEMTRACER WARNING: Couldn't send initial settings.\n");
		}
	}

	void OnConnection()
	{
		SendInitialSettings();

		const int numModules = m_hooks.m_pfnModuleInfoGetNumLoadedModules();
		for (int i = 0; i < numModules; ++i)
		{
			ModuleInfoPacket modInfo;
			m_hooks.m_pfnModuleInfoGet(i, modInfo.info);
			const size_t bytesToSend = modInfo.PrepareToSend();
			size_t bytesWritten(0);
			if (!MemTracer::Socket::Write(m_writeSocket, &modInfo, bytesToSend, bytesWritten) ||
				bytesWritten != bytesToSend)
			{
				Log("MEMTRACER WARNING: Couldn't send module information.\n");
			}
		}
	}

	void SendAllPackets()
	{
		for (int i = 0; i < m_maxTracedThreads; ++i)
		{
			if (!m_packetBuffers[i].m_free)
			{
#if RDE_MEMTRACER_SEQUENTIAL
				if (!m_packetBuffers[i].CollectPackets(m_packetCollection))
				{
					break;
				}
#else
				m_packetBuffers[i].SendPackets(m_writeSocket);
#endif
			}
		}
#if RDE_MEMTRACER_SEQUENTIAL
		// TODO: Deal with Socket::Write failures.
		// (right now packet is lost).
		m_packetCollection.Sort();
		for (Packet* p = m_packetCollection.Begin(); p != m_packetCollection.End(); ++p)
		{
			const size_t bytesToSend = p->PrepareToSend();
			size_t bytesWritten(0);
			MemTracer::Socket::Write(m_writeSocket, p, bytesToSend, bytesWritten); 
		}
		m_packetCollection.Reset();
#endif
	}

	int ReserveThreadPacketBuffer()
	{
		int retIndex(-2);
		m_hooks.m_pfnMutexAcquire(m_packetBuffersLock);
		// NOTE: This is quite naive attempt to make sure that main thread queue
		// is the last one (rely on the fact that it's most likely to be the first
		// one trying to send message). This means EndFrame event should be sent after
		// memory operations from that frame.
		for (int i = m_maxTracedThreads - 1; i >= 0; --i)
		{
			if (m_packetBuffers[i].m_free)
			{
				m_packetBuffers[i].m_free = false;
				retIndex = i;
				break;
			}
		}
		m_hooks.m_pfnMutexRelease(m_packetBuffersLock);
		return retIndex;
	}

	void ThreadFunc()
	{
		Log("MemTracer: Starting\n");

		namespace Socket = MemTracer::Socket;
		const bool blockingSocket = true;
		Socket::Handle serverSocket = Socket::Create(blockingSocket);
		if (!serverSocket)
		{
			Log("MemTracer: Couldn't create server socket.\n");
			return;
		}

		if (!Socket::Listen(serverSocket, m_port))
		{
			Log("MemTracer: Couldn't configure server socket.\n");
			Socket::Close(serverSocket);
			return;
		}

		Log("MemTracer: Waiting for connection.\n");
		while (!m_terminating)
		{
			if (Socket::TestConnection(serverSocket))
				break;

			rde::Thread::Sleep(100);
		}
		Log("MemTracer: Connected.\n");
		m_writeSocket = Socket::Accept(serverSocket, kSocketBufferSize);
		if (!m_writeSocket)
		{
			Log("MemTracer: Couldn't create write socket.\n");
			Socket::Close(serverSocket);
			return;
		}

		OnConnection();

		MemoryWriteBarrier();
		m_isConnected = true;
		while (!m_terminating && m_writeSocket)
		{
			rde::Thread::Sleep(1);
			SendAllPackets();
		}
		// One last time, to send any outstanding pockets out there.
		SendAllPackets();

		Socket::Close(m_writeSocket);
		Socket::Close(serverSocket);
		m_isConnected = false;
	}

	int GetCallStack(Address* callStack, int maxDepth, int entriesToSkip)
	{
		return m_hooks.m_pfnCallStackGet(callStack, maxDepth, entriesToSkip);
	}

	size_t GetMemoryOverhead() const
	{
		return sizeof(PacketBuffer) * m_maxTracedThreads + m_packetCollection.GetMemoryOverhead();
	}

	size_t GetNumTrackedThreads() const
	{
		size_t numTrackedThreads(0);
		for (int i = 0; i < m_maxTracedThreads; ++i)
		{
			if (!m_packetBuffers[i].m_free)
			{
				++numTrackedThreads;
			}
		}
		return numTrackedThreads;
	}

private:
	void Log(const char* msg) const
	{
		m_hooks.m_pfnLog(msg);
	}

	unsigned short				m_port;
	MemTracer::Socket::Handle	m_writeSocket;
	MemTracer::MutexHandle		m_packetBuffersLock;
	MemTracer::FunctionHooks	m_hooks;
	PacketBuffer*				m_packetBuffers;
	PacketCollection			m_packetCollection;
	int							m_maxTracedThreads;
	volatile bool				m_isConnected;
	volatile bool				m_terminating;
};

MemTracerImpl			s_tracer;
MemTracer::ThreadHandle	s_tracerThread;
RDE_THREADLOCAL	MemTag	t_tagStack[kMaxTagStackDepth];
RDE_THREADLOCAL size_t	t_tagStackTop(0);

void MemTracer_ThreadFunc(MemTracerImpl* tracer)
{
	tracer->ThreadFunc();
}

} // namespace

namespace MemTracer
{
FunctionHooks::FunctionHooks()
:	m_pfnThreadFork(&DefaultThreadFork),
	m_pfnThreadJoin(&DefaultThreadJoin),
	m_pfnMutexCreate(&DefaultMutexCreate),
	m_pfnMutexAcquire(&DefaultMutexAcquire),
	m_pfnMutexRelease(&DefaultMutexRelease),
	m_pfnMutexDestroy(&DefaultMutexDestroy),
	m_pfnCallStackGet(&DefaultCallStackGet),
	m_pfnModuleInfoGetNumLoadedModules(&DefaultModuleInfo_GetNumLoadedModules),
	m_pfnModuleInfoGet(&DefaultModuleInfo_Get),
	m_pfnLog(&DefaultLog)
{
}

bool Init(FunctionHooks& hooks, unsigned short port, int maxTracedThreads, BlockingMode::Enum mode)
{
	s_tracer.Init(port, maxTracedThreads, hooks);
	if (!Socket::InitSockets())
	{
		return false;
	}
	s_tracerThread = hooks.m_pfnThreadFork((ThreadFunction*)&MemTracer_ThreadFunc, &s_tracer, 16 * 1024);

	while (mode == BlockingMode::BLOCKING && !s_tracer.IsConnected())
	{
		// Wait for connection
	}
	return true;
}

void Shutdown()
{
	s_tracer.Close(s_tracerThread);
	Socket::ShutdownSockets();
}

void OnAlloc(const void* ptr, size_t bytes, const char* tag)
{
	if (s_tracer.IsConnected())
	{
		Packet packet(CommandId::ALLOC, s_seq.Next());
		AllocInfo& info = packet.data.alloc;

		const int numEntries = s_tracer.GetCallStack(&info.callStack[0], kMaxCallStackDepth, kCallStackEntriesToSkip);
		// Incomplete callstack, not much we can do unfortunately.
		// It happens sometimes in optimized build, if you're really curious where did the call
		// come from, you can try putting a breakpoint here.
		if (numEntries <= 0)
		{
			return;
		}

		info.address = ByteSwapAddressToNet(reinterpret_cast<Address>(ptr));
		info.bytes = ByteSwapToNet32(static_cast<uint32>(bytes));
		info.depth = static_cast<uint8>(numEntries);
		if (tag)
			info.SetTag(tag);
		info.ReverseCallStack();
		s_tracer.AddPacket(packet);

		if (t_tagStackTop > 0)
			TagBlock(ptr, t_tagStack[t_tagStackTop - 1]);				
	}
}

void OnFree(const void* ptr)
{
	if (s_tracer.IsConnected())
	{
		Packet packet(CommandId::FREE, s_seq.Next());
		FreeInfo& info = packet.data.free;
		info.address = ByteSwapAddressToNet(reinterpret_cast<Address>(ptr));
		s_tracer.AddPacket(packet);
	}
}

void TagBlock(const void* ptr, const char* tag)
{
	if (s_tracer.IsConnected())
	{
		Packet packet(CommandId::TAG, s_seq.Next());
		TagInfo& info = packet.data.tag;
		info.address = ByteSwapAddressToNet(reinterpret_cast<Address>(ptr));
		strncpy(info.tag, tag, kMaxTagLen - 1);
		s_tracer.AddPacket(packet);
	}
}

void PushTag(const char* tag)
{
	if (t_tagStackTop + 1 < kMaxTagStackDepth)
	{
		strncpy(t_tagStack[t_tagStackTop++], tag, kMaxTagLen - 1);
	}
}

void PopTag()
{
	RDE_ASSERT(t_tagStackTop > 0);
	if (t_tagStackTop > 0)
	{
		--t_tagStackTop;		
	}
}

void AddSnapshot(const char* snapshotName)
{
	if (snapshotName && s_tracer.IsConnected())
	{
		Packet packet(CommandId::ADD_SNAPSHOT, s_seq.Next());
		strncpy(packet.data.snapshotName.name, snapshotName, kMaxSnapshotNameLen - 1);
		s_tracer.AddPacket(packet);
	}
}

void FrameEnd()
{
	if (s_tracer.IsConnected())
	{
		Packet packet(CommandId::FRAME_END, s_seq.Next());
		s_tracer.AddPacket(packet);
	}
}

void SetTracedVar(const char* varName, int value)
{
	if (s_tracer.IsConnected())
	{
		Packet packet(CommandId::TRACED_VAR, s_seq.Next());
		packet.data.tracedVar.Set(varName, value);
		s_tracer.AddPacket(packet);
	}
}

size_t GetMemoryOverhead()
{
	return s_tracer.GetMemoryOverhead();
}

size_t GetNumTrackedThreads()
{
	return s_tracer.GetNumTrackedThreads();
}

} // MemTracer

#endif // ENABLED
