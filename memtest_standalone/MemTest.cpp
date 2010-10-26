#include "MemTracer/MemTracer.h"

#include <cstdio>
#include <new>
#include <stack>
#include <string>

#include <process.h>		// beginthreadex
#include <windows.h>		// Sleep

void* MyAlloc(size_t bytes)
{
	void* ptr = malloc(bytes);
	MemTracer::OnAlloc(ptr, bytes, "TEST");
	return ptr;
}

void MyFree(void* ptr)
{
	MemTracer::OnFree(ptr);
	free(ptr);
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

struct ResourceStack
{
	typedef std::string ResourceTag;

	ResourceStack()
	{
		InitializeCriticalSection(&m_resourcesMutex);
	}
	~ResourceStack()
	{
		DeleteCriticalSection(&m_resourcesMutex);
	}

	ResourceTag PopResourceToLoad()
	{
		EnterCriticalSection(&m_resourcesMutex);
		
		if (m_resourcesToLoad.empty())
		{
			LeaveCriticalSection(&m_resourcesMutex);
			return ResourceTag();
		}

		ResourceTag retTag = m_resourcesToLoad.top();
		m_resourcesToLoad.pop();
		LeaveCriticalSection(&m_resourcesMutex);
		return retTag;
	}
	void AddResourceToLoad(const ResourceTag& tag)
	{
		m_resourcesToLoad.push(tag);
	}

	typedef std::stack<ResourceTag>			Tags;
	Tags				m_resourcesToLoad;
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
			fseek(f, 0, SEEK_END);
			const long fileSize = ftell(f);
			fseek(f, 0, SEEK_SET);

			char* buffer = new char[fileSize];
			(void)buffer;

			fclose(f);
		}

		MemTracer::PopTag();
	}
	bool IsDone() const { return m_done; }

	ResourceStack*	m_stack;
	HANDLE			m_hThread;
	volatile bool	m_done;
};

void LoadLevel()
{
	ResourceStack resStack;

	const int maxResourcesToLoad = 200;

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
	
	static const int NUM_LOADERS = 9;
	ResourceLoader loaders[NUM_LOADERS];
	for (int i = 1; i < NUM_LOADERS; ++i)
	{
		loaders[i].Init(resStack);
		loaders[i].StartThread();
	}
	loaders[0].Init(resStack);
	loaders[0].MainLoop();

	bool allDone(false);
	while (!allDone)
	{
		int numDone(0);
		for (int i = 1; i < NUM_LOADERS; ++i)
		{
			if (loaders[i].IsDone())
			{
				++numDone;
			}
		}
		allDone = (numDone == NUM_LOADERS - 1);
	}
}

void Frame()
{
	static int s_frameNr = 0;

	if (((++s_frameNr) % 100) == 0)
	{
		operator new(1000);
		MemTracer::SetTracedVar("TestVar", s_frameNr);
	}

	Sleep(20);
	MemTracer::FrameEnd();
}


int __cdecl main(int, char const *[])
{
	printf("MemTracer: waiting for connection...\n");
	MemTracer::FunctionHooks hooks;
	if (!MemTracer::Init(hooks, 1000, 10, MemTracer::BlockingMode::BLOCKING))
	{
		printf("Failed to initialize MemTracer\n");
	}
	printf("MemTracer: Connected, memory overhead ~= %d kb\n", MemTracer::GetMemoryOverhead() >> 10);

	LoadLevel();
	MemTracer::AddSnapshot("Level loaded");

	bool done(false);
	while (!done)
	{
		Frame();
	}

	printf("Done, number of threads generating data: %d\n", MemTracer::GetNumTrackedThreads());
	MemTracer::Shutdown();
	return 0;
}
