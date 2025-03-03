using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Error
{
	public class ShowErrorOnTransformCommand
	{
		private string errorId;

		private ErrorDirection errorDirection;

		private RectTransform targetRT;

		private RectTransform parentContainer;

		private float centerOffset;

		public ShowErrorOnTransformCommand(string errorId, ErrorDirection errorDirection, RectTransform targetRT, RectTransform parentContainer, float centerOffset = 100f)
		{
			this.errorId = errorId;
			this.errorDirection = errorDirection;
			this.targetRT = targetRT;
			this.parentContainer = parentContainer;
			this.centerOffset = centerOffset;
		}

		public void Execute()
		{
			CoroutineRunner.StartPersistent(loadErrorPopup(), this, "loadErrorPopup");
		}

		private IEnumerator loadErrorPopup()
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(ErrorPopup.DefaultContentKey);
			yield return assetRequest;
			GameObject asset = Object.Instantiate(assetRequest.Asset);
			asset.transform.SetParent(parentContainer, false);
			ErrorPopup errorPopup = asset.GetComponent<ErrorPopup>();
			string errorMessage = Service.Get<ErrorsMap>().GetErrorMessage(errorId);
			errorPopup.ShowErrorMessage(errorMessage, errorDirection, targetRT, centerOffset);
		}
	}
}
