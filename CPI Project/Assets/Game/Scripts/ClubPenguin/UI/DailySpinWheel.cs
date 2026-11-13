using ClubPenguin.CellPhone;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin.UI
{
	public class DailySpinWheel : MonoBehaviour
	{
		private enum SpinWheelState
		{
			Idle,
			StartSpin,
			SpinLoop,
			EndSpin
		}

		public enum WheelOverlayState
		{
			None,
			Center,
			Full
		}

		private const int CHEST_ITEM_INDEX = 0;

		private const int RESPIN_ITEM_INDEX = 1;

		private const string TICKER_IDLE_TRIGGER = "Idle";

		private const string TICKER_WINDUP_TRIGGER = "Windup";

		private const string TICKER_SPIN_TRIGGER = "Forward";

		public readonly PrefabContentKey ItemContentKey = new PrefabContentKey("Prefabs/DailySpinWheelItem");

		public GameObject ItemContainer;

		public float SpinLoopSpeed = 500f;

		public float StartSpinTime = 0.4f;

		public float SpinLoopTime = 3f;

		public float EndSpinMuliplier = 2f;

		public float EndRotationVariation = 10f;

		public Animator TickerAnimator;

		public Image WheelImage;

		public Image[] SliceImages;

		public System.Action OnSpinComplete;

		private int numItems = 8;

		private float currentSpinSpeed;

		private float spinLoopTimer;

		private float targetRotation;

		private int currentChestId;

		private List<DailySpinWheelItem> wheelItems;

		private SpinWheelState currentState;

		private WheelOverlayState currentOverlayState;

		private Hashtable startSpinTweenData;

		private Hashtable spinLoopTweenData;

		private Hashtable endSpinTweenData;

		private GameObject itemPrefab;

		private CellPhoneDailySpinActivityDefinition widgetData;

		private Color DIM_COLOR = new Color(0.7f, 0.7f, 0.7f);

		private void Start()
		{
			wheelItems = new List<DailySpinWheelItem>();
			initItweenSpinData();
		}

		private void Update()
		{
			if (currentSpinSpeed == 0f)
			{
				return;
			}
			base.transform.localRotation *= Quaternion.Euler(0f, 0f, currentSpinSpeed * Time.deltaTime);
			if (currentState == SpinWheelState.SpinLoop)
			{
				spinLoopTimer += Time.deltaTime;
				if (spinLoopTimer >= SpinLoopTime)
				{
					setState(SpinWheelState.EndSpin);
				}
			}
		}

		public void SetWidgetData(CellPhoneDailySpinActivityDefinition widgetData, int currentChestId)
		{
			this.widgetData = widgetData;
			this.currentChestId = currentChestId;
			numItems = widgetData.SpinRewards.Count + 2;
			Content.LoadAsync(onItemPrefabLoaded, ItemContentKey);
		}

		public void SetCurrentChestId(int id)
		{
			currentChestId = id;
			for (int i = 0; i < wheelItems.Count; i++)
			{
				wheelItems[i].SetCurrentChestId(id);
			}
		}

		private void onItemPrefabLoaded(string path, GameObject prefab)
		{
			itemPrefab = prefab;
			initItems();
		}

		private void initItems()
		{
			float num = 360f / (float)numItems;
			for (int i = 0; i < numItems; i++)
			{
				DailySpinWheelItem component = UnityEngine.Object.Instantiate(itemPrefab, ItemContainer.transform, false).GetComponent<DailySpinWheelItem>();
				component.transform.localRotation = Quaternion.Euler(0f, 0f, num * (float)i);
				wheelItems.Add(component);
			}
			CellPhoneDailySpinActivityDefinition.SpinReward spinReward = default(CellPhoneDailySpinActivityDefinition.SpinReward);
			spinReward.SpinOutcomeId = widgetData.ChestSpinOutcomeId;
			wheelItems[widgetData.ChestWheelPosition].SetReward(spinReward, widgetData, currentChestId);
			CellPhoneDailySpinActivityDefinition.SpinReward spinReward2 = default(CellPhoneDailySpinActivityDefinition.SpinReward);
			spinReward2.SpinOutcomeId = widgetData.RespinSpinOutcomeId;
			wheelItems[widgetData.RespinWheelPosition].SetReward(spinReward2, widgetData, currentChestId);
			for (int i = 0; i < widgetData.SpinRewards.Count; i++)
			{
				wheelItems[widgetData.SpinRewards[i].WheelPosition].SetReward(widgetData.SpinRewards[i], widgetData, currentChestId);
				wheelItems[widgetData.SpinRewards[i].WheelPosition].SetSliceImage(SliceImages[widgetData.SpinRewards[i].WheelPosition]);
			}
			SetOverlayState(currentOverlayState);
		}

		private void setState(SpinWheelState newState)
		{
			currentState = newState;
			switch (newState)
			{
			case SpinWheelState.Idle:
				currentSpinSpeed = 0f;
				break;
			case SpinWheelState.StartSpin:
				startSpinTweenData["from"] = base.transform.localRotation.eulerAngles.z;
				startSpinTweenData["to"] = base.transform.localRotation.eulerAngles.z + 70f;
				iTween.ValueTo(base.gameObject, startSpinTweenData);
				TickerAnimator.SetTrigger("Windup");
				break;
			case SpinWheelState.SpinLoop:
				spinLoopTimer = 0f;
				iTween.ValueTo(base.gameObject, spinLoopTweenData);
				TickerAnimator.SetTrigger("Forward");
				break;
			case SpinWheelState.EndSpin:
			{
				currentSpinSpeed = 0f;
				float z = base.transform.localRotation.eulerAngles.z;
				z += 340f;
				float num = z - targetRotation;
				float num2 = 1f + num / 360f * EndSpinMuliplier;
				endSpinTweenData["from"] = z;
				endSpinTweenData["to"] = targetRotation;
				endSpinTweenData["time"] = num2;
				iTween.ValueTo(base.gameObject, endSpinTweenData);
				break;
			}
			}
		}

		public void SetOverlayState(WheelOverlayState newState)
		{
			currentOverlayState = newState;
			switch (newState)
			{
			case WheelOverlayState.None:
				WheelImage.color = Color.white;
				SetSliceDimState(false, false);
				break;
			case WheelOverlayState.Center:
				WheelImage.color = Color.white;
				SetSliceDimState(true, false);
				break;
			case WheelOverlayState.Full:
				WheelImage.color = DIM_COLOR;
				SetSliceDimState(true, true);
				break;
			}
		}

		public void StartSpin(int spinOutcomeId)
		{
			int num = -1;
			if (spinOutcomeId == widgetData.ChestSpinOutcomeId)
			{
				num = widgetData.ChestWheelPosition;
			}
			else if (spinOutcomeId == widgetData.RespinSpinOutcomeId)
			{
				num = widgetData.RespinWheelPosition;
			}
			else
			{
				for (int i = 0; i < widgetData.SpinRewards.Count; i++)
				{
					if (widgetData.SpinRewards[i].SpinOutcomeId == spinOutcomeId)
					{
						num = widgetData.SpinRewards[i].WheelPosition;
					}
				}
			}
			if (num == -1)
			{
				Log.LogError(this, "Reward for spin outcome ID not found");
			}
			else
			{
				float num2 = 360f / (float)numItems;
				targetRotation = (float)num * num2 + UnityEngine.Random.Range(0f, EndRotationVariation) - EndRotationVariation * 0.5f;
				targetRotation += 180f;
				targetRotation *= -1f;
			}
			if (currentState == SpinWheelState.Idle)
			{
				setState(SpinWheelState.StartSpin);
			}
		}

		private void updateRotation(float value)
		{
			base.transform.localRotation = Quaternion.Euler(0f, 0f, value);
			if (currentState == SpinWheelState.EndSpin)
			{
				float speed = 1f - value / targetRotation;
				TickerAnimator.speed = speed;
			}
		}

		private void startSpinComplete()
		{
			setState(SpinWheelState.SpinLoop);
		}

		private void updateSpinSpeed(float value)
		{
			currentSpinSpeed = value;
		}

		private void endSpinComplete()
		{
			setState(SpinWheelState.Idle);
			if (OnSpinComplete != null)
			{
				OnSpinComplete();
			}
			TickerAnimator.speed = 1f;
			TickerAnimator.SetTrigger("Idle");
		}

		private void initItweenSpinData()
		{
			startSpinTweenData = iTween.Hash("name", "startSpin", "time", StartSpinTime, "easetype", iTween.EaseType.easeInOutCubic, "onupdate", "updateRotation", "onupdatetarget", base.gameObject, "oncomplete", "startSpinComplete", "oncompletetarget", base.gameObject);
			spinLoopTweenData = iTween.Hash("name", "spinLoop", "from", (0f - SpinLoopSpeed) * 0.6f, "to", 0f - SpinLoopSpeed, "time", 0.3f, "easetype", iTween.EaseType.easeInCubic, "onupdate", "updateSpinSpeed", "onupdatetarget", base.gameObject);
			endSpinTweenData = iTween.Hash("name", "endSpin", "easetype", iTween.EaseType.easeOutQuad, "onupdate", "updateRotation", "onupdatetarget", base.gameObject, "oncomplete", "endSpinComplete", "oncompletetarget", base.gameObject);
		}

		public void SetSliceDimState(bool dim, bool dimIcon)
		{
			if (wheelItems != null)
			{
				for (int i = 0; i < wheelItems.Count; i++)
				{
					wheelItems[i].SetDimState(dim, dimIcon);
				}
			}
		}
	}
}
