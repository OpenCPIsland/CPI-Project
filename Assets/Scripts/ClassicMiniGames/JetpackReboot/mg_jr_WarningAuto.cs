using System.Collections;

namespace JetpackReboot
{
	public class mg_jr_WarningAuto : mg_jr_Warning
	{
		protected int NumberOfSignsToUse
		{
			get;
			set;
		}

		protected override void Awake()
		{
			base.Awake();
			NumberOfSignsToUse = 1;
		}

		private void OnEnable()
		{
			StartCoroutine(WaitOneFrameAndWarn());
		}

		private IEnumerator WaitOneFrameAndWarn()
		{
			yield return 0;
			if (base.gameObject.activeInHierarchy)
			{
				ActivateWarning(NumberOfSignsToUse);
			}
		}
	}
}
