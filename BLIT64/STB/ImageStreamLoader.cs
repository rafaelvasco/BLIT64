using System;
using System.IO;
using System.Runtime.InteropServices;

namespace STB
{
	internal unsafe class ImageStreamLoader
    {
		private Stream _stream;
		private byte[] _buffer = new byte[1024];

		private readonly StbImage.stbi_io_callbacks _callbacks;

		public ImageStreamLoader()
		{
			_callbacks = new StbImage.stbi_io_callbacks
			{
				read = ReadCallback,
				skip = SkipCallback,
				eof = Eof
			};
		}

		private int SkipCallback(void* user, int i)
		{
			return (int)_stream.Seek(i, SeekOrigin.Current);
		}

		private int Eof(void* user)
		{
			return _stream.CanRead ? 1 : 0;
		}

		private int ReadCallback(void* user, sbyte* data, int size)
		{
			if (size > _buffer.Length)
			{
				_buffer = new byte[size * 2];
			}

			var res = _stream.Read(_buffer, 0, size);
			Marshal.Copy(_buffer, 0, new IntPtr(data), size);
			return res;
		}

		public ImageResult Load(Stream stream, ColorComponents requiredComponents = ColorComponents.Default)
		{
			byte* result = null;

			_stream = stream;

			try
			{
				int x, y, comp;
				result = StbImage.stbi_load_from_callbacks(_callbacks, null, &x, &y, &comp, (int)requiredComponents);

				return ImageResult.FromResult(result, x, y, (ColorComponents)comp, requiredComponents);
			}
			finally
			{
				if (result != null)
				{
					CRuntime.free(result);
				}
				_stream = null;
			}
		}

	}
}
