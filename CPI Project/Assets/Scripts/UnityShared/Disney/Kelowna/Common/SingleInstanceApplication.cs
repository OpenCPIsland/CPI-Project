using System;
using System.Threading;

namespace Disney.Kelowna.Common
{
	public class SingleInstanceApplication : IDisposable
	{
		public const string CLIENT_MUTEX_NAME = "MUTEX_F77CC12D-3096-40F1-8D24-A3EE6AEC72B4";

		public const string LAUNCHER_MUTEX_NAME = "MUTEX_620CCF48-01A3-453C-A5ED-C18A8D1724E6";

		private readonly bool isAnotherProcessRunning;

		private readonly string mutexName;

		private readonly Mutex mutex;

		private bool isMutexSignalled = false;

		private bool disposed = false;

		public static SingleInstanceApplication CreateServerInstance(string mutexName)
		{
			return new SingleInstanceApplication(mutexName, true);
		}

		public static SingleInstanceApplication CreateClientInstance(string mutexName)
		{
			return new SingleInstanceApplication(mutexName, false);
		}

		private SingleInstanceApplication(string mutexName, bool createServerInstance)
		{
			try
			{
				this.mutexName = mutexName;
				mutex = new Mutex(false, this.mutexName);
				isMutexSignalled = mutex.WaitOne(10, false);
				isAnotherProcessRunning = !isMutexSignalled;
				if (createServerInstance)
				{
					if (!isMutexSignalled)
					{
						mutex = null;
					}
				}
				else
				{
					if (isMutexSignalled)
					{
						isMutexSignalled = false;
						mutex.ReleaseMutex();
					}
					mutex = null;
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		~SingleInstanceApplication()
		{
			Dispose(false);
		}

		public bool IsAnotherProcessRunning()
		{
			return isAnotherProcessRunning;
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!disposed)
			{
				if (disposing)
				{
				}
				if (mutex != null && isMutexSignalled)
				{
					isMutexSignalled = false;
					mutex.ReleaseMutex();
				}
				disposed = true;
			}
		}
	}
}
