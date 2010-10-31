using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace MemTracer
{
    public class MsgReceiver
    {
        // Assumes the following message format:
        //  - 1 byte with message size (not including this byte),
        //  - message contents 

        public List<byte[]> OnDataReceived(byte[] dataBuffer, int receivedBytes)
        {
            List<byte[]> messages = new List<byte[]>();
            if (receivedBytes > 0)
            {
                int dataIndex = 0;

                // Combine data that we received now with whatever incomplete messages
                // we may have from previous call.
                if (m_pendingData != null)
                {
                    byte[] combinedBuffer = new byte[receivedBytes + m_pendingData.Length];
                    Array.Copy(m_pendingData, 0, combinedBuffer, 0, m_pendingData.Length);
                    Array.Copy(dataBuffer, 0, combinedBuffer, m_pendingData.Length, receivedBytes);
                    receivedBytes += m_pendingData.Length;
                    dataBuffer = combinedBuffer;
                    m_pendingData = null;
                }

                while (dataIndex < receivedBytes)
                {
                    // We only got message size, maybe not even that. Save & continue next time.
                    if (receivedBytes - dataIndex <= 1)
                    {
                        SavePendingData(dataBuffer, dataIndex, receivedBytes);
                        break;
                    }

                    int messageSize = dataBuffer[dataIndex];
                    // Incomplete message.
                    if (receivedBytes - (dataIndex + 1) < messageSize)
                    {
                        SavePendingData(dataBuffer, dataIndex, receivedBytes);
                        break;
                    }

                    dataIndex += 1;
                    byte[] fullMessage = new byte[messageSize];
                    Array.Copy(dataBuffer, dataIndex, fullMessage, 0, messageSize);
                    dataIndex += messageSize;
                    messages.Add(fullMessage);
                }
            }
            return messages;
        }
        void SavePendingData(byte[] data, int index, int len)
        {
            int toSave = len - index;
            m_pendingData = new byte[toSave];
            Array.Copy(data, index, m_pendingData, 0, toSave);
        }

        byte[] m_pendingData = null;
    };
}
