using ClubPenguin;
using ClubPenguin.Actions;
using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.MobileNetwork;
using Fabric;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperTrampoline : MonoBehaviour
{
	[Header("Springs")]
	[Tooltip("List of all impulse objects (springs, trampolines, etc.)")]
	public List<ImpulseAction> impulseObjects = new List<ImpulseAction>();

	[Tooltip("List of impulse power per level")]
	public float[] impulsePowerPerLevel;

	[Header("Meter Face")]
	public GameObject[] LitSections;

	[Header("Meter Arrow")]
	[Tooltip("Gameobject for meter arrow")]
	public GameObject arrowObj;

	[Tooltip("Rotations for meter arrow (by level)")]
	public int[] arrowPositions;

	[Tooltip("Tween for meter arrow changes")]
	public iTween.EaseType arrowTween = iTween.EaseType.easeOutBack;

	[Tooltip("Time for arrow tween changes")]
	public float arrowTweenTime = 1f;

	[Tooltip("Amount to change meter by per penguin")]
	public float teamworkDelta = 1f;

	[Header("Particles")]
	[Tooltip("Particles shown when meter increases")]
	public GameObject increaseParticles;

	[Tooltip("Anchors for particles when meter increases")]
	public GameObject[] increaseAnchors;

	[Header("Localization")]
	[LocalizationToken]
	public string levelToken = "";

	public TextMesh textTitle;

	public TextMesh textLevel;

	[Header("Audio")]
	public string[] AudioIncreaseLevel;

	public string AudioPowerOverdrive;

	public float PauseBeforeOverdrive = 2f;

	public string AudioDecreaseLevel;

	public string AudioPowerDown;

	public GameObject soundAnchorObj;

	private float teamworkLevel;

	private float[] impulseOriginalMagnitude;

	private CPDataEntityCollection dataEntityCollection;

	private HashSet<RemotePlayerData> playerRemovedListeners;

	private bool showIncreaseEffects = false;

	private bool isOverdriveActive = false;

	private Animator anim;

	private int hashOverdriveIsOn;

	private void Awake()
	{
		dataEntityCollection = Service.Get<CPDataEntityCollection>();
	}

	private void Start()
	{
		playerRemovedListeners = new HashSet<RemotePlayerData>();
		if (increaseParticles != null && increaseAnchors.Length > 0)
		{
			showIncreaseEffects = true;
		}
		if (textTitle != null)
		{
			if (string.IsNullOrEmpty(levelToken))
			{
				throw new Exception("SuperTrampoline - token for Level text must be set");
			}
			textTitle.text = Service.Get<Localizer>().GetTokenTranslation(levelToken).ToUpper();
		}
		anim = base.gameObject.GetComponent<Animator>();
		if (anim == null)
		{
			throw new Exception("SuperTrampoline - no animator found");
		}
		hashOverdriveIsOn = Animator.StringToHash("OverdriveIsOn");
		int count = impulseObjects.Count;
		impulseOriginalMagnitude = new float[count];
		for (int i = 0; i < count; i++)
		{
			impulseOriginalMagnitude[i] = impulseObjects[i].Magnitude;
		}
		teamworkLevel = 0f;
		setMeter(0f);
	}

	private void OnDestroy()
	{
		foreach (RemotePlayerData playerRemovedListener in playerRemovedListeners)
		{
			playerRemovedListener.PlayerRemoved -= onPlayerRemoved;
		}
		playerRemovedListeners.Clear();
	}

	private bool addRemotePlayer(DataEntityHandle remotePlayerHandle)
	{
		RemotePlayerData component = dataEntityCollection.GetComponent<RemotePlayerData>(remotePlayerHandle);
		component.PlayerRemoved += onPlayerRemoved;
		return playerRemovedListeners.Add(component);
	}

	private bool addRemotePlayer(RemotePlayerData remotePlayerData)
	{
		remotePlayerData.PlayerRemoved += onPlayerRemoved;
		return playerRemovedListeners.Add(remotePlayerData);
	}

	private bool deleteRemotePlayer(DataEntityHandle remotePlayerHandle)
	{
		RemotePlayerData component = dataEntityCollection.GetComponent<RemotePlayerData>(remotePlayerHandle);
		component.PlayerRemoved -= onPlayerRemoved;
		return playerRemovedListeners.Remove(component);
	}

	private bool deleteRemotePlayer(RemotePlayerData remotePlayerData)
	{
		remotePlayerData.PlayerRemoved -= onPlayerRemoved;
		return playerRemovedListeners.Remove(remotePlayerData);
	}

	private void updateTeamworkLevel(float delta)
	{
		teamworkLevel += delta;
		teamworkLevel = Mathf.Clamp(teamworkLevel, 0f, impulsePowerPerLevel.Length - 1);
		setMeter(teamworkLevel, delta);
		int count = impulseObjects.Count;
		for (int i = 0; i < count; i++)
		{
			impulseObjects[i].Magnitude = impulseOriginalMagnitude[i] + impulsePowerPerLevel[(int)teamworkLevel];
		}
	}

	private void setMeter(float newLevel, float delta = 0f)
	{
		int num = Mathf.Clamp((int)newLevel, 0, impulsePowerPerLevel.Length - 1);
		textLevel.text = string.Format("{0}", num);
		if (delta > 0f)
		{
			if (showIncreaseEffects && delta > 0f)
			{
				UnityEngine.Object.Instantiate(increaseParticles, increaseAnchors[UnityEngine.Random.Range(0, increaseAnchors.Length)].transform.position, increaseParticles.transform.rotation);
			}
			if (!isOverdriveActive && num == impulsePowerPerLevel.Length - 1)
			{
				if (anim != null)
				{
					isOverdriveActive = true;
					CoroutineRunner.Start(showOverdriveEffects(PauseBeforeOverdrive), this, "showOverdriveEffects");
				}
			}
			else if (AudioIncreaseLevel.Length > 0 && num < AudioIncreaseLevel.Length)
			{
				playAudioEvent(AudioIncreaseLevel[num]);
			}
		}
		else if (delta < 0f)
		{
			if (isOverdriveActive || anim.GetBool(hashOverdriveIsOn))
			{
				stopOverdriveEffects();
				isOverdriveActive = false;
			}
			if (num == 0)
			{
				playAudioEvent(AudioPowerDown);
			}
			else
			{
				playAudioEvent(AudioDecreaseLevel);
			}
		}
		if (arrowObj != null)
		{
			iTween.RotateTo(arrowObj, iTween.Hash("z", (float)arrowPositions[num], "easeType", arrowTween, "time", arrowTweenTime, "islocal", true, "oncomplete", "onGaugeArrowComplete", "oncompleteparams", newLevel, "oncompletetarget", base.gameObject));
		}
		if (LitSections.Length > 0)
		{
			int num2 = Mathf.Clamp(num, 0, LitSections.Length);
			for (int i = 0; i < num2; i++)
			{
				LitSections[i].SetActive(true);
			}
			int num3 = LitSections.Length;
			for (int i = num2; i < num3; i++)
			{
				LitSections[i].SetActive(false);
			}
		}
	}

	private IEnumerator showOverdriveEffects(float waitTime)
	{
		isOverdriveActive = true;
		playAudioEvent(AudioPowerOverdrive);
		yield return new WaitForSeconds(waitTime);
		anim.SetBool(hashOverdriveIsOn, isOverdriveActive);
	}

	private void stopOverdriveEffects()
	{
		anim.SetBool(hashOverdriveIsOn, false);
		stopAudioEvent(AudioPowerOverdrive);
	}

	private void onGaugeArrowComplete(int newLevel)
	{
	}

	public void OnRemoteTriggerEnter(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			updateTeamworkLevel(teamworkDelta);
		}
		else if (other.gameObject.CompareTag("RemotePlayer"))
		{
			AvatarDataHandle component = other.gameObject.GetComponent<AvatarDataHandle>();
			if (addRemotePlayer(component.Handle))
			{
				updateTeamworkLevel(teamworkDelta);
			}
		}
	}

	public void OnRemoteTriggerExit(Collider other)
	{
		if (other.gameObject.CompareTag("Player"))
		{
			updateTeamworkLevel(0f - teamworkDelta);
		}
		else if (other.gameObject.CompareTag("RemotePlayer"))
		{
			AvatarDataHandle component = other.gameObject.GetComponent<AvatarDataHandle>();
			if (deleteRemotePlayer(component.Handle))
			{
				updateTeamworkLevel(0f - teamworkDelta);
			}
		}
	}

	private void onPlayerRemoved(RemotePlayerData remotePlayerData)
	{
		if (deleteRemotePlayer(remotePlayerData))
		{
			updateTeamworkLevel(0f - teamworkDelta);
		}
	}

	private void playAudioEvent(string audioEventName)
	{
		if (!string.IsNullOrEmpty(audioEventName))
		{
			if (soundAnchorObj != null)
			{
				EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound, soundAnchorObj);
			}
			else
			{
				EventManager.Instance.PostEvent(audioEventName, EventAction.PlaySound);
			}
		}
	}

	private void stopAudioEvent(string audioEventName)
	{
		if (!string.IsNullOrEmpty(audioEventName))
		{
			if (soundAnchorObj != null)
			{
				EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound, soundAnchorObj);
			}
			else
			{
				EventManager.Instance.PostEvent(audioEventName, EventAction.StopSound);
			}
		}
	}
}
