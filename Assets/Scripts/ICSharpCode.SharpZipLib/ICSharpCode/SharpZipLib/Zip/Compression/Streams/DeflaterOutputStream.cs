using ICSharpCode.SharpZipLib.Encryption;
using System;
using System.IO;
using System.Security.Cryptography;

namespace ICSharpCode.SharpZipLib.Zip.Compression.Streams
{
	public class DeflaterOutputStream : Stream
	{
		private string password;

		private ICryptoTransform cryptoTransform_;

		protected byte[] AESAuthCode;

		private byte[] buffer_;

		protected Deflater deflater_;

		protected Stream baseOutputStream_;

		private bool isClosed_;

		private bool isStreamOwner_ = true;

		public bool IsStreamOwner
		{
			get
			{
				return isStreamOwner_;
			}
			set
			{
				isStreamOwner_ = value;
			}
		}

		public bool CanPatchEntries
		{
			get
			{
				return baseOutputStream_.CanSeek;
			}
		}

		public string Password
		{
			get
			{
				return password;
			}
			set
			{
				if (value != null && value.Length == 0)
				{
					password = null;
				}
				else
				{
					password = value;
				}
			}
		}

		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return baseOutputStream_.CanWrite;
			}
		}

		public override long Length
		{
			get
			{
				return baseOutputStream_.Length;
			}
		}

		public override long Position
		{
			get
			{
				return baseOutputStream_.Position;
			}
			set
			{
				throw new NotSupportedException("Position property not supported");
			}
		}

		public DeflaterOutputStream(Stream baseOutputStream)
			: this(baseOutputStream, new Deflater(), 512)
		{
		}

		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater)
			: this(baseOutputStream, deflater, 512)
		{
		}

		public DeflaterOutputStream(Stream baseOutputStream, Deflater deflater, int bufferSize)
		{
			if (baseOutputStream == null)
			{
				throw new ArgumentNullException("baseOutputStream");
			}
			if (!baseOutputStream.CanWrite)
			{
				throw new ArgumentException("Must support writing", "baseOutputStream");
			}
			if (deflater == null)
			{
				throw new ArgumentNullException("deflater");
			}
			if (bufferSize < 512)
			{
				throw new ArgumentOutOfRangeException("bufferSize");
			}
			baseOutputStream_ = baseOutputStream;
			buffer_ = new byte[bufferSize];
			deflater_ = deflater;
		}

		public virtual void Finish()
		{
			deflater_.Finish();
			while (!deflater_.IsFinished)
			{
				int num = deflater_.Deflate(buffer_, 0, buffer_.Length);
				if (num <= 0)
				{
					break;
				}
				if (cryptoTransform_ != null)
				{
					EncryptBlock(buffer_, 0, num);
				}
				baseOutputStream_.Write(buffer_, 0, num);
			}
			if (!deflater_.IsFinished)
			{
				throw new SharpZipBaseException("Can't deflate all input?");
			}
			baseOutputStream_.Flush();
			if (cryptoTransform_ != null)
			{
				cryptoTransform_.Dispose();
				cryptoTransform_ = null;
			}
		}

		protected void EncryptBlock(byte[] buffer, int offset, int length)
		{
			cryptoTransform_.TransformBlock(buffer, 0, length, buffer, 0);
		}

		protected void InitializePassword(string password)
		{
			PkzipClassicManaged pkzipClassicManaged = new PkzipClassicManaged();
			byte[] rgbKey = PkzipClassic.GenerateKeys(ZipConstants.ConvertToArray(password));
			cryptoTransform_ = pkzipClassicManaged.CreateEncryptor(rgbKey, null);
		}

		protected void Deflate()
		{
			while (!deflater_.IsNeedingInput)
			{
				int num = deflater_.Deflate(buffer_, 0, buffer_.Length);
				if (num <= 0)
				{
					break;
				}
				if (cryptoTransform_ != null)
				{
					EncryptBlock(buffer_, 0, num);
				}
				baseOutputStream_.Write(buffer_, 0, num);
			}
			if (!deflater_.IsNeedingInput)
			{
				throw new SharpZipBaseException("DeflaterOutputStream can't deflate all input?");
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("DeflaterOutputStream Seek not supported");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException("DeflaterOutputStream SetLength not supported");
		}

		public override int ReadByte()
		{
			throw new NotSupportedException("DeflaterOutputStream ReadByte not supported");
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("DeflaterOutputStream Read not supported");
		}

		public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("DeflaterOutputStream BeginRead not currently supported");
		}

		public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
		{
			throw new NotSupportedException("BeginWrite is not supported");
		}

		public override void Flush()
		{
			deflater_.Flush();
			Deflate();
			baseOutputStream_.Flush();
		}

		public override void Close()
		{
			if (!isClosed_)
			{
				isClosed_ = true;
				try
				{
					Finish();
					if (cryptoTransform_ != null)
					{
						GetAuthCodeIfAES();
						cryptoTransform_.Dispose();
						cryptoTransform_ = null;
					}
				}
				finally
				{
					if (isStreamOwner_)
					{
						baseOutputStream_.Close();
					}
				}
			}
		}

		private void GetAuthCodeIfAES()
		{
		}

		public override void WriteByte(byte value)
		{
			Write(new byte[1]
			{
				value
			}, 0, 1);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			deflater_.SetInput(buffer, offset, count);
			Deflate();
		}
	}
}
