using DisneyMobile.CoreUnitySystems;

namespace MinigameFramework
{
	internal class MinigameFactory
	{
		internal static string GetInitialScene(EMinigameTypes _type)
		{
			string result = "";
			switch (_type)
			{
			case EMinigameTypes.BEAN_COUNTER:
				result = "mg_bc_LoadScene";
				break;
			case EMinigameTypes.ICE_FISHING:
				result = "mg_if_LoadScene";
				break;
			case EMinigameTypes.PUFFLE_ROUNDUP:
				result = "mg_pr_LoadScene";
				break;
			case EMinigameTypes.PIZZATRON:
				result = "mg_pt_LoadScene";
				break;
			case EMinigameTypes.SMOOTHIE_SMASH:
				result = "mg_ss_LoadScene";
				break;
			case EMinigameTypes.JETPACK_REBOOT:
				result = "mg_jr_LoadScene";
				break;
			default:
				Logger.LogWarning(_type, "Unhandled EMinigameType trying to get initial scene.");
				break;
			}
			return result;
		}
	}
}
