#ifndef MEMTRACER_DEFAULTFUNCTIONS_H
#define MEMTRACER_DEFAULTFUNCTIONS_H

#include "MemTracer/MemTracer.h"

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
