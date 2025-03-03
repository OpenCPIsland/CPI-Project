using ClubPenguin.Progression;
using Disney.MobileNetwork;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class LevelProgressDisplay : MonoBehaviour
	{
		public bool IsLocalPlayerOnly;

		public Text LevelText;

		public MascotLevelDisplay[] MascotLevels;

		private void Awake()
		{
			if (LevelText != null)
			{
				LevelText.text = "";
			}
		}

		private void Start()
		{
			if (IsLocalPlayerOnly)
			{
				SetUpProgression(true);
			}
		}

		public void SetUpProgression(bool isLocalPlayer, ProfileData profileData = null)
		{
			int num = 0;
			if (isLocalPlayer)
			{
				for (int i = 0; i < MascotLevels.Length; i++)
				{
					MascotLevels[i].SetUpMascotLevel(true, null, -1L);
				}
				num = Service.Get<ProgressionService>().Level;
			}
			else
			{
				for (int i = 0; i < MascotLevels.Length; i++)
				{
					num += MascotLevels[i].SetUpMascotLevel(false, profileData, -1L);
				}
			}
			LevelText.text = num.ToString();
		}
	}
}
