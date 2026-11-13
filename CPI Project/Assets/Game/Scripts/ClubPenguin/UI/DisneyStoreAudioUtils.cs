using UnityEngine;

namespace ClubPenguin.UI
{
	internal static class DisneyStoreAudioUtils
	{
		private const string SELECT_AUDIO = "SFX/UI/Store/ButtonSelect";

		private const string CLOSE_AUDIO = "SFX/UI/MainTray/ButtonClose";

		internal static void PlaySelect()
		{
			SoundUtils.PlayAudioEvent(SELECT_AUDIO);
		}

		internal static void PlayClose()
		{
			SoundUtils.PlayAudioEvent(CLOSE_AUDIO);
		}
	}
}
