using System;
using System.Collections.Generic;
using System.Threading;

namespace DI.Threading
{
	public class Channel<T> : IDisposable
	{
		private List<T> buffer = new List<T>();

		private object setSyncRoot = new object();

		private object getSyncRoot = new object();

		private object disposeRoot = new object();

		private ManualResetEvent setEvent = new ManualResetEvent(false);

		private ManualResetEvent getEvent = new ManualResetEvent(true);

		private ManualResetEvent exitEvent = new ManualResetEvent(false);

		private bool disposed = false;

		public int BufferSize
		{
			get;
			private set;
		}

		public Channel()
			: this(1)
		{
		}

		public Channel(int bufferSize)
		{
			if (bufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("bufferSize", "Must be greater or equal to 1.");
			}
			BufferSize = bufferSize;
		}

		~Channel()
		{
			Dispose();
		}

		public void Resize(int newBufferSize)
		{
			if (newBufferSize < 1)
			{
				throw new ArgumentOutOfRangeException("newBufferSize", "Must be greater or equal to 1.");
			}
			lock (setSyncRoot)
			{
				if (!disposed && WaitHandle.WaitAny(new WaitHandle[2]
				{
					exitEvent,
					getEvent
				}) != 0)
				{
					buffer.Clear();
					if (newBufferSize != BufferSize)
					{
						BufferSize = newBufferSize;
					}
				}
			}
		}

		public bool Set(T value)
		{
			return Set(value, int.MaxValue);
		}

		public bool Set(T value, int timeoutInMilliseconds)
		{
			lock (setSyncRoot)
			{
				if (disposed)
				{
					return false;
				}
				int num = WaitHandle.WaitAny(new WaitHandle[2]
				{
					exitEvent,
					getEvent
				}, timeoutInMilliseconds);
				if (num == 258 || num == 0)
				{
					return false;
				}
				buffer.Add(value);
				if (buffer.Count == BufferSize)
				{
					setEvent.Set();
					getEvent.Reset();
				}
				return true;
			}
		}

		public T Get()
		{
			return Get(int.MaxValue, default(T));
		}

		public T Get(int timeoutInMilliseconds, T defaultValue)
		{
			lock (getSyncRoot)
			{
				if (disposed)
				{
					return defaultValue;
				}
				int num = WaitHandle.WaitAny(new WaitHandle[2]
				{
					exitEvent,
					setEvent
				}, timeoutInMilliseconds);
				if (num == 258 || num == 0)
				{
					return defaultValue;
				}
				T result = buffer[0];
				buffer.RemoveAt(0);
				if (buffer.Count == 0)
				{
					getEvent.Set();
					setEvent.Reset();
				}
				return result;
			}
		}

		public void Close()
		{
			lock (disposeRoot)
			{
				if (!disposed)
				{
					exitEvent.Set();
				}
			}
		}

		public void Dispose()
		{
			if (!disposed)
			{
				lock (disposeRoot)
				{
					exitEvent.Set();
					lock (getSyncRoot)
					{
						lock (setSyncRoot)
						{
							setEvent.Close();
							setEvent = null;
							getEvent.Close();
							getEvent = null;
							exitEvent.Close();
							exitEvent = null;
							disposed = true;
						}
					}
				}
			}
		}
	}
	public class Channel : Channel<object>
	{
	}
}
