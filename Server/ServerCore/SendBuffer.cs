using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCore
{
    public class SendBufferHelper
    {
        public static ThreadLocal<SendBuffer> CurrentBuffer = new ThreadLocal<SendBuffer>(() => { return null; });

        public static int ChunkSize { get; set; } = 65535 * 100;
            
        public static ArraySegment<byte>Open(int reserverSize)
        {
            if (CurrentBuffer.Value == null)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            if (CurrentBuffer.Value.FreeSize < reserverSize)
                CurrentBuffer.Value = new SendBuffer(ChunkSize);

            return CurrentBuffer.Value.Open(reserverSize);
        }

        public static ArraySegment<byte> Close (int usedSize)
        {
            return CurrentBuffer.Value.Close(usedSize);
        }
            
    }

    public class SendBuffer
    {
        byte[] _buffer;
        int _usedSize = 0;

        public int FreeSize { get { return _buffer.Length - _usedSize; } }

        public SendBuffer(int chunkSize)
        {
            _buffer = new byte[chunkSize];
        }

        public ArraySegment<byte> Open(int reserverSize)
        {
            if (reserverSize > FreeSize)
                return null;

            return new ArraySegment<byte>(_buffer, _usedSize, reserverSize);
        }

        public ArraySegment<byte> Close(int usedSize)
        {
            ArraySegment<byte> segmnent = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
            _usedSize += usedSize;
            return segmnent;
        }
    }
}
