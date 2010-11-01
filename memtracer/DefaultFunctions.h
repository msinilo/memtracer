#ifndef MEMTRACER_DEFAULTFUNCTIONS_H
#define MEMTRACER_DEFAULTFUNCTIONS_H

#include "MemTracer/MemTracer.h"

//#define RDE_MEMTRACER_NEED_DEFAULT_FUNCTIONS	0

// By default - disabled if memtracer disabled, but this can be
// overwritten (if you provide your own functions, no need to link with those).
#ifndef RDE_MEMTRACER_NEED_DEFAULT_FUNCTIONS
#	define RDE_MEMTRACER_NEED_DEFAULT_FUNCTIONS	RDE_MEMTRACER_ENABLED
#endif

namespace MemTracer
{
ThreadHandle DefaultThreadFork(ThreadFunction* function, void* arg, size_t stackSize);
void DefaultThreadJoin(ThreadHandle);

MutexHandle DefaultMutexCreate();
void DefaultMutexAcquire(MutexHandle h);
void DefaultMutexRelease(MutexHandle h);
void DefaultMutexDestroy(MutexHandle h);

int DefaultModuleInfo_GetNumLoadedModules();
void DefaultModuleInfo_Get(int i, ModuleInfo& outModuleInfo);

int DefaultCallStackGet(Address* callStack, int maxDepth, int numEntriesToSkip);
void DefaultLog(const char* msg);

}

#endif
