using System;
using Tweaker.AssemblyScanner;

namespace Tweaker.Core
{
	public class AutoTweakable : IDisposable
	{
		private static AutoTweakableProcessor s_processor;

		public ITweakable tweakable;

		public static ITweakableManager Manager
		{
			get;
			set;
		}

		public uint UniqueId
		{
			get;
			private set;
		}

		static AutoTweakable()
		{
			s_processor = new AutoTweakableProcessor();
		}

		~AutoTweakable()
		{
			if (tweakable != null)
			{
				Dispose();
			}
		}

		public static void Bind<TContainer>(TContainer container)
		{
			if (CheckForManager())
			{
				IScanner scanner = new Scanner();
				scanner.AddProcessor(s_processor);
				IScanResultProvider<AutoTweakableResult> resultProvider = scanner.GetResultProvider<AutoTweakableResult>();
				resultProvider.ResultProvided += OnResultProvided;
				scanner.ScanInstance(container);
				resultProvider.ResultProvided -= OnResultProvided;
			}
		}

		private static void OnResultProvided(object sender, ScanResultArgs<AutoTweakableResult> e)
		{
			if (CheckForManager())
			{
				ITweakable tweakble = e.result.tweakble;
				Manager.RegisterTweakable(tweakble);
				e.result.autoTweakable.tweakable = tweakble;
				e.result.autoTweakable.UniqueId = e.result.uniqueId;
			}
		}

		public void Dispose()
		{
			if (tweakable.Manager != null && CheckValidTweakable() && tweakable != null)
			{
				Manager.UnregisterTweakable(tweakable);
			}
			tweakable = null;
		}

		protected bool CheckValidTweakable()
		{
			if (tweakable == null)
			{
				throw new AutoTweakableException("AutoTweakable has been disposed and can no longer be used.");
			}
			return true;
		}

		protected static bool CheckForManager()
		{
			if (Manager == null)
			{
				throw new AutoTweakableException("No manager has been set. Set a manager through AutoTweakableBase.Manager before creating auto tweakable instance.");
			}
			return true;
		}
	}
}
