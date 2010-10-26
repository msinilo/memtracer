#ifndef MEMTRACER_H
#define MEMTRACER_H

#define RDE_MEMTRACER_ENABLED		1
#define RDE_MEMTRACER_STANDALONE	1

#include "MemTracer/RdeWrapper.h"
#include <cstddef>

namespace MemTracer
{

typedef const void*	Address;
typedef void*		ThreadHandle;
typedef void*		MutexHandle;

struct ModuleInfo
{
	enum { MAX_PATH	= 128 };		

	unsigned long	moduleBase;
	unsigned long	moduleSize;
	char			debugInfoFile[MAX_PATH];
};

typedef unsigned (STDCALL ThreadFunction)(void* arg);
typedef ThreadHandle (Thread_Fork)(ThreadFunction* function, void* arg, size_t stackSize);
typedef void (Thread_Join)(ThreadHandle);
typedef MutexHandle (Mutex_Create)();
typedef void (Mutex_Acquire)(MutexHandle);
typedef void (Mutex_Release)(MutexHandle);
typedef void (Mutex_Destroy)(MutexHandle);
typedef int (CallStack_Get)(Address* callStack, int maxDepth, int entriesToSkip);
typedef int (ModuleInfo_GetNumLoadedModules)();
typedef void (ModuleInfo_Get)(int i, ModuleInfo& outModuleInfo);
typedef void (Log_Function)(const char* msg);

struct FunctionHooks
{	
#if RDE_MEMTRACER_ENABLED
	FunctionHooks();
#else
	FunctionHooks() {}
#endif

	Thread_Fork*					m_pfnThreadFork;
	Thread_Join*					m_pfnThreadJoin;
	Mutex_Create*					m_pfnMutexCreate;
	Mutex_Acquire*					m_pfnMutexAcquire;
	Mutex_Release*					m_pfnMutexRelease;
	Mutex_Destroy*					m_pfnMutexDestroy;
	CallStack_Get*					m_pfnCallStackGet;
	ModuleInfo_GetNumLoadedModules*	m_pfnModuleInfoGetNumLoadedModules;
	ModuleInfo_Get*					m_pfnModuleInfoGet;
	Log_Function*					m_pfnLog;
};

namespace BlockingMode
{
	enum Enum
	{
		NON_BLOCKING,
		BLOCKING
	};
}

namespace CommandId
{
	enum Enum
	{
		INITIAL_SETTINGS = 1,
		MODULE_INFO,
		ALLOC,
		FREE,
		FRAME_END,
		TAG,
		ADD_SNAPSHOT,
		TRACED_VAR
	};
}

namespace Platform
{
	enum Enum
	{
		WINDOWS,
		XENON,
		PS3
	};
}

#if RDE_MEMTRACER_ENABLED

// Returns true on success/false on error.
bool Init(FunctionHooks& hooks, unsigned short port, int maxTracedThreads, BlockingMode::Enum mode);
void Shutdown();
void OnAlloc(const void* ptr, size_t bytes, const char* tag);
void OnFree(const void* ptr);
void TagBlock(const void* ptr, const char* tag);
void PushTag(const char* tag);
void PopTag();
void AddSnapshot(const char* snapshotName);
void FrameEnd();
void SetTracedVar(const char* varName, int value);
size_t GetMemoryOverhead();
size_t GetNumTrackedThreads();

#else

inline bool Init(FunctionHooks&, unsigned short, int, BlockingMode::Enum) { return true; }
inline void Shutdown() {}
inline void OnAlloc(const void*, size_t, const char*) {}
inline void OnFree(const void*) {}
inline void TagBlock(const void*, const char*) {}
inline void PushTag(const char*) {}
inline void PopTag() {}
inline void AddSnapshot(const char*) {}
inline void FrameEnd() {}
inline void SetTracedVar(const char*, int) {}
inline size_t GetMemoryOverhead() { return 0; }
inline size_t GetNumTrackedThreads() { return 0; }

#endif

} // MemTracer

#endif
