using Disney.Kelowna.Common;
using Disney.MobileNetwork;
using System.Collections;
using UnityEngine;

namespace ClubPenguin.Error
{
	public class ShowErrorOnPositionCommand
	{
		private string errorId;

		private Vector2 position;

		private RectTransform parentContainer;

		public ShowErrorOnPositionCommand(string errorId, Vector2 position, RectTransform parentContainer)
		{
			this.errorId = errorId;
			this.position = position;
			this.parentContainer = parentContainer;
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
			errorPopup.ShowErrorMessage(errorMessage, position);
		}
	}
}
