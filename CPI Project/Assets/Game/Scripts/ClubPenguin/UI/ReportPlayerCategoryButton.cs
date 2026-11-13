using Disney.Mix.SDK;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	[RequireComponent(typeof(Button))]
	public class ReportPlayerCategoryButton : MonoBehaviour
	{
		public Text ButtonText;

		public ReportUserReason Reason;

		protected ReportPlayerController reportPlayerController;

		protected virtual void Start()
		{
			reportPlayerController = GetComponentInParent<ReportPlayerController>();
			ButtonText.text = reportPlayerController.GetTextForReason(Reason);
			GetComponent<Button>().onClick.AddListener(onClick);
		}

		protected virtual void onClick()
		{
			reportPlayerController.OnCategoryButtonClicked(Reason);
		}
	}
}
