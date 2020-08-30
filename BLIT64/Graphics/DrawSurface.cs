using System;
using System.Runtime.InteropServices;

namespace BLIT64
{
    internal class DrawSurface : GameAsset
    {
        public int Width { get; }

        public int Height { get; }

        public uint ByteCount { get; }

        public int Pitch => Width * 4;

        private GCHandle _gc_handle;
        internal readonly IntPtr DataPtr;
        private byte[] _data;

        ~DrawSurface()
        {
            Console.WriteLine("RenderSurface Leak");
            Dispose(false);
        }

        internal DrawSurface(int width, int height) : this(new byte[width * height * 4], width, height )
        {
        }

        internal DrawSurface(byte[] source_data, int width, int height)
        {
            Width = width;
            Height = height;
            _data = new byte[source_data.Length];
            Buffer.BlockCopy(source_data, 0, _data, 0, source_data.Length);
            ByteCount = (uint)_data.Length;

            _gc_handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            DataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(_data, 0);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    _data = null;
                }

                _gc_handle.Free();
                GC.SuppressFinalize(this);
                disposed = true;
            }
        }
    }
}
