using System;
using System.Collections.Generic;

namespace ClubPenguin.Core
{
	public class ActionConfirmationService
	{
		public delegate void ActionConfirmationCallback();

		public delegate void FilterCallback(bool result);

		private Dictionary<string, IActionConfirmationFilter> filterIdToFilter;

		public ActionConfirmationService()
		{
			filterIdToFilter = new Dictionary<string, IActionConfirmationFilter>();
		}

		public void AddFilter(IActionConfirmationFilter filter)
		{
			if (!filterIdToFilter.ContainsKey(filter.GetFilterId()))
			{
				filterIdToFilter.Add(filter.GetFilterId(), filter);
			}
		}

		public void RemoveFilter(string filterId)
		{
			if (filterIdToFilter.ContainsKey(filterId))
			{
				filterIdToFilter.Remove(filterId);
			}
		}

		public void ConfirmAction(Type action, object payload, ActionConfirmationCallback successCallback, ActionConfirmationCallback failureCallback = null)
		{
			List<IActionConfirmationFilter> list = new List<IActionConfirmationFilter>();
			foreach (IActionConfirmationFilter value in filterIdToFilter.Values)
			{
				if (!value.IsActionValid(action, payload))
				{
					list.Add(value);
				}
			}
			confirmAction(action, true, list, successCallback, failureCallback);
		}

		private void confirmAction(Type action, bool isActionValid, List<IActionConfirmationFilter> filters, ActionConfirmationCallback successCallback, ActionConfirmationCallback failureCallback = null)
		{
			if (isActionValid && filters.Count > 0)
			{
				IActionConfirmationFilter actionConfirmationFilter = filters[0];
				filters.RemoveAt(0);
				actionConfirmationFilter.ShowConfirmation(action, delegate(bool result)
				{
					confirmAction(action, result, filters, successCallback, failureCallback);
				});
			}
			else if (isActionValid)
			{
				successCallback();
			}
			else if (failureCallback != null)
			{
				failureCallback();
			}
		}
	}
}
