#include "MemTracer/MemTracer.h"

#include <cstdio>
#include <new>
#include <stack>
#include <string>
#include <vector>

#include <conio.h>			// kbhit
#include <process.h>		// beginthreadex
#include <time.h>
#include <windows.h>		// Sleep


// NOTE: This application is _intentionally_ very ineffective when
// it comes to dealing with memory.

void* s_allocated[1000];
LONG s_numAlloc = 0;

CRITICAL_SECTION csAllocator;

void* MyAlloc(size_t bytes)
{
	EnterCriticalSection(&csAllocator);

	void* ptr = malloc(bytes);
	MemTracer::OnAlloc(ptr, bytes, "SYSM");
	printf("Allocating %p\n", ptr);
	s_allocated[s_numAlloc] = ptr;
	InterlockedIncrement(&s_numAlloc);

	LeaveCriticalSection(&csAllocator);
	return ptr;
}

void MyFree(void* ptr)
{
	EnterCriticalSection(&csAllocator);
	if (ptr)
	{
		printf("Freeing %p\n", ptr);
		for (int i = 0; i < s_numAlloc; ++i)
		{
			if (s_allocated[i] == ptr)
			{
				//s_allocated[i] = 0;
				break;
			}
		}
		MemTracer::OnFree(ptr);
	}

	free(ptr);
	LeaveCriticalSection(&csAllocator);
}

void* __cdecl operator new(size_t bytes) 
{
	return MyAlloc(bytes);
}
void* __cdecl operator new[](size_t bytes) 
{
	return MyAlloc(bytes);
}
void* __cdecl operator new(size_t bytes, const std::nothrow_t&) throw() 
{
	return MyAlloc(bytes);
} 
void* __cdecl operator new[](size_t bytes, const std::nothrow_t&) throw() 
{
	return MyAlloc(bytes);
} 

void __cdecl operator delete(void* ptr) throw()
{
	MyFree(ptr);
}
void __cdecl operator delete[](void* ptr) throw()
{
	MyFree(ptr);
}

struct ResourceData
{
	ResourceData()
	:	m_buffer(0),
		m_bufferSize(0)
	{
	}
	ResourceData(const ResourceData& rhs)
	:	m_buffer(0),
		m_bufferSize(0)
	{
		*this = rhs;			
	}
	~ResourceData()
	{
		Free();
	}

	ResourceData& operator=(const ResourceData& rhs)
	{
		if (this != &rhs)
		{
			Free();
			if (rhs.m_bufferSize)
			{
				m_buffer = new char[rhs.m_bufferSize];
				memcpy(m_buffer, rhs.m_buffer, rhs.m_bufferSize);
				m_bufferSize = rhs.m_bufferSize;
			}
		}
		return *this;
	}

	bool Load(FILE* f)
	{
		RDE_ASSERT(m_buffer == 0);

		fseek(f, 0, SEEK_END);
		const long fileSize = ftell(f);
		fseek(f, 0, SEEK_SET);

		if (fileSize > 0)
		{
			m_buffer = new char[fileSize];
			if (m_buffer)
			{
				const size_t bytesRead = fread(m_buffer, 1, fileSize, f);
				RDE_ASSERT(bytesRead == (size_t)fileSize);
				m_bufferSize = (size_t)fileSize;
			}
		}
		return m_buffer != 0;
	}

private:
	void Free()
	{
		delete[] m_buffer;
		m_buffer = 0;
		m_bufferSize = 0;
	}

	char*	m_buffer;
	size_t	m_bufferSize;
};

struct ResourceStack
{
	typedef std::string ResourceTag;

	ResourceStack()
	{
		InitializeCriticalSection(&m_resourceTagMutex);
		InitializeCriticalSection(&m_resourcesMutex);
	}
	~ResourceStack()
	{
		DeleteCriticalSection(&m_resourceTagMutex);
		DeleteCriticalSection(&m_resourcesMutex);
	}

	ResourceTag PopResourceToLoad()
	{
		EnterCriticalSection(&m_resourceTagMutex);
		
		if (m_resourcesToLoad.empty())
		{
			LeaveCriticalSection(&m_resourceTagMutex);
			return ResourceTag();
		}

		ResourceTag retTag = m_resourcesToLoad.top();
		m_resourcesToLoad.pop();
		LeaveCriticalSection(&m_resourceTagMutex);
		return retTag;
	}
	void AddResourceToLoad(const ResourceTag& tag)
	{
		m_resourcesToLoad.push(tag);
	}
	void AddResourceData(const ResourceData& data)
	{
		EnterCriticalSection(&m_resourcesMutex);
		m_loadedResources.push_back(data);
		LeaveCriticalSection(&m_resourcesMutex);
	}

	typedef std::stack<ResourceTag>			Tags;
	typedef std::vector<ResourceData>		Resources;

	Tags				m_resourcesToLoad;
	CRITICAL_SECTION	m_resourceTagMutex;
	Resources			m_loadedResources;
	CRITICAL_SECTION	m_resourcesMutex;
};

struct ResourceLoader 
{
	ResourceLoader()
	:	m_hThread(0),
		m_done(false)
	{
	}
	~ResourceLoader()
	{
		if (m_hThread)
		{
			const DWORD rv = ::WaitForSingleObject(m_hThread, INFINITE);
			//RDE_ASSERT(rv == WAIT_OBJECT_0);
			if (rv == WAIT_OBJECT_0)
			{
				::CloseHandle(m_hThread);
				//RDE_ASSERT(ok);
				m_hThread = 0;
			}
		}
	}

