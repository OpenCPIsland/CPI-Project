using UnityEngine;

namespace DisneyMobile.CoreUnitySystems.UnitTest
{
	public class UnitTestRunner : MonoBehaviour
	{
		private void Start()
		{
			Run(false);
		}

		public void Run(bool failuesAreExceptions)
		{
			NUnitLiteUnityRunner.RunTests(failuesAreExceptions);
		}
	}
}
