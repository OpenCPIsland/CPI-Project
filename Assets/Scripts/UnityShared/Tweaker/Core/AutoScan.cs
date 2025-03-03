using System;
using System.Collections.Generic;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class AutoScan<T> : AutoScanBase, IDisposable where T : new()
	{
		public T value;

		private List<ITweakerObject> scannedObjects;

		public AutoScan()
			: this(new T())
		{
		}

		public AutoScan(T value)
		{
			this.value = value;
			scannedObjects = new List<ITweakerObject>();
			if (AutoScanBase.Scanner == null)
			{
				throw new AutoScanException("No scanner has been set. Set a scanner through AutoScan.Scanner before creating auto scannable instances.");
			}
			AutoScanBase.Scanner.GetResultProvider<IInvokable>().ResultProvided += OnInvokableScanned;
			AutoScanBase.Scanner.GetResultProvider<ITweakable>().ResultProvided += OnTweakableScanned;
			AutoScanBase.Scanner.GetResultProvider<IWatchable>().ResultProvided += OnWatchableScanned;
			AutoScanBase.Scanner.ScanInstance(value);
			AutoScanBase.Scanner.GetResultProvider<IInvokable>().ResultProvided -= OnInvokableScanned;
			AutoScanBase.Scanner.GetResultProvider<ITweakable>().ResultProvided -= OnTweakableScanned;
			AutoScanBase.Scanner.GetResultProvider<IWatchable>().ResultProvided -= OnWatchableScanned;
		}

		~AutoScan()
		{
			Dispose();
		}

		private void OnInvokableScanned(object sender, ScanResultArgs<IInvokable> e)
		{
			scannedObjects.Add(e.result);
		}

		private void OnTweakableScanned(object sender, ScanResultArgs<ITweakable> e)
		{
			scannedObjects.Add(e.result);
		}

		private void OnWatchableScanned(object sender, ScanResultArgs<IWatchable> e)
		{
			scannedObjects.Add(e.result);
		}

		public void Dispose()
		{
			if (scannedObjects != null)
			{
				foreach (ITweakerObject scannedObject in scannedObjects)
				{
					if (scannedObject is IInvokable)
					{
						if (AutoInvokableBase.Manager != null)
						{
							AutoInvokableBase.Manager.UnregisterInvokable(scannedObject as IInvokable);
						}
					}
					else
					{
						if (!(scannedObject is ITweakable))
						{
							if (scannedObject is IWatchable)
							{
								throw new NotImplementedException();
							}
							throw new AutoScanException("Could not unregister invalid ITweaketObject");
						}
						if (AutoTweakable.Manager != null)
						{
							AutoTweakable.Manager.UnregisterTweakable(scannedObject as ITweakable);
						}
					}
				}
			}
		}
	}
}
