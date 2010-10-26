#include "MemTracer.h"

#if RDE_MEMTRACER_STANDALONE

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <windows.h>

// For PS3 see official documentation.

namespace rde
{
	void Thread::Sleep(long millis)
	{
		::Sleep(millis);
	}
}
#endif
