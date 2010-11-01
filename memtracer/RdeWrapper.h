#ifndef RDE_WRAPPER_H
#define RDE_WRAPPER_H

#if RDE_MEMTRACER_STANDALONE

#include <cassert>

#define RDE_ASSERT			assert

#if defined(COMPILER_GCC)
#	define RDE_THREADLOCAL		__thread
#	define STDCALL
#else
#	include <intrin.h>
#	define RDE_THREADLOCAL		__declspec(thread)
#	define STDCALL				__stdcall
#endif

#define RDE_JOIN_TOKENS(a, b)		RDE_JOIN_TOKENS_IMPL(a, b)
#define RDE_JOIN_TOKENS_IMPL(a,	b)	RDE_JOIN_TOKENS_IMPL2(a, b)
#define RDE_JOIN_TOKENS_IMPL2(a, b)	a ## b

#define RDE_COMPILE_CHECK(expr)	typedef char RDE_JOIN_TOKENS(CC_, __LINE__) [(expr) ? 1 : -1]

#if defined(PLATFORM_PS3) || defined(PLATFORM_XENON)
// Use lwsync for mem barriers here
#	define RDE_BIG_ENDIAN		1
#else // for X86 we're fine with compiler barriers, no need for true mem barriers.
// I assume MSVC for x86. PS3 GCC is handled in the branch above.
#	define MemoryReadBarrier	_ReadBarrier
#	define MemoryWriteBarrier	_WriteBarrier
#	define RDE_LITTLE_ENDIAN	1
#endif

namespace rde
{
typedef unsigned char	uint8;
typedef unsigned long	uint32;
typedef long			Atomic32;

namespace Thread
{
void Sleep(long millis);
};

template<typename T>
inline T Load_Relaxed(const T& v)
{
	T ret = v;
	return ret;
}

template<typename T> 
inline T Load_Acquire(const T& v)
{
	T ret = v;
	MemoryReadBarrier();
	return ret;
}
template<typename T> 
inline void Store_Release(T& dst, T v)
{
	MemoryWriteBarrier();
	dst = v;
}

#if defined(PLATFORM_WIN32) || defined(PLATFORM_XENON)
// Returns a new value of i
inline Atomic32 AtomicInc(volatile Atomic32& i)
{
	return _InterlockedIncrement(reinterpret_cast<volatile long*>(&i));
}
#else if defined(PLATFORM_PS3)
// use Cell atomic here.
#endif

} // rde

#else // RDE_MEMTRACER_STANDALONE

#include "core/Config.h"
#include "core/RdeAssert.h"

#endif // RDE_MEMTRACER_STANDALONE

#endif
