using ClubPenguin.Core;
using ClubPenguin.Net;
using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ClubPenguin.Breadcrumbs
{
	public class NotificationBreadcrumbController
	{
		public Action<string, int> OnBreadcrumbAdded;

		public Action<string, int> OnBreadcrumbRemoved;

		public Action<string> OnBreadcrumbReset;

		private Dictionary<string, int> breadcrumbs;

		private List<Breadcrumb> persistentBreadcrumbs;

		private List<Breadcrumb> addSyncQueue;

		private ICoroutine addSyncCoroutine;

		public Dictionary<FeatureLabelBreadcrumbDefinition.DictionaryKey, FeatureLabelBreadcrumbDefinition> AvailableFeatureLabelBreadcrumbs
		{
			get;
			private set;
		}

		public NotificationBreadcrumbController()
		{
			breadcrumbs = new Dictionary<string, int>();
			persistentBreadcrumbs = new List<Breadcrumb>();
			addSyncQueue = new List<Breadcrumb>();
			AvailableFeatureLabelBreadcrumbs = new Dictionary<FeatureLabelBreadcrumbDefinition.DictionaryKey, FeatureLabelBreadcrumbDefinition>();
			Service.Get<EventDispatcher>().AddListener<PlayerStateServiceEvents.LocalPlayerDataReceived>(onLocalPlayerDataReceived);
			Service.Get<EventDispatcher>().AddListener<BreadcrumbServiceEvents.BreadcrumbIdsReceived>(onBreadcrumbsReceived);
		}

		public void SetAvailableFeatureLabels(DateTime presentTime)
		{
			FeatureLabelBreadcrumbDefinition[] array = Service.Get<IGameData>().Get<FeatureLabelBreadcrumbDefinition[]>();
			foreach (FeatureLabelBreadcrumbDefinition featureLabelBreadcrumbDefinition in array)
			{
				if (DateTimeUtils.IsBefore(presentTime, featureLabelBreadcrumbDefinition.EndDate))
				{
					AvailableFeatureLabelBreadcrumbs.Add(new FeatureLabelBreadcrumbDefinition.DictionaryKey(featureLabelBreadcrumbDefinition.TypeId, featureLabelBreadcrumbDefinition.Type), featureLabelBreadcrumbDefinition);
				}
			}
		}

		public void ClearAllBreadcrumbs()
		{
			breadcrumbs.Clear();
			persistentBreadcrumbs.Clear();
		}

		private bool onBreadcrumbsReceived(BreadcrumbServiceEvents.BreadcrumbIdsReceived evt)
		{
			setAllBreadcrumbs(evt.BreadcrumbIds);
			return false;
		}

		private bool onLocalPlayerDataReceived(PlayerStateServiceEvents.LocalPlayerDataReceived evt)
		{
			setAllBreadcrumbs(evt.Data.breadcrumbs.breadcrumbs);
			return false;
		}

		private void setAllBreadcrumbs(List<Breadcrumb> crumbs)
		{
			persistentBreadcrumbs = new List<Breadcrumb>();
			if (crumbs == null)
			{
				return;
			}
			for (int i = 0; i < crumbs.Count; i++)
			{
				if (!string.IsNullOrEmpty(crumbs[i].id))
				{
					int breadcrumbType = crumbs[i].breadcrumbType;
					AddBreadcrumb(PersistentBreadcrumbTypeDefinition.ToStaticBreadcrumb(breadcrumbType, crumbs[i].id));
					persistentBreadcrumbs.Add(crumbs[i]);
				}
			}
		}

		public int GetPersistentBreadcrumbCount(PersistentBreadcrumbTypeDefinitionKey type, string id)
		{
			return GetBreadcrumbCount(PersistentBreadcrumbTypeDefinition.ToStaticBreadcrumb(type.Id, id));
		}

		public int GetBreadcrumbCount(string breadcrumbId)
		{
			string[] array = new string[breadcrumbs.Keys.Count];
			breadcrumbs.Keys.CopyTo(array, 0);
			int num = 0;
			foreach (string text in array)
			{
				if (text.IndexOf('/') == -1)
				{
					if (breadcrumbs.ContainsKey(breadcrumbId) && breadcrumbs[breadcrumbId] > num)
					{
						num = breadcrumbs[breadcrumbId];
					}
					continue;
				}
				string[] array2 = text.Split('/');
				for (int j = 0; j < array2.Length; j++)
				{
					if (array2[j] == breadcrumbId && breadcrumbs[text] > num)
					{
						num = breadcrumbs[text];
					}
				}
			}
			return num;
		}

		public void AddBreadcrumb(StaticBreadcrumbDefinitionKey breadcrumbKey, int count = 1)
		{
			AddBreadcrumb(breadcrumbKey.Id, count);
		}

		public void AddBreadcrumb(string breadcrumbId, int count = 1)
		{
			if (!breadcrumbs.ContainsKey(breadcrumbId))
			{
				breadcrumbs.Add(breadcrumbId, count);
			}
			else
			{
				breadcrumbs[breadcrumbId] += count;
			}
			if (OnBreadcrumbAdded != null)
			{
				OnBreadcrumbAdded(breadcrumbId, breadcrumbs[breadcrumbId]);
			}
		}

		public void AddPersistentBreadcrumb(PersistentBreadcrumbTypeDefinitionKey type, string id)
		{
			Breadcrumb item = new Breadcrumb(id, type.Id);
			persistentBreadcrumbs.Add(item);
			addSyncQueue.Add(item);
			if (addSyncCoroutine == null || addSyncCoroutine.Disposed)
			{
				addSyncCoroutine = CoroutineRunner.Start(syncAddedBreadcumbs(), this, "Batching breadcrumb adds");
			}
			AddBreadcrumb(PersistentBreadcrumbTypeDefinition.ToStaticBreadcrumb(type.Id, id));
		}

		private IEnumerator syncAddedBreadcumbs()
		{
			yield return null;
			if (addSyncQueue.Count > 0)
			{
				Service.Get<INetworkServicesManager>().BreadcrumbService.AddBreadcrumbIds(addSyncQueue);
			}
			addSyncQueue.Clear();
		}

		public bool RemoveBreadcrumb(StaticBreadcrumbDefinitionKey breadcrumbKey, int count = 1)
		{
			return RemoveBreadcrumb(breadcrumbKey.Id, count);
		}

		public bool RemoveBreadcrumb(string breadcrumbId, int count = 1)
		{
			bool flag = false;
			if (breadcrumbs.ContainsKey(breadcrumbId))
			{
				flag = true;
				breadcrumbs[breadcrumbId] -= count;
				if (breadcrumbs[breadcrumbId] < 0)
				{
					breadcrumbs[breadcrumbId] = 0;
				}
				if (OnBreadcrumbRemoved != null)
				{
					OnBreadcrumbRemoved(breadcrumbId, breadcrumbs[breadcrumbId]);
				}
				if (breadcrumbs[breadcrumbId] == 0)
				{
					breadcrumbs.Remove(breadcrumbId);
				}
			}
			else
			{
				string[] array = new string[breadcrumbs.Keys.Count];
				breadcrumbs.Keys.CopyTo(array, 0);
				foreach (string text in array)
				{
					int num = text.LastIndexOf('/');
					if (num == -1)
					{
						continue;
					}
					string a = text.Substring(num);
					if (a == breadcrumbId)
					{
						breadcrumbs[text] -= count;
						if (OnBreadcrumbRemoved != null)
						{
							OnBreadcrumbRemoved(text, breadcrumbs[text]);
						}
						if (breadcrumbs[text] == 0)
						{
							breadcrumbs.Remove(breadcrumbId);
						}
						flag = true;
						break;
					}
				}
			}
			if (!flag)
			{
			}
			return flag;
		}

		public void RemovePersistentBreadcrumb(PersistentBreadcrumbTypeDefinitionKey type, string id)
		{
			string text = PersistentBreadcrumbTypeDefinition.ToStaticBreadcrumb(type.Id, id);
			if (!breadcrumbs.ContainsKey(text))
			{
				return;
			}
			int num = 0;
			while (true)
			{
				if (num < persistentBreadcrumbs.Count)
				{
					if (persistentBreadcrumbs[num].breadcrumbType == type.Id && persistentBreadcrumbs[num].id.Equals(id))
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			List<Breadcrumb> list = new List<Breadcrumb>();
			list.Add(persistentBreadcrumbs[num]);
			persistentBreadcrumbs.Remove(persistentBreadcrumbs[num]);
			Service.Get<INetworkServicesManager>().BreadcrumbService.RemoveBreadcrumbIds(list);
			RemoveBreadcrumb(text);
		}

		public void ResetBreadcrumbs(StaticBreadcrumbDefinitionKey breadcrumbKey)
		{
			ResetBreadcrumbs(breadcrumbKey.Id);
		}

		public void ResetBreadcrumbs(string breadcrumbId)
		{
			if (breadcrumbs.ContainsKey(breadcrumbId))
			{
				breadcrumbs[breadcrumbId] = 0;
				if (OnBreadcrumbReset != null)
				{
					OnBreadcrumbReset(breadcrumbId);
				}
				breadcrumbs.Remove(breadcrumbId);
				return;
			}
			string[] array = new string[breadcrumbs.Keys.Count];
			breadcrumbs.Keys.CopyTo(array, 0);
			int num = 0;
			string text;
			while (true)
			{
				if (num >= array.Length)
				{
					return;
				}
				text = array[num];
				int num2 = text.LastIndexOf('/');
				if (num2 != -1)
				{
					string a = text.Substring(num2 + 1);
					if (a == breadcrumbId)
					{
						break;
					}
				}
				num++;
			}
			breadcrumbs[text] = 0;
			if (OnBreadcrumbReset != null)
			{
				OnBreadcrumbReset(text);
			}
			breadcrumbs.Remove(text);
		}
	}
}
