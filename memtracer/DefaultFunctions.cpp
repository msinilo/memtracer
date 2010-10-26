#include "DefaultFunctions.h"

#if RDE_MEMTRACER_ENABLED

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>
#include <process.h>		// beginthreadex
#include <dbghelp.h>

#pragma comment(lib, "dbghelp.lib")

namespace
{
const int kMaxLoadedModules = 64;
MemTracer::ModuleInfo	s_loadedModules[kMaxLoadedModules];
int						s_numLoadedModules(0);

MemTracer::Address* GetNextStackFrame(MemTracer::Address* prevSP)
{
	MemTracer::Address* newSP = (MemTracer::Address*)(*prevSP);
	if (newSP == prevSP)
		return 0;
	// Difference between stack pointers has to be sane.
	if (newSP > prevSP && ((uintptr_t)newSP - (uintptr_t)prevSP) > 1000000)
		return 0;
	if ((uintptr_t)newSP & (sizeof(void*) - 1))
		return 0;

	return newSP;
}

BOOL CALLBACK EnumerateModulesProc(PCSTR name, DWORD64 base, ULONG size, PVOID)
{
	if (SymLoadModule64(GetCurrentProcess(), 0, name, 0, base, size))
	{
		IMAGEHLP_MODULE64 moduleInfo;
		ZeroMemory(&moduleInfo, sizeof(IMAGEHLP_MODULE64));
		moduleInfo.SizeOfStruct = sizeof(IMAGEHLP_MODULE64);
		if (s_numLoadedModules < kMaxLoadedModules && 
			SymGetModuleInfo64(GetCurrentProcess(), base, &moduleInfo))
		{
			s_loadedModules[s_numLoadedModules].moduleBase = (unsigned long)(base & 0xFFFFFFFF);
			s_loadedModules[s_numLoadedModules].moduleSize = size;
			strncpy(s_loadedModules[s_numLoadedModules].debugInfoFile, moduleInfo.LoadedPdbName,
				sizeof(s_loadedModules[s_numLoadedModules].debugInfoFile) - 1);
			if (strlen(s_loadedModules[s_numLoadedModules].debugInfoFile) > 0)
			{
				++s_numLoadedModules;
			}
		}
		else if (s_numLoadedModules == kMaxLoadedModules)
		{
			MemTracer::DefaultLog("Too many modules, interrupting enumeration. Increase kMaxLoadedModules\n");
			return FALSE;
		}
	}
	return TRUE;
}

void EnumerateModules()
{
	if (s_numLoadedModules == 0)
	{
		if (SymInitialize(GetCurrentProcess(), 0, FALSE) == 0)
		{
			MemTracer::DefaultLog("Failure initializing symbol API.\n");
			return;
		}

		PVOID userContext(NULL);
		EnumerateLoadedModules64(GetCurrentProcess(), &EnumerateModulesProc, userContext);
	}
}

} // namespace

namespace MemTracer
{
ThreadHandle DefaultThreadFork(ThreadFunction* function, void* arg, size_t stackSize)
{
	const DWORD creationFlags = 0x0;
	uintptr_t hThread = ::_beginthreadex(NULL, (unsigned)stackSize, function, arg, creationFlags, NULL);
	return (ThreadHandle)hThread;
}
void DefaultThreadJoin(ThreadHandle h)
{
    DWORD waitRc = WaitForSingleObject(h, INFINITE);
    if (waitRc == WAIT_OBJECT_0)
    {
        CloseHandle(h);
    }
}

MutexHandle DefaultMutexCreate()
{
	CRITICAL_SECTION* cs = new CRITICAL_SECTION();
	::InitializeCriticalSection(cs);
	return cs;
}
void DefaultMutexAcquire(MutexHandle h)
{
	::EnterCriticalSection((CRITICAL_SECTION*)h);
}
void DefaultMutexRelease(MutexHandle h)
{
	::LeaveCriticalSection((CRITICAL_SECTION*)h);
}
void DefaultMutexDestroy(MutexHandle h)
{
	::DeleteCriticalSection((CRITICAL_SECTION*)h);
}

int DefaultModuleInfo_GetNumLoadedModules()
{
	::EnumerateModules();
	return s_numLoadedModules;
}
void DefaultModuleInfo_Get(int i, ModuleInfo& outModuleInfo)
{
	::EnumerateModules();
	RDE_ASSERT(i < s_numLoadedModules);
	outModuleInfo = s_loadedModules[i];
}

int DefaultCallStackGet(Address* callStack, int maxDepth, int numEntriesToSkip)
{
	uintptr_t ebpReg;
	__asm mov [ebpReg], ebp
	Address* sp = (Address*)ebpReg;
	int numEntries(0);
	while (sp && numEntries < maxDepth)
	{
		if (numEntriesToSkip > 0)
			--numEntriesToSkip;
		else
			callStack[numEntries++] = sp[1];

		sp = ::GetNextStackFrame(sp);
	}
	return numEntries;
}

void DefaultLog(const char* msg)
{
	::OutputDebugString(msg);
}

} // MemTracer 


#else // RDE_MEMTRACER_ENABLED

namespace MemTracer
{
ThreadHandle DefaultThreadFork(ThreadFunction*, void*, size_t)
{
	return ThreadHandle();
}
void DefaultThreadJoin(ThreadHandle) {}

MutexHandle DefaultMutexCreate()
{
	return MutexHandle();
}
void DefaultMutexAcquire(MutexHandle) {}
void DefaultMutexRelease(MutexHandle) {}
void DefaultMutexDestroy(MutexHandle) {}

int DefaultModuleInfo_GetNumLoadedModules()
{
	return 0;
}
void DefaultModuleInfo_Get(int, ModuleInfo&) {}

int DefaultCallStackGet(Address*, int, int)
{
	return 0;
}
void DefaultLog(const char*) {}

} // MemTracer

#endif

