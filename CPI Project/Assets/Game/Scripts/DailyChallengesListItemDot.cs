using ClubPenguin;
using Disney.MobileNetwork;
using System.Collections.Generic;

public class DailyChallengesListItemDot : DailyChallengesListItem
{
	public const string INVENTORY_SCENE = "ClothingDesigner";

	public string LoadingScene = "Loading";

	public void OnCatalogButtonPressed()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add(SceneTransitionService.SceneArgs.ShowCatalogOnEntry.ToString(), true);
		Service.Get<SceneTransitionService>().LoadScene("ClothingDesigner", LoadingScene, dictionary);
	}
}
