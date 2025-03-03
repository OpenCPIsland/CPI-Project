using System;
using System.Collections.Generic;
using UnityEngine;

public struct UniWebViewMessage
{
	public string rawMessage
	{
		get;
		private set;
	}

	public string scheme
	{
		get;
		private set;
	}

	public string path
	{
		get;
		private set;
	}

	public Dictionary<string, string> args
	{
		get;
		private set;
	}

	public UniWebViewMessage(string rawMessage)
	{
		this = default(UniWebViewMessage);
		this.rawMessage = rawMessage;
		string[] array = rawMessage.Split(new string[1]
		{
			"://"
		}, StringSplitOptions.None);
		if (array.Length >= 2)
		{
			scheme = array[0];
			string text = "";
			for (int i = 1; i < array.Length; i++)
			{
				text += array[i];
			}
			string[] array2 = text.Split("?"[0]);
			path = array2[0].TrimEnd('/');
			args = new Dictionary<string, string>();
			if (array2.Length <= 1)
			{
				return;
			}
			string[] array3 = array2[1].Split("&"[0]);
			foreach (string text2 in array3)
			{
				string[] array4 = text2.Split("="[0]);
				if (array4.Length > 1)
				{
					args[array4[0]] = WWW.UnEscapeURL(array4[1]);
				}
			}
		}
		else
		{
			Debug.LogError("Bad url scheme. Can not be parsed to UniWebViewMessage: " + rawMessage);
		}
	}
}
