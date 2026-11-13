using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Error
{
	public class ErrorsMap
	{
		public const string DEFAULT_ERROR_FILE_LOCATION = "Errors/errors.json";

		public System.Action EUpdateComplete;

		private Dictionary<string, string> errors = new Dictionary<string, string>();

		public string GetErrorMessage(string id)
		{
			if (errors.ContainsKey(id))
			{
				return errors[id];
			}
			return id;
		}

		public bool IsValidError(string key)
		{
			return errors.ContainsKey(key);
		}

		public void LoadErrorJson(string filePath)
		{
			CoroutineRunner.StartPersistent(loadErrors(filePath), this, "loadErrors");
		}

		private IEnumerator loadErrors(string filePath)
		{
			AssetRequest<TextAsset> request = Content.LoadAsync<TextAsset>(filePath);
			yield return request;
			string jsonText = request.Asset.text;
			UpdateErrorsFromJson(jsonText);
		}

		public void UpdateErrorsFromJson(string jsonText)
		{
			Dictionary<string, string> dictionary = Service.Get<JsonService>().Deserialize<Dictionary<string, string>>(jsonText);
			if (dictionary == null || dictionary.Count == 0)
			{
				Log.LogError(this, "Could not find any errors or tokens in specified text.");
				return;
			}
			foreach (KeyValuePair<string, string> item in dictionary)
			{
				if (errors.ContainsKey(item.Key))
				{
					errors[item.Key] = item.Value;
				}
				else
				{
					errors.Add(item.Key, item.Value);
				}
			}
			if (EUpdateComplete != null)
			{
				EUpdateComplete();
			}
		}

		private void printErrors()
		{
			foreach (KeyValuePair<string, string> error in errors)
			{
				Log.LogError(this, "Error - Key: " + error.Key + " Value: " + error.Value);
			}
		}
	}
}
