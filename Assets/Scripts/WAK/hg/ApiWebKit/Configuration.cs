using hg.ApiWebKit.core.http;
using hg.ApiWebKit.providers;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit
{
	public static class Configuration
	{
		private static Dictionary<string, object> settings = new Dictionary<string, object>
		{
			{
				"default-http-client",
				typeof(HttpWWWClient)
			},
			{
				"request-timeout",
				10f
			},
			{
				"extend-timeout-on-transfer",
				true
			},
			{
				"destroy-operation-on-completion",
				true
			},
			{
				"log-WARNING",
				true
			},
			{
				"log-ERROR",
				true
			},
			{
				"log-INFO",
				true
			},
			{
				"log-VERBOSE",
				false
			},
			{
				"log-internal",
				true
			},
			{
				"log-callback",
				(Action<string, LogSeverity>)delegate
				{
				}
			},
			{
				"persistent-game-object-name",
				"unity3dassets.com"
			},
			{
				"persistent-game-object-flags",
				HideFlags.None
			},
			{
				"tiny-fsm-game-object-flags",
				HideFlags.HideAndDontSave
			},
			{
				"on-http-start",
				(Action<HttpRequest>)delegate
				{
				}
			},
			{
				"on-http-finish",
				(Action<HttpResponse>)delegate
				{
				}
			}
		};

		private static Dictionary<string, string> baseUris = new Dictionary<string, string>
		{
			{
				"default",
				"http://your.server.com/api"
			}
		};

		public static void SetDefaultBaseUri(string uri)
		{
			if (!baseUris.ContainsKey("default"))
			{
				baseUris.Add("default", uri);
			}
			else
			{
				baseUris["default"] = uri;
			}
			LogInternal("default base-uri set : " + uri);
		}

		public static void SetBaseUri(string name, string uri)
		{
			if (!baseUris.ContainsKey(name))
			{
				baseUris.Add(name, uri);
			}
			else
			{
				baseUris[name] = uri;
			}
			LogInternal(name + " base-uri set : " + uri);
		}

		public static string GetBaseUri(string name)
		{
			if (name == null)
			{
				return null;
			}
			if (baseUris.ContainsKey(name))
			{
				return validate_base_uri(baseUris[name]);
			}
			LogInternal(name + " does not exist in the base-uri dictionary", LogSeverity.ERROR);
			return null;
		}

		private static string validate_base_uri(string base_uri)
		{
			if (base_uri.EndsWith("/"))
			{
				LogInternal(base_uri + " base-uri must not end with '/'", LogSeverity.WARNING);
				return base_uri.Remove(base_uri.Length - 1, 1);
			}
			return base_uri;
		}

		public static bool HasSetting(string name)
		{
			return settings.ContainsKey(name);
		}

		public static T GetSetting<T>(string name)
		{
			if (settings.ContainsKey(name))
			{
				return (T)settings[name];
			}
			LogInternal("Configuration does NOT contain requested key : " + name, LogSeverity.WARNING);
			return default(T);
		}

		public static T GetSetting<T>(string name, T defaultValue)
		{
			if (settings.ContainsKey(name))
			{
				return (T)settings[name];
			}
			LogInternal("Configuration does NOT contain requested key : " + name, LogSeverity.WARNING);
			return defaultValue;
		}

		public static object GetSetting(string name)
		{
			return GetSetting<object>(name);
		}

		public static void SetSetting(string name, object value)
		{
			if (name == "log-VERBOSE" && (bool)value)
			{
				Debug.LogWarning("WARNING! (Web API Kit) Verbose logging has been turned on.  Verbose logging adversly affects performance!");
			}
			if (settings.ContainsKey(name))
			{
				LogInternal("Configuration value set : " + name + " = " + value, LogSeverity.VERBOSE);
				settings[name] = value;
			}
			else
			{
				LogInternal("Configuration value added : " + name + " = " + value, LogSeverity.VERBOSE);
				settings.Add(name, value);
			}
		}

		public static bool RemoveSetting(string name)
		{
			if (settings.ContainsKey(name))
			{
				return settings.Remove(name);
			}
			return false;
		}

		public static void Log(string message, LogSeverity severity = LogSeverity.INFO)
		{
			if (!GetSetting<bool>("log-" + severity))
			{
				return;
			}
			Action<string, LogSeverity> setting = GetSetting<Action<string, LogSeverity>>("log-callback");
			if (setting != null)
			{
				setting(message, severity);
				return;
			}
			switch (severity)
			{
			case LogSeverity.ERROR:
				Debug.LogError("<color=red>{ERR}</color> " + message);
				break;
			case LogSeverity.WARNING:
				Debug.LogWarning("<color=orange>{WARN}</color> " + message);
				break;
			case LogSeverity.INFO:
				Debug.Log("<color=white>{INFO}</color> " + message);
				break;
			case LogSeverity.DEBUG:
				Debug.Log("<color=grey>{DEBUG}</color> " + message);
				break;
			case LogSeverity.VERBOSE:
				Debug.Log("<color=grey>{VERB}</color> " + message);
				break;
			}
		}

		public static void LogInternal(string transactionId, string message, LogSeverity severity = LogSeverity.INFO)
		{
			if (GetSetting<bool>("log-internal"))
			{
				Log("<color=white>[" + transactionId + "]</color> " + message, severity);
			}
		}

		public static void LogInternal(string message, LogSeverity severity = LogSeverity.INFO)
		{
			if (GetSetting<bool>("log-internal"))
			{
				Log(message, severity);
			}
		}

		public static GameObject Bootstrap()
		{
			GameObject gameObject = GameObject.Find("/" + GetSetting<string>("persistent-game-object-name"));
			if (gameObject == null)
			{
				gameObject = new GameObject(GetSetting<string>("persistent-game-object-name"));
				gameObject.hideFlags = GetSetting<HideFlags>("persistent-game-object-flags");
				UnityEngine.Object.DontDestroyOnLoad(gameObject);
			}
			return gameObject;
		}
	}
}
