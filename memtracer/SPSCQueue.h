#ifndef SPSCQUEUE_H
#define SPSCQUEUE_H

namespace rde
{

template<typename T, uint32 TSize>
struct SPSCQueue
{
public:
   SPSCQueue() :
       m_pushIndex(0),
       m_popIndex(0)
   {
   }

   uint32 Size() const
   {
       const uint32 popIndex = Load_Acquire(m_popIndex);
       const uint32 pushIndex = Load_Acquire(m_pushIndex);
       return pushIndex - popIndex;
   }
   bool IsFull() const
   {
       const uint32 c = Size();
       return c == TSize;
   }

   void Push(const T& t)
   {
       uint32 pushIndex = Load_Relaxed(m_pushIndex) ;
       const uint32 index = (pushIndex & (TSize - 1));
       m_buffer[index] = t;
       Store_Release(m_pushIndex, pushIndex + 1);
   }

   // NULL if queue is empty.
   T* Peek()
   {
       const uint32 popIndex = Load_Relaxed(m_popIndex);
       const uint32 pushIndex = Load_Acquire(m_pushIndex);
       if(pushIndex <= popIndex)
           return NULL;

       const uint32 index = popIndex & (TSize - 1);
       return &m_buffer[index];
   }
   void Pop() // use in conjuction with Peek()
   {
       const uint32 popIndex = Load_Relaxed(m_popIndex);
       Store_Release(m_popIndex, popIndex + 1);
   }

private:
	SPSCQueue(const SPSCQueue&);
	SPSCQueue& operator=(const SPSCQueue&);

	static const int kCacheLineSize = 32;
	typedef uint8	PadBuffer[kCacheLineSize - 4];

	uint32		m_pushIndex;
	PadBuffer	m_padding0;
	uint32		m_popIndex;
	PadBuffer	m_padding1;
	T			m_buffer[TSize];
};

} // rde

#endif
