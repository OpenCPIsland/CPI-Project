using ClubPenguin.Core;
using ClubPenguin.Net;
using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.UI
{
	public class OtherPlayerDetailsRequestBatcher
	{
		private const float timeOutSeconds = 0.5f;

		private bool isRequestTimerStarted;

		private List<DataEntityHandle> requestEntities = new List<DataEntityHandle>();

		private bool hasRequestedFirstBatch;

		public void RequestOtherPlayerDetails(DataEntityHandle handle)
		{
			prepareOtherPlayerDetailsRequest(handle);
		}

		public void ResetFirstRequestStatus()
		{
			hasRequestedFirstBatch = false;
		}

		private void prepareOtherPlayerDetailsRequest(DataEntityHandle handle)
		{
			requestEntities.Add(handle);
			if (!isRequestTimerStarted)
			{
				isRequestTimerStarted = true;
				CoroutineRunner.Start(waitForOtherPlayerDetailsRequests(), this, "waitForOtherPlayerDetailsRequests");
			}
		}

		private IEnumerator waitForOtherPlayerDetailsRequests()
		{
			if (!hasRequestedFirstBatch)
			{
				hasRequestedFirstBatch = true;
				yield return new WaitForEndOfFrame();
			}
			else
			{
				yield return new WaitForSeconds(0.5f);
			}
			sendRequests(requestEntities);
			requestEntities.Clear();
			isRequestTimerStarted = false;
		}

		private void sendRequests(List<DataEntityHandle> handles)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			for (int i = 0; i < handles.Count; i++)
			{
				string swid = getSwid(handles[i]);
				if (!string.IsNullOrEmpty(swid))
				{
					list.Add(swid);
					continue;
				}
				string displayName = getDisplayName(handles[i]);
				if (!string.IsNullOrEmpty(displayName))
				{
					list2.Add(displayName);
				}
				else
				{
					Log.LogError(this, "Entity did not have a display name or a swid");
				}
			}
			if (list.Count > 0)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataBySwids(list);
			}
			if (list2.Count > 0)
			{
				Service.Get<INetworkServicesManager>().PlayerStateService.GetOtherPlayerDataByDisplayNames(list2);
			}
		}

		private string getSwid(DataEntityHandle handle)
		{
			SwidData component = Service.Get<CPDataEntityCollection>().GetComponent<SwidData>(handle);
			if (component != null && !string.IsNullOrEmpty(component.Swid))
			{
				return component.Swid;
			}
			return null;
		}

		private string getDisplayName(DataEntityHandle handle)
		{
			DisplayNameData component = Service.Get<CPDataEntityCollection>().GetComponent<DisplayNameData>(handle);
			if (component != null && !string.IsNullOrEmpty(component.DisplayName))
			{
				return component.DisplayName;
			}
			return null;
		}
	}
}
