using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using MinigameFramework;
using UnityEngine;

public class HarnessUIPanel : MonoBehaviour
{
	public AudioSource CloseButtonAudioSource;

	public void Awake()
	{
		ClassicMiniGames.PushBackButtonHandler(OnExitPressed);
		AudioSource[] componentsInChildren = GetComponentsInChildren<AudioSource>();
		foreach (AudioSource audioSource in componentsInChildren)
		{
			audioSource.volume = ClassicMiniGames.MainGameSFXVolume;
		}
		CloseButtonAudioSource.volume = ClassicMiniGames.MainGameSFXVolume;
	}

	private void OnDestroy()
	{
		ClassicMiniGames.RemoveBackButtonHandler(OnExitPressed);
	}

	public void OnBeanCounterPressed()
	{
		Debug.Log("Bean Counter!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.BEAN_COUNTER);
	}

	public void OnIceFishingPressed()
	{
		Debug.Log("Ice Fishing!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.ICE_FISHING);
	}

	public void OnPuffleRoundupPressed()
	{
		Debug.Log("Puffle Roundup!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.PUFFLE_ROUNDUP);
	}

	public void OnPizzatronPressed()
	{
		Debug.Log("Pizzatron!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.PIZZATRON);
	}

	public void OnSmoothieSmashPressed()
	{
		Debug.Log("Smoothie Smash!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.SMOOTHIE_SMASH);
	}

	public void OnJetpackRebootPressed()
	{
		Debug.Log("Jetpack Boost!");
		MinigameManager.Instance.ShowMinigame(EMinigameTypes.JETPACK_REBOOT);
	}

	public void OnExitPressed()
	{
		Debug.Log("Mini Games - OnExitPressed");
		ClassicMiniGames.RotateToPortrait(delegate
		{
			Debug.Log("Mini Games - Finsihed waiting for screen rotation... exiting mini games now");
			MinigameManager.Instance.Shutdown();
			InputManager.Instance.Shutdown();
			RealTime.Shutdown();
			ClassicMiniGames.ReturnToWorld();
		});
	}
}
