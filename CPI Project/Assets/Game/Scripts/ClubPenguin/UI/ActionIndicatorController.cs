using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class ActionIndicatorController
	{
		private Dictionary<DActionIndicator, GameObject> actionIndicators;

		public ActionIndicatorController()
		{
			actionIndicators = new Dictionary<DActionIndicator, GameObject>();
			Service.Get<EventDispatcher>().AddListener<ActionIndicatorEvents.AddActionIndicator>(actionIndicatorAdded);
			Service.Get<EventDispatcher>().AddListener<ActionIndicatorEvents.RemoveActionIndicator>(actionIndicatorRemoved);
		}

		private bool actionIndicatorAdded(ActionIndicatorEvents.AddActionIndicator evt)
		{
			CoroutineRunner.Start(loadActionIndicator(evt.IndicatorData), this, "loadActionIndicator");
			return false;
		}

		private IEnumerator loadActionIndicator(DActionIndicator indicatorData)
		{
			AssetRequest<GameObject> assetRequest = Content.LoadAsync<GameObject>(indicatorData.IndicatorContentKey);
			actionIndicators[indicatorData] = null;
			yield return assetRequest;
			if (indicatorData.TargetTransform != null && !indicatorData.TargetTransform.gameObject.IsDestroyed())
			{
				DActionIndicator indicatorByID = getIndicatorByID(indicatorData.IndicatorId);
				if (indicatorByID != null)
				{
					GameObject gameObject = Object.Instantiate(assetRequest.Asset);
					gameObject.transform.SetParent(indicatorData.TargetTransform);
					gameObject.transform.localPosition = indicatorData.TargetOffset;
					actionIndicators[indicatorData] = gameObject;
				}
			}
		}

		private bool actionIndicatorRemoved(ActionIndicatorEvents.RemoveActionIndicator evt)
		{
			DActionIndicator indicatorByID = getIndicatorByID(evt.IndicatorData.IndicatorId);
			if (indicatorByID != null && actionIndicators.ContainsKey(indicatorByID))
			{
				if (actionIndicators[indicatorByID] != null)
				{
					Object.Destroy(actionIndicators[indicatorByID]);
				}
				actionIndicators.Remove(indicatorByID);
			}
			return false;
		}

		private DActionIndicator getIndicatorByID(string indicatorID)
		{
			DActionIndicator result = null;
			foreach (DActionIndicator key in actionIndicators.Keys)
			{
				if (key.IndicatorId == indicatorID)
				{
					result = key;
					break;
				}
			}
			return result;
		}
	}
}
