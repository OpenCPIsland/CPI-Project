using Disney.Kelowna.Common;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Text))]
	public class InputFieldCaret : MonoBehaviour
	{
		public float BlinkRate;

		private bool isActive;

		private ICoroutine updateCaretVisibility;

		private Text caretText;

		private void Awake()
		{
			caretText = GetComponent<Text>();
		}

		private void OnEnable()
		{
			isActive = true;
			updateCaretVisibility = CoroutineRunner.Start(updateCaretVisibilityRoutine(), this, "updateCaretVisibilityRoutine");
		}

		private void OnDisable()
		{
			isActive = false;
		}

		private IEnumerator updateCaretVisibilityRoutine()
		{
			while (isActive)
			{
				caretText.enabled = true;
				yield return new WaitForSeconds(BlinkRate);
				caretText.enabled = false;
				yield return new WaitForSeconds(BlinkRate);
			}
		}

		private void OnDestroy()
		{
			if (updateCaretVisibility != null && !updateCaretVisibility.Disposed)
			{
				updateCaretVisibility.Cancel();
			}
		}
	}
}
