using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class FriendsSearchStateHandler : MonoBehaviour
	{
		public Text TitleText;

		public GameObject FriendsResults;

		public GameObject SearchResult;

		public GameObject BackButton;

		public GameObject CloseButton;

		private string friendsResults = "#Friends Search#";

		private string searchResult = "#Search Result#";

		public void OnStateChanged(string state)
		{
			switch (state)
			{
			case "FriendsResults":
				TitleText.text = friendsResults;
				FriendsResults.SetActive(true);
				SearchResult.SetActive(false);
				CloseButton.SetActive(true);
				BackButton.SetActive(false);
				break;
			case "SearchResult":
				TitleText.text = searchResult;
				FriendsResults.SetActive(false);
				SearchResult.SetActive(true);
				CloseButton.SetActive(false);
				BackButton.SetActive(true);
				break;
			}
		}
	}
}
