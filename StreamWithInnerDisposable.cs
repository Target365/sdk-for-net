using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Target365.Sdk
{
	internal class StreamWithInnerDisposable(Stream baseStream, IDisposable innerDisposable) : Stream
	{
		private readonly Stream _decorated = baseStream ?? throw new ArgumentNullException(nameof(baseStream));
		private readonly IDisposable _innerDisposable = innerDisposable ?? throw new ArgumentNullException(nameof(innerDisposable));

		public override bool CanRead => _decorated.CanRead;

		public override bool CanSeek => _decorated.CanSeek;

		public override bool CanWrite => _decorated.CanWrite;

		public override long Length => _decorated.Length;

		public override long Position { get => _decorated.Position; set => _decorated.Position = value; }

		public override void Flush()
		{
			_decorated.Flush();
		}

		/// <summary>
		/// No-op.
		/// </summary>
		/// <param name="cancellationToken">Cancellation token</param>
		public override Task FlushAsync(System.Threading.CancellationToken cancellationToken)
		{
			return _decorated.FlushAsync(cancellationToken);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _decorated.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _decorated.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			_decorated.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			_decorated.Write(buffer, offset, count);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
		{
			return _decorated.WriteAsync(buffer, offset, count, cancellationToken);
		}

#if NET10_0
		public override ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
		{
			return _decorated.WriteAsync(buffer, cancellationToken);
		}
#endif
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				try
				{
					_decorated.Dispose();
				}
				finally
				{
					_innerDisposable.Dispose();
				}
			}

			base.Dispose(disposing);
		}
	}
}
