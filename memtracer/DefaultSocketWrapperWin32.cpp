#include "SocketWrapper.h"
#include "MemTracer.h"

#if (RDE_MEMTRACER_ENABLED) && (RDE_MEMTRACER_USE_DEFAULT_SOCKET_WRAPPER)

#include <Winsock2.h>

#pragma comment(lib, "Ws2_32.lib")

namespace
{
SOCKET AsWinSocket(MemTracer::Socket::Handle h)
{
	return reinterpret_cast<SOCKET>(h);
}
}

namespace MemTracer
{
bool Socket::InitSockets()
{
    WSADATA wsaData;
    const int ret = WSAStartup(MAKEWORD(2, 2), &wsaData);
	return (ret == 0);
}

void Socket::ShutdownSockets()
{
	WSACleanup();
}

Socket::Handle Socket::Create(bool blocking)
{
	SOCKET winSocket = ::socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
	if (winSocket == INVALID_SOCKET)
		return Handle(0);

	unsigned long inonBlocking = (blocking ? 0 : 1);
	return (ioctlsocket(winSocket, FIONBIO, &inonBlocking) == 0 ? Handle(winSocket) : Handle(0));
}

void Socket::Close(Handle& h)
{
	::closesocket(::AsWinSocket(h));
	h = Handle(0);
}

bool Socket::Listen(Handle h, Port port)
{
	sockaddr_in addr = { 0 };
	addr.sin_addr.s_addr = INADDR_ANY;
	addr.sin_family = AF_INET;
	addr.sin_port = htons(port);
	memset(addr.sin_zero, 0, sizeof(addr.sin_zero));
	if (::bind(::AsWinSocket(h), reinterpret_cast<const sockaddr*>(&addr), sizeof(addr)) == SOCKET_ERROR)
	{
		Close(h);
		return false;
	}

	if (::listen(::AsWinSocket(h), SOMAXCONN) == SOCKET_ERROR)
	{
		Close(h);
		return false;
	}

	return true;
}

bool Socket::TestConnection(Handle h)
{
	SOCKET winSocket = ::AsWinSocket(h);

	fd_set readSet;
	FD_ZERO(&readSet);
	FD_SET(winSocket, &readSet );
	timeval timeout = { 0, 17000 };
	const int res = ::select(0, &readSet, 0, 0, &timeout);
	if (res > 0)
	{
		if (FD_ISSET(winSocket, &readSet))
			return true;
	}
	return false;
}

Socket::Handle Socket::Accept(Handle listeningSocket, size_t bufferSize)
{
	typedef int socklen_t;
	sockaddr_in addr;
	socklen_t len = sizeof(sockaddr_in);
	memset(&addr, 0, sizeof(sockaddr_in));
	SOCKET outSocket = ::accept(::AsWinSocket(listeningSocket), (sockaddr*)&addr, &len);
	if (outSocket != SOCKET_ERROR)
	{
		int sizeOfBufSize = sizeof(bufferSize);
		::setsockopt(outSocket, SOL_SOCKET, SO_RCVBUF, (const char*)&bufferSize, sizeOfBufSize);
		::setsockopt(outSocket, SOL_SOCKET, SO_SNDBUF, (const char*)&bufferSize, sizeOfBufSize);
		return Handle(outSocket);
	}
	return Handle(0);
}

bool Socket::Write(Handle& h, const void* buffer, size_t bytes, size_t& outBytesWritten)
{
	outBytesWritten = 0;
	if (bytes == 0 || !h)
		return true;

	const int res = ::send(::AsWinSocket(h), (const char*)buffer, (int)bytes, 0);
	if (res == SOCKET_ERROR)
	{
		const int err = WSAGetLastError();
		if (err == WSAECONNRESET || err == WSAECONNABORTED)
		{
			Close(h);
		}
	}
	else
	{
		outBytesWritten = res;
	}
	return outBytesWritten != 0;
}

}

#endif
