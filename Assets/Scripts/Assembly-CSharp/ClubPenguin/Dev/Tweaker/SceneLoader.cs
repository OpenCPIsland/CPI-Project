using Disney.MobileNetwork;
using Tweaker.Core;

namespace ClubPenguin.Dev.Tweaker
{
	public static class SceneLoader
	{
		private static void LoadScene(string scene)
		{
			Service.Get<ZoneTransitionService>().LoadAsZoneOrScene(scene, "Loading");
		}

		[Invokable("SceneLoader.Boot")]
		private static void LoadBoot()
		{
			LoadScene("Boot");
		}

		[Invokable("SceneLoader.Home")]
		private static void LoadHome()
		{
			LoadScene("Home");
		}

		[Invokable("SceneLoader.Loading")]
		private static void LoadLoading()
		{
			LoadScene("Loading");
		}

		[Invokable("SceneLoader.Settings")]
		private static void LoadSettings()
		{
			LoadScene("Settings");
		}

		[Invokable("SceneLoader.Newsfeed")]
		private static void LoadNewsfeed()
		{
			LoadScene("Newsfeed");
		}

		[Invokable("SceneLoader.Boardwalk")]
		private static void LoadBoardwalk()
		{
			LoadScene("Boardwalk");
		}

		[Invokable("SceneLoader.Diving")]
		private static void LoadDiving()
		{
			LoadScene("Diving");
		}

		[Invokable("SceneLoader.Beach")]
		private static void LoadBeach()
		{
			LoadScene("Beach");
		}

		[Invokable("SceneLoader.MtBlizzard")]
		private static void LoadMtBlizzard()
		{
			LoadScene("MtBlizzard");
		}

		[Invokable("SceneLoader.Town")]
		private static void LoadTown()
		{
			LoadScene("Town");
		}

		[Invokable("SceneLoader.HerbertBase")]
		private static void LoadHerbertBase()
		{
			LoadScene("HerbertBase");
		}

		[Invokable("SceneLoader.MtBlizzardSummit")]
		private static void LoadMtBlizzardSummit()
		{
			LoadScene("MtBlizzardSummit");
		}

		[Invokable("SceneLoader.BoxDimension")]
		private static void LoadBoxDimension()
		{
			LoadScene("BoxDimension");
		}

		[Invokable("SceneLoader.ClothingDesigner")]
		private static void LoadClothingDesigner()
		{
			LoadScene("ClothingDesigner");
		}

		[Invokable("SceneLoader.CellPhoneApplet")]
		private static void LoadCellPhoneApplet()
		{
			LoadScene("CellPhoneApplet");
		}

		[Invokable("SceneLoader.BaseIgloo")]
		private static void LoadBaseIgloo()
		{
			LoadScene("BaseIgloo");
		}

		[Invokable("SceneLoader.DefaultIgloo")]
		private static void LoadDefaultIgloo()
		{
			LoadScene("DefaultIgloo");
		}

		[Invokable("SceneLoader.IslandIgloo")]
		private static void LoadIslandIgloo()
		{
			LoadScene("IslandIgloo");
		}

		[Invokable("SceneLoader.ForestIgloo")]
		private static void LoadForestIgloo()
		{
			LoadScene("ForestIgloo");
		}

		[Invokable("SceneLoader.Beach_SunSet2018_Decorations")]
		private static void LoadBeach_SunSet2018_Decorations()
		{
			LoadScene("Beach_SunSet2018_Decorations");
		}

		[Invokable("SceneLoader.Boardwalk_SunSet2018_Decorations")]
		private static void LoadBoardwalk_SunSet2018_Decorations()
		{
			LoadScene("Boardwalk_SunSet2018_Decorations");
		}

		[Invokable("SceneLoader.Diving_SunSet2018_Decorations")]
		private static void LoadDiving_SunSet2018_Decorations()
		{
			LoadScene("Diving_SunSet2018_Decorations");
		}

		[Invokable("SceneLoader.MtBlizzard_SunSet2018_Decorations")]
		private static void LoadMtBlizzard_SunSet2018_Decorations()
		{
			LoadScene("MtBlizzard_SunSet2018_Decorations");
		}

		[Invokable("SceneLoader.Town_SunSet2018_Decorations")]
		private static void LoadTown_SunSet2018_Decorations()
		{
			LoadScene("Town_SunSet2018_Decorations");
		}

		[Invokable("SceneLoader.Mg_bc_LoadScene")]
		private static void LoadMg_bc_LoadScene()
		{
			LoadScene("Mg_bc_LoadScene");
		}

		[Invokable("SceneLoader.Mg_bc_GameScene")]
		private static void LoadMg_bc_GameScene()
		{
			LoadScene("Mg_bc_GameScene");
		}

		[Invokable("SceneLoader.Mg_if_LoadScene")]
		private static void LoadMg_if_LoadScene()
		{
			LoadScene("Mg_if_LoadScene");
		}

		[Invokable("SceneLoader.Mg_if_GameScene")]
		private static void LoadMg_if_GameScene()
		{
			LoadScene("Mg_if_GameScene");
		}

		[Invokable("SceneLoader.Mg_pr_LoadScene")]
		private static void LoadMg_pr_LoadScene()
		{
			LoadScene("Mg_pr_LoadScene");
		}

		[Invokable("SceneLoader.Mg_pr_GameScene")]
		private static void LoadMg_pr_GameScene()
		{
			LoadScene("Mg_pr_GameScene");
		}

		[Invokable("SceneLoader.Mg_pt_GameScene")]
		private static void LoadMg_pt_GameScene()
		{
			LoadScene("Mg_pt_GameScene");
		}

		[Invokable("SceneLoader.Mg_pt_LoadScene")]
		private static void LoadMg_pt_LoadScene()
		{
			LoadScene("Mg_pt_LoadScene");
		}

		[Invokable("SceneLoader.Mg_ss_LoadScene")]
		private static void LoadMg_ss_LoadScene()
		{
			LoadScene("Mg_ss_LoadScene");
		}

		[Invokable("SceneLoader.Mg_ss_GameScene")]
		private static void LoadMg_ss_GameScene()
		{
			LoadScene("Mg_ss_GameScene");
		}

		[Invokable("SceneLoader.Mg_jr_LoadScene")]
		private static void LoadMg_jr_LoadScene()
		{
			LoadScene("Mg_jr_LoadScene");
		}

		[Invokable("SceneLoader.Mg_jr_GameScene")]
		private static void LoadMg_jr_GameScene()
		{
			LoadScene("Mg_jr_GameScene");
		}

		[Invokable("SceneLoader.ClassicMiniGames")]
		private static void LoadClassicMiniGames()
		{
			LoadScene("ClassicMiniGames");
		}

		[Invokable("SceneLoader.EndCredits")]
		private static void LoadEndCredits()
		{
			LoadScene("EndCredits");
		}
	}
}
