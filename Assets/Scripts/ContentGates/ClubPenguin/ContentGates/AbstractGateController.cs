using ClubPenguin.Analytics;
using ClubPenguin.Core;
using ClubPenguin.UI;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.ContentGates
{
	public abstract class AbstractGateController : IContentInterruption
	{
		private GameObject instantiatedPrefab;

		internal GatePrefabController gatePrefabController;

		private Transform parentTransform;

		private GateData gateData;

		protected abstract PrefabContentKey GateContentKey
		{
			get;
		}

		public event System.Action OnReturn;

		public event System.Action OnContinue;

		protected abstract void prepGate();

		protected abstract void onValueChanged(string strAnswer);

		protected abstract void onSubmitClicked();

		protected abstract string getAnalyticsContext();

		public void Show(Transform parentTransform = null)
		{
			CPDataEntityCollection cPDataEntityCollection = Service.Get<CPDataEntityCollection>();
			if (!cPDataEntityCollection.TryGetComponent(cPDataEntityCollection.LocalPlayerHandle, out gateData))
			{
				gateData = cPDataEntityCollection.AddComponent<GateData>(cPDataEntityCollection.LocalPlayerHandle);
				gateData.GateStatus = new Dictionary<Type, bool>();
			}
			bool value = false;
			gateData.GateStatus.TryGetValue(GetType(), out value);
			this.parentTransform = parentTransform;
			if (value)
			{
				Continue();
			}
			else
			{
				initializePopup(GateContentKey);
			}
		}

		private void initializePopup(PrefabContentKey popupKey)
		{
			CoroutineRunner.Start(loadPopupFromPrefab(popupKey), this, "loadPopupFromPrefab");
		}

		private IEnumerator loadPopupFromPrefab(PrefabContentKey popupKey)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync(popupKey);
			yield return assetRequest;
			instantiatedPrefab = UnityEngine.Object.Instantiate(assetRequest.Asset);
			if (parentTransform != null)
			{
				instantiatedPrefab.transform.SetParent(parentTransform, false);
			}
			gatePrefabController = instantiatedPrefab.GetComponent<GatePrefabController>();
			try
			{
				gatePrefabController.SubmitButton.onClick.AddListener(onSubmitClicked);
				gatePrefabController.CloseButton.onClick.AddListener(onCloseClicked);
				gatePrefabController.ErrorIcon.gameObject.SetActive(false);
				prepGate();
				gatePrefabController.AnswerInputField.onValueChanged.AddListener(onValueChanged);
				if ((bool)GameObject.Find("TopCanvas"))
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowTopPopup(instantiatedPrefab, false, true, "Accessibility.Popup.Title.AgeGate"));
				}
				else
				{
					Service.Get<EventDispatcher>().DispatchEvent(new PopupEvents.ShowPopup(instantiatedPrefab, false, true, "Accessibility.Popup.Title.AgeGate"));
				}
			}
			catch
			{
				Log.LogErrorFormatted(this, "Missing key elements from prefab for {0}\n CloseButton: {1}\n SubmitButton: {2}\n ErrorIcon: {3}", popupKey, gatePrefabController.CloseButton, gatePrefabController.SubmitButton, gatePrefabController.ErrorIcon);
				Return();
			}
			yield return null;
		}

		protected void handleGateSuccess()
		{
			if (gateData.GateStatus.ContainsKey(GetType()))
			{
				gateData.GateStatus[GetType()] = true;
			}
			else
			{
				gateData.GateStatus.Add(GetType(), true);
			}
			UnityEngine.Object.Destroy(instantiatedPrefab);
			Continue();
		}

		protected void handleGateFailure()
		{
			UnityEngine.Object.Destroy(instantiatedPrefab);
			Return();
		}

		private void onCloseClicked()
		{
			Service.Get<ICPSwrveService>().Action("game." + getAnalyticsContext(), "cancelled");
			UnityEngine.Object.Destroy(instantiatedPrefab);
			Return();
		}

		protected void Continue()
		{
			if (this.OnContinue != null)
			{
				this.OnContinue();
			}
		}

		protected void Return()
		{
			if (this.OnReturn != null)
			{
				this.OnReturn();
			}
		}
	}
}
