#ifndef MEMTRACER_SOCKET_WRAPPER_H
#define MEMTRACER_SOCKET_WRAPPER_H

// Undefine if you prefer to provide your own socket functions.
// Link with your own .cpp containing definitions of functions listed here.
#define RDE_MEMTRACER_USE_DEFAULT_SOCKET_WRAPPER	1

namespace MemTracer
{
// Minimal subset of functionality that we need.
namespace Socket
{
typedef void*			Handle;
typedef unsigned short	Port;

// Called once at application start/end
// (De)initializes the whole network/socket system.
bool InitSockets();
void ShutdownSockets();

// Creates/initializes socket. Returns empty (NULL) handle on error.
Handle Create(bool blocking);
// Closes socket.
void Close(Handle&);

bool Listen(Handle, Port port);
// Non-blocking, returns true if connected.
bool TestConnection(Handle);
// Returns handle of accepted socket.
// bufferSize = sending/receiving buffers size in bytes.
Handle Accept(Handle listeningSocket, size_t bufferSize);

bool Write(Handle& h, const void* buffer, size_t bytes, size_t& outBytesWritten);

} // Socket
} // MemTracer

#endif