	void Init(ResourceStack& stack)
	{
		m_stack = &stack;
	}
	void StartThread()
	{
		const unsigned int stackSize = 4096;
		m_hThread = reinterpret_cast<HANDLE>(
			::_beginthreadex(0, stackSize, &Thread_Run, this, 0, 0));
	}
	static unsigned __stdcall Thread_Run(LPVOID arg)
	{
		ResourceLoader* loader = static_cast<ResourceLoader*>(arg);
		loader->MainLoop();
		return 0;
	}

	void MainLoop()
	{
		int numLoaded(0);
		while (true)
		{
			const ResourceStack::ResourceTag tagToLoad = m_stack->PopResourceToLoad();
			if (!tagToLoad.empty())
			{
				LoadResource(tagToLoad);
				++numLoaded;
			}
			else
			{
				break;
			}
		}
		m_done = true;
	}
	void LoadResource(const ResourceStack::ResourceTag& tag)
	{
		MemTracer::PushTag(tag.c_str());

		ResourceStack::ResourceTag fullName("c:\\Windows\\System32\\");
		fullName.append(tag);
		FILE* f = fopen(fullName.c_str(), "rb");
		if (f)
		{
			ResourceData data;
			if (data.Load(f))
			{
				m_stack->AddResourceData(data);
			}
			fclose(f);
		}

		MemTracer::PopTag();
	}
	bool IsDone() const { return m_done; }

	ResourceStack*	m_stack;
	HANDLE			m_hThread;
	volatile bool	m_done;
};

static const int kNumLoaders = 9;
void LoadLevel(ResourceStack& resStack)
{
	const int maxResourcesToLoad = 8;

	WIN32_FIND_DATA ffd;
	HANDLE hFind = FindFirstFile("c:\\Windows\\System32\\*.*", &ffd);
	if (hFind)
	{
		do
		{
			if (!(ffd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
			{
				ResourceStack::ResourceTag tag(ffd.cFileName);
				resStack.AddResourceToLoad(tag);
			}
			if (resStack.m_resourcesToLoad.size() >= maxResourcesToLoad)
			{
				break;
			}
		} while (FindNextFile(hFind, &ffd) != 0);
	}
	
	ResourceLoader loaders[kNumLoaders];
	for (int i = 1; i < kNumLoaders; ++i)
	{
		loaders[i].Init(resStack);
		loaders[i].StartThread();
	}
	loaders[0].Init(resStack);
	loaders[0].MainLoop();

	volatile bool allDone(false);
	while (!allDone)
	{
		int numDone(0);
		for (int i = 1; i < kNumLoaders; ++i)
		{
			if (loaders[i].IsDone())
			{
				++numDone;
			}
		}
		allDone = (numDone == kNumLoaders - 1);
	}
}

typedef std::vector<void*> MemBlocks;
void Frame(MemBlocks& memBlocks)
{
	static size_t s_frameNr = 0;

	// Every few frames allocate a new block
	if (((++s_frameNr) & 0x7F) == 0)
	{
		const size_t toAlloc = 1000 + (rand() % 10000);
		memBlocks.push_back(operator new(toAlloc));
		MemTracer::SetTracedVar("TestVar", (int)s_frameNr);
	}
	// Every few frames, free randomly chosen block
	if ((s_frameNr & 0xFF) == 0)
	{
		const size_t indexToFree = rand() % memBlocks.size();
		MemBlocks::iterator it = memBlocks.begin();
		std::advance(it, indexToFree);
		void* ptr = *it;
		operator delete(ptr);
		memBlocks.erase(it);
	}

	Sleep(20);
	MemTracer::FrameEnd();
}


int __cdecl main(int, char const *[])
{
	InitializeCriticalSection(&csAllocator);

	printf("MemTracer: waiting for connection...\n");
	MemTracer::FunctionHooks hooks;
	const unsigned short port = 1000;
	if (!MemTracer::Init(hooks, port, kNumLoaders + 1, MemTracer::BlockingMode::BLOCKING))
	{
		printf("Failed to initialize MemTracer\n");
	}
	printf("MemTracer: Connected, memory overhead ~= %d kb\n", MemTracer::GetMemoryOverhead() >> 10);

	srand((unsigned int)time(NULL));

	{
		ResourceStack resStack;
		LoadLevel(resStack);
		MemTracer::AddSnapshot("Level loaded");

		MemBlocks memBlocks;
		while (!_kbhit())
		{
			Frame(memBlocks);
		}
		MemTracer::AddSnapshot("Exiting");
		printf("*************** Exiting\n");
		for (MemBlocks::iterator it = memBlocks.begin(); it != memBlocks.end(); ++it)
			delete *it;
	}
	MemTracer::AddSnapshot("Done");
	MemTracer::FrameEnd();

	for (int i = 0; i < s_numAlloc; ++i)
	{
		if (s_allocated[i] != 0)
		{
			printf("Not free: %p\n", s_allocated[i]);
		}
	}


	printf("Done, number of threads generating data: %d\n", MemTracer::GetNumTrackedThreads());
	MemTracer::Shutdown();

	DeleteCriticalSection(&csAllocator);
	return 0;
}
