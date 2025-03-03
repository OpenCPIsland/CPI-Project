using System.Collections.Generic;
using System.Reflection;
using Tweaker.AssemblyScanner;
using Tweaker.Core;

namespace Tweaker
{
	public class Tweaker
	{
		public IInvokableManager Invokables
		{
			get;
			private set;
		}

		public ITweakableManager Tweakables
		{
			get;
			private set;
		}

		public IWatchableManager Watchables
		{
			get;
			private set;
		}

		public TweakerOptions Options
		{
			get;
			private set;
		}

		public IScanner Scanner
		{
			get;
			private set;
		}

		public void Init(TweakerOptions options = null, IScanner scanner = null)
		{
			if (options == null)
			{
				options = new TweakerOptions();
			}
			Options = options;
			TweakerOptionFlags flags = Options.Flags;
			Scanner = ((scanner != null) ? scanner : global::Tweaker.AssemblyScanner.Scanner.Global);
			if (flags == TweakerOptionFlags.None || (flags & TweakerOptionFlags.Default) != 0)
			{
				Options.Flags = TweakerOptions.GetDefaultFlags();
			}
			CreateManagers();
			if ((flags & TweakerOptionFlags.DoNotAutoScan) == 0)
			{
				PerformScan();
			}
			Scanner.ScanInstance(this);
		}

		private void CreateManagers()
		{
			TweakerOptionFlags flags = Options.Flags;
			if ((flags & TweakerOptionFlags.ScanForInvokables) != 0)
			{
				Invokables = new InvokableManager(Scanner, Options);
			}
			else
			{
				Invokables = new InvokableManager(null, Options);
			}
			if ((flags & TweakerOptionFlags.ScanForTweakables) != 0)
			{
				Tweakables = new TweakableManager(Scanner, Options);
			}
			else
			{
				Tweakables = new TweakableManager(null, Options);
			}
			if ((flags & TweakerOptionFlags.ScanForWatchables) != 0)
			{
				Watchables = new WatchableManager(Scanner, Options);
			}
			else
			{
				Watchables = new WatchableManager();
			}
		}

		private void PerformScan()
		{
			TweakerOptionFlags flags = Options.Flags;
			if ((flags & TweakerOptionFlags.ScanInEverything) != 0)
			{
				ScanEverything();
				return;
			}
			if ((flags & TweakerOptionFlags.ScanInNonSystemAssemblies) != 0)
			{
				ScanNonSystemAssemblies();
				return;
			}
			List<Assembly> list = new List<Assembly>();
			if ((flags & TweakerOptionFlags.ScanInExecutingAssembly) != 0)
			{
				list.Add(Assembly.GetCallingAssembly());
			}
			if ((flags & TweakerOptionFlags.ScanInEntryAssembly) != 0)
			{
				list.Add(Assembly.GetEntryAssembly());
			}
			ScanOptions scanOptions = new ScanOptions();
			scanOptions.Assemblies.ScannableRefs = list.ToArray();
			ScanWithOptions(scanOptions);
		}

		private void ScanWithOptions(ScanOptions options)
		{
			Scanner.Scan(options);
		}

		private void ScanEverything()
		{
			ScanWithOptions(null);
		}

		private void ScanEntryAssembly()
		{
			ScanOptions scanOptions = new ScanOptions();
			scanOptions.Assemblies.ScannableRefs = new Assembly[1]
			{
				Assembly.GetEntryAssembly()
			};
			ScanWithOptions(scanOptions);
		}

		private void ScanNonSystemAssemblies()
		{
			ScanOptions scanOptions = new ScanOptions();
			scanOptions.Assemblies.NameRegex = "^(?!(System\\.)|System$|mscorlib$|Microsoft\\.|vshost|Unity|Accessibility|Mono\\.).+";
			ScanWithOptions(scanOptions);
		}
	}
}
