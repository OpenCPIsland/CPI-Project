using System.Reflection;
using Tweaker.AssemblyScanner;
using Tweaker.Core;
using UnityEngine;

namespace Tweaker.UI.Testbed
{
	public class TweakerConsoleTestbed : MonoBehaviour
	{
		public TweakerConsoleController ConsolePrefab;

		private TweakerConsoleController console;

		private ITweakerLogger logger;

		private Tweaker tweaker;

		private void Awake()
		{
			global::Tweaker.Core.LogManager.Set(new LogManager());
			logger = global::Tweaker.Core.LogManager.GetCurrentClassLogger();
			logger.Info("Logger initialized");
			tweaker = new Tweaker();
			Scanner scanner = new Scanner();
			ScanOptions scanOptions = new ScanOptions();
			scanOptions.Assemblies.ScannableRefs = new Assembly[1]
			{
				typeof(TweakerConsoleTestbed).Assembly
			};
			TweakerOptions tweakerOptions = new TweakerOptions();
			tweakerOptions.Flags = (TweakerOptionFlags.ScanForInvokables | TweakerOptionFlags.ScanForTweakables | TweakerOptionFlags.ScanForWatchables | TweakerOptionFlags.DoNotAutoScan | TweakerOptionFlags.IncludeTests);
			tweaker.Init(tweakerOptions, scanner);
			TweakerSerializer serializer = new TweakerSerializer(tweaker.Scanner);
			tweaker.Scanner.Scan(scanOptions);
			console = Object.Instantiate(ConsolePrefab);
			logger.Info("console instatiated: " + console);
			console.GetComponent<RectTransform>().SetParent(GetComponent<RectTransform>(), false);
			logger.Info("console parented to testbed canvas");
			console.Init(tweaker, serializer);
		}

		private void Start()
		{
			console.Refresh();
		}
	}
}
