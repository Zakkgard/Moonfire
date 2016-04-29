namespace Moonfire.Core.Networking
{
    using System.Collections.Generic;
    using System.Net.Sockets;

    public class BufferManager
    {
        public BufferManager(int totalBytes, int bufferSize)
        {
            this.NumBytes = totalBytes;
            this.CurrentIndex = 0;
            this.BufferSize = bufferSize;
            this.FreeIndexPool = new Stack<int>();

            this.InitializeBuffer();
        }
        
        private int NumBytes { get; set; } 

        private byte[] Buffer { get; set; }

        private Stack<int> FreeIndexPool { get; set; }

        private int CurrentIndex { get; set; }

        private int BufferSize { get; set; }

        public void InitializeBuffer()
        {
            this.Buffer = new byte[this.NumBytes];
        }

        public bool SetBuffer(SocketAsyncEventArgs args)
        {
            if (this.FreeIndexPool.Count > 0)
            {
                args.SetBuffer(this.Buffer, this.FreeIndexPool.Pop(), this.BufferSize);
            }
            else
            {
                if ((this.NumBytes - this.BufferSize) < this.CurrentIndex)
                {
                    return false;
                }

                args.SetBuffer(this.Buffer, this.CurrentIndex, this.BufferSize);
                this.CurrentIndex += this.BufferSize;
            }

            return true;
        }

        public void FreeBuffer(SocketAsyncEventArgs args)
        {
            this.FreeIndexPool.Push(args.Offset);
            args.SetBuffer(null, 0, 0);
        }
    }
}
