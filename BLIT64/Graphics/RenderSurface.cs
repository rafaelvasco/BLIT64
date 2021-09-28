using System;
using System.Runtime.InteropServices;

namespace BLIT64
{
    internal class RenderSurface : GameAsset
    {
        internal int Index{ get; }

        public int Width { get; }

        public int Height { get; }

        public uint ByteCount { get; }

        private GCHandle _gc_handle;
        internal readonly IntPtr DataPtr;
        private readonly byte[] _data;

        internal RenderSurface(int index, int width, int height) : this(new byte[width * height * 4], width, height )
        {
            Index = index;
        }

        private RenderSurface(byte[] source_data, int width, int height)
        {
            Width = width;
            Height = height;
            _data = new byte[source_data.Length];
            Buffer.BlockCopy(source_data, 0, _data, 0, source_data.Length);
            ByteCount = (uint)_data.Length;

            _gc_handle = GCHandle.Alloc(_data, GCHandleType.Pinned);
            DataPtr = Marshal.UnsafeAddrOfPinnedArrayElement(_data, 0);
        }

        protected override void FreeUnmanaged()
        {
            _gc_handle.Free();
        }
    }
}
