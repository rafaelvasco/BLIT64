using System;
using System.IO;
using System.Runtime.InteropServices;

namespace STB
{
	internal unsafe class ImageWriter
    {
		private Stream _stream;
		private byte[] _buffer = new byte[1024];

		private int WriteCallback(void* context, void* data, int size)
		{
			if (data == null || size <= 0)
			{
				return 0;
			}

			if (_buffer.Length < size)
			{
				_buffer = new byte[size * 2];
			}

			var bptr = (byte*)data;

			Marshal.Copy(new IntPtr(bptr), _buffer, 0, size);

			_stream.Write(_buffer, 0, size);

			return size;
		}

		private void CheckParams(byte[] data, int width, int height, ColorComponents components)
		{
			if (data == null)
			{
				throw new ArgumentNullException("data");
			}

			if (width <= 0)
			{
				throw new ArgumentOutOfRangeException("width");
			}

			if (height <= 0)
			{
				throw new ArgumentOutOfRangeException("height");
			}

			int requiredDataSize = width * height * (int)components;
			if (data.Length < requiredDataSize)
			{
				throw new ArgumentException(
					string.Format("Not enough data. 'data' variable should contain at least {0} bytes.", requiredDataSize));
			}
		}

		public void WritePng(void* data, int width, int height, ColorComponents components, Stream dest)
		{
			try
			{
				_stream = dest;

				StbImageWrite.stbi_write_png_to_func(WriteCallback, null, width, height, (int)components, data,
				   width * (int)components);
			}
			finally
			{
				_stream = null;
			}
		}

		public void WritePng(byte[] data, int width, int height, ColorComponents components, Stream dest)
		{
			CheckParams(data, width, height, components);

			fixed (byte* b = &data[0])
			{
				WritePng(data, width, height, components, dest);
			}
		}

		
	}
}
