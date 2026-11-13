using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using Tweaker.Core;
using Tweaker.Util;
using UnityEngine;

namespace ClubPenguin.ContentGates
{
	internal class ParentGateGenerator
	{
		private int a;

		private int b;

		private int c;

		private int d;

		private int answer;

		private int format;

		private string question;

		private string instructions;

		private static DevCacheableType<ParentGateType> parentGateVersion = new DevCacheableType<ParentGateType>("contentGates.ParentGateVersion", ParentGateType.numbers);

		[Tweakable("Session.ParentGateVersion")]
		private static ParentGateType _parentGateVersion
		{
			get
			{
				return parentGateVersion.Value;
			}
			set
			{
				parentGateVersion.Value = value;
			}
		}

		public static void GenerateExtraTweakerTypes()
		{
			ExtraTypesGenerator.GenerateExtraTypes<ParentGateType>();
		}

		public ParentGateGenerator()
		{
			Service.Get<ICommonGameSettings>().RegisterSetting(parentGateVersion, true);
		}

		public void resetQuestion()
		{
			switch (parentGateVersion.Value)
			{
			case ParentGateType.algebra:
				resetAlgebraQuestion();
				break;
			default:
				resetNumberQuestion();
				break;
			}
		}

		public void resetAlgebraQuestion()
		{
			a = Random.Range(2, 10);
			b = Random.Range(1, 12);
			c = Random.Range(5, 99);
			format = Random.Range(1, 6);
			instructions = Service.Get<Localizer>().GetTokenTranslation("GlobalUI.Settings.SettingsCanvas.Instructions");
			switch (format)
			{
			case 1:
				d = a * b + c;
				question = a + "x + " + c + " = " + d;
				answer = b;
				break;
			case 2:
				d = a * b - c;
				question = a + "x - " + c + " = " + d;
				answer = b;
				break;
			case 3:
				d = a * b + c;
				question = a + " * " + b + " + x  = " + d;
				answer = c;
				break;
			case 4:
				d = a * b - c;
				question = a + " * " + b + " - x  = " + d;
				answer = c;
				break;
			case 5:
				d = a * b + c;
				question = a + " * " + b + " + " + c + " = x";
				answer = d;
				break;
			default:
				d = a * b - c;
				question = a + " * " + b + " - " + c + " = x";
				answer = d;
				break;
			}
		}

		public void resetNumberQuestion()
		{
			Localizer localizer = Service.Get<Localizer>();
			a = Random.Range(1, 9);
			b = Random.Range(0, 9);
			c = Random.Range(0, 9);
			d = Random.Range(0, 9);
			string[] array = new string[10]
			{
				"GlobalUI.Gates.ParentGate.Zero",
				"GlobalUI.Gates.ParentGate.One",
				"GlobalUI.Gates.ParentGate.Two",
				"GlobalUI.Gates.ParentGate.Three",
				"GlobalUI.Gates.ParentGate.Four",
				"GlobalUI.Gates.ParentGate.Five",
				"GlobalUI.Gates.ParentGate.Six",
				"GlobalUI.Gates.ParentGate.Seven",
				"GlobalUI.Gates.ParentGate.Eight",
				"GlobalUI.Gates.ParentGate.Nine"
			};
			instructions = localizer.GetTokenTranslation("GlobalUI.Settings.ParentGateNumbers.Instructions");
			question = localizer.GetTokenTranslation(array[a]) + " " + localizer.GetTokenTranslation(array[b]) + " " + localizer.GetTokenTranslation(array[c]) + " " + localizer.GetTokenTranslation(array[d]);
			answer = a * 1000 + b * 100 + c * 10 + d;
		}

		public string getQuestion()
		{
			return question;
		}

		public string getInstructions()
		{
			return instructions;
		}

		public bool checkAnswer(int x)
		{
			return answer == x;
		}
	}
}
