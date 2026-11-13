using ClubPenguin.Props;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Game.PartyGames
{
	[RequireComponent(typeof(Prop))]
	public class ScavengerHuntFinderProp : MonoBehaviour
	{
		private const bool BULB_OFF = false;

		private const bool BULB_ON = true;

		public float BulbOnTime = 0.3f;

		public float BlinkSpeedMinimum = 0.1f;

		public float MaxDelayedSpeed = 100f;

		public GameObject BulbOff;

		public GameObject BulbOn;

		public GameObject FoundMarbleEffect;

		private Dictionary<int, GameObject> itemIdToItemGameObject;

		private bool isBulbBlinking;

		private EventDispatcher eventDispatcher;

		private EventChannel eventChannel;

		private Prop prop;

		private float elapsedTime;

		private void Awake()
		{
			prop = GetComponent<Prop>();
			setupListeners();
			swapBulbLights(false);
		}

		private void setupListeners()
		{
			eventDispatcher = Service.Get<EventDispatcher>();
			eventChannel = new EventChannel(eventDispatcher);
			eventChannel.AddListener<ScavengerHuntEvents.StartFinderBulb>(onStartFinderBulb);
		}

		private bool onStartFinderBulb(ScavengerHuntEvents.StartFinderBulb evt)
		{
			if (prop.OwnerId == evt.LocalPlayerId || prop.OwnerId == evt.OtherPlayerId)
			{
				itemIdToItemGameObject = evt.ItemIdToItemGameObject;
				elapsedTime = 0f;
				isBulbBlinking = true;
				eventChannel.AddListener<ScavengerHuntEvents.RemoveMarble>(onRemoveMarble);
				CoroutineRunner.Start(blinkBulb(), this, "blinkBulb");
			}
			else
			{
				eventChannel.RemoveAllListeners();
			}
			return false;
		}

		private bool onRemoveMarble(ScavengerHuntEvents.RemoveMarble evt)
		{
			Object.Instantiate(FoundMarbleEffect, evt.MarbleTransform.position, Quaternion.identity);
			return false;
		}

		private IEnumerator blinkBulb()
		{
			WaitForSeconds bulbBlinkTime = new WaitForSeconds(BulbOnTime);
			while (isBulbBlinking)
			{
				yield return null;
				float blinkSpeed = MaxDelayedSpeed / getClosestDistanceSquared();
				if (blinkSpeed < BlinkSpeedMinimum)
				{
					blinkSpeed = BlinkSpeedMinimum;
				}
				elapsedTime += Time.deltaTime * blinkSpeed;
				if (elapsedTime >= 1f)
				{
					elapsedTime = 0f;
					swapBulbLights(true);
					yield return bulbBlinkTime;
					swapBulbLights(false);
				}
			}
		}

		private float getClosestDistanceSquared()
		{
			float num = 100000f;
			foreach (KeyValuePair<int, GameObject> item in itemIdToItemGameObject)
			{
				float num2 = Vector3.SqrMagnitude(base.transform.position - item.Value.transform.position);
				if (num2 < num)
				{
					num = num2;
				}
			}
			return num;
		}

		private void swapBulbLights(bool isBulbOn)
		{
			BulbOn.SetActive(isBulbOn);
			BulbOff.SetActive(!isBulbOn);
			eventDispatcher.DispatchEvent(new ScavengerHuntEvents.FinderBulbBlink(isBulbOn));
		}

		private void OnDestroy()
		{
			if (eventChannel != null)
			{
				eventChannel.RemoveAllListeners();
			}
			CoroutineRunner.StopAllForOwner(this);
		}
	}
}
