using hg.ApiWebKit.core.attributes;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace hg.ApiWebKit.core.http
{
	[Serializable]
	public class HttpOperation
	{
		private HttpRequest _httpRequest = null;

		private FieldInfo[] _cachedFieldInfos = null;

		public bool IsFaulted;

		[SerializeField]
		public List<string> FaultReasons = new List<string>();

		private Dictionary<string, Delegate> _userActions = new Dictionary<string, Delegate>();

		public HttpRequest Request
		{
			get
			{
				return _httpRequest;
			}
		}

		public string[] InParameters
		{
			get;
			private set;
		}

		public string TransactionId
		{
			get;
			set;
		}

		public Delegate this[string actionName]
		{
			get
			{
				if (_userActions.ContainsKey(actionName))
				{
					return _userActions[actionName];
				}
				return null;
			}
			set
			{
				if (_userActions.ContainsKey(actionName))
				{
					Log("Setting '" + actionName + "' User Action", LogSeverity.VERBOSE);
					_userActions[actionName] = value;
				}
				else
				{
					Log("Adding '" + actionName + "' User Action", LogSeverity.VERBOSE);
					_userActions.Add(actionName, value);
				}
			}
		}

		public void Log(string message, LogSeverity severity = LogSeverity.INFO)
		{
			Configuration.LogInternal(TransactionId, message, severity);
		}

		public virtual void Fault(string reason)
		{
			IsFaulted = true;
			FaultReasons.Add(reason);
		}

		public virtual void CancelRequest()
		{
			if (_httpRequest != null)
			{
				_httpRequest.Cancel();
			}
		}

		protected virtual void FromResponse(HttpResponse response)
		{
			if (response.Request.CompletionState == HttpRequestState.CANCELLED || response.Request.CompletionState == HttpRequestState.TIMEOUT || response.Request.CompletionState == HttpRequestState.DISCONNECTED)
			{
				Log("Request was cancelled, timed-out, or network was unavailable.  Error : " + ((response.Error != null) ? response.Error : "(null)"), LogSeverity.WARNING);
				Log(response.Summary(), LogSeverity.VERBOSE);
				Log("<color=grey>Data-Text: </color><color=cyan>" + ((response.Text != null) ? response.Text.Replace("<", "&lt;").Replace(">", "&gt;") : "(null)") + "</color>\n", LogSeverity.VERBOSE);
				return;
			}
			FieldInfo[] cachedFieldInfos = _cachedFieldInfos;
			foreach (FieldInfo fieldInfo in cachedFieldInfos)
			{
				Log("Processing field '" + fieldInfo.Name + "'.", LogSeverity.VERBOSE);
				HttpMappedValueAttribute[] array = (HttpMappedValueAttribute[])fieldInfo.GetCustomAttributes(typeof(HttpMappedValueAttribute), true);
				HttpMappedValueAttribute[] array2 = array;
				foreach (HttpMappedValueAttribute httpMappedValueAttribute in array2)
				{
					if (httpMappedValueAttribute.MapOnResponse())
					{
						Log("Processing map '" + httpMappedValueAttribute.GetType().FullName + "'.", LogSeverity.VERBOSE);
						httpMappedValueAttribute.Initialize();
						string name = httpMappedValueAttribute.OnResponseResolveName(this, fieldInfo, response);
						object value = httpMappedValueAttribute.OnResponseResolveValue(name, this, fieldInfo, response);
						value = httpMappedValueAttribute.OnResponseApplyConverters(value, this, fieldInfo);
						httpMappedValueAttribute.OnResponseResolveModel(value, this, fieldInfo);
					}
				}
			}
			HttpFaultAttribute[] array3 = (HttpFaultAttribute[])GetType().GetCustomAttributes(typeof(HttpFaultAttribute), true);
			HttpFaultAttribute[] array4 = array3;
			foreach (HttpFaultAttribute httpFaultAttribute in array4)
			{
				httpFaultAttribute.CheckFaults(this, response);
			}
			Log(response.Summary(), LogSeverity.VERBOSE);
			Log("<color=grey>Data-Text: </color><color=cyan>" + ((response.Text != null) ? response.Text.Replace("<", "&lt;").Replace(">", "&gt;") : "(null)") + "</color>\n", LogSeverity.VERBOSE);
			Configuration.Log(response.LogSummary(), (!response.HasError) ? LogSeverity.DEBUG : LogSeverity.ERROR);
		}

		public HttpPathAttribute GetHttpPath()
		{
			HttpPathAttribute[] array = (HttpPathAttribute[])GetType().GetCustomAttributes(typeof(HttpPathAttribute), true);
			if (array.Length == 0)
			{
				throw new Exception(GetType().Name + " class is missing [HttpPath] attribute.");
			}
			return array[0];
		}

		protected virtual HttpRequest ToRequest(params string[] parameters)
		{
			InParameters = parameters;
			TransactionId = string.Format("{0:X}", Guid.NewGuid().GetHashCode());
			Log("<color=white><b>'" + GetType().FullName + "'</b> HTTP Operation Initializing</color>");
			HttpPathAttribute httpPath = GetHttpPath();
			HttpMethodAttribute[] array = (HttpMethodAttribute[])GetType().GetCustomAttributes(typeof(HttpMethodAttribute), true);
			if (array.Length == 0)
			{
				throw new Exception(GetType().Name + " class is missing [HttpMethod] attribute.");
			}
			if (array.Length > 1)
			{
				Log("Multiple [HttpMethod] attributes found.  Verb " + array[0].Verb + " will be used.", LogSeverity.ERROR);
			}
			Log("<color=green>" + array[0].Verb + "</color><color=cyan><b> " + httpPath.Uri + "</b></color>");
			HttpProviderAttribute[] array2 = (HttpProviderAttribute[])GetType().GetCustomAttributes(typeof(HttpProviderAttribute), true);
			if (array2.Length == 0)
			{
				array2 = new HttpProviderAttribute[1]
				{
					new HttpProviderAttribute(Configuration.GetSetting<Type>("default-http-client"))
				};
				Log("Missing [HttpProvider] attribute.  Provider '" + array2[0].ProviderType.FullName + "' will be used.", LogSeverity.VERBOSE);
			}
			HttpTimeoutAttribute[] array3 = (HttpTimeoutAttribute[])GetType().GetCustomAttributes(typeof(HttpTimeoutAttribute), true);
			if (array3.Length == 0)
			{
				array3 = new HttpTimeoutAttribute[1]
				{
					new HttpTimeoutAttribute(Configuration.GetSetting<float>("request-timeout"))
				};
			}
			Log("Request TTL : " + array3[0].Timeout + " seconds.", LogSeverity.VERBOSE);
			HttpHeaderAttribute[] httpClassHeaders = (HttpHeaderAttribute[])GetType().GetCustomAttributes(typeof(HttpHeaderAttribute), true);
			_cachedFieldInfos = GetType().GetFields(BindingFlags.Instance | BindingFlags.Public);
			HttpRequestModel model = new HttpRequestModel(this, array2[0], httpPath, array[0], array3[0], httpClassHeaders);
			FieldInfo[] cachedFieldInfos = _cachedFieldInfos;
			foreach (FieldInfo fieldInfo in cachedFieldInfos)
			{
				Log("Processing field '" + fieldInfo.Name + "'.", LogSeverity.VERBOSE);
				HttpMappedValueAttribute[] array4 = (HttpMappedValueAttribute[])fieldInfo.GetCustomAttributes(typeof(HttpMappedValueAttribute), true);
				HttpMappedValueAttribute[] array5 = array4;
				foreach (HttpMappedValueAttribute httpMappedValueAttribute in array5)
				{
					if (httpMappedValueAttribute.MapOnRequest())
					{
						Log("Processing map '" + httpMappedValueAttribute.GetType().FullName + "'.", LogSeverity.VERBOSE);
						httpMappedValueAttribute.Initialize();
						string name = httpMappedValueAttribute.OnRequestResolveName(this, fieldInfo);
						object value = httpMappedValueAttribute.OnRequestResolveValue(name, this, fieldInfo);
						value = httpMappedValueAttribute.OnRequestApplyConverters(value, this, fieldInfo);
						httpMappedValueAttribute.OnRequestResolveModel(name, value, ref model, this, fieldInfo);
					}
				}
			}
			HttpAuthorizationAttribute[] array6 = (HttpAuthorizationAttribute[])GetType().GetCustomAttributes(typeof(HttpAuthorizationAttribute), true);
			if (array6.Length > 0)
			{
				HttpAuthorizationAttribute httpAuthorizationAttribute = array6[0];
				Log("Processing map '" + httpAuthorizationAttribute.GetType().FullName + "'.", LogSeverity.VERBOSE);
				httpAuthorizationAttribute.Initialize();
				string name2 = httpAuthorizationAttribute.OnRequestResolveName(this, null);
				object value2 = httpAuthorizationAttribute.OnRequestResolveValue(name2, this, null);
				value2 = httpAuthorizationAttribute.OnRequestApplyConverters(value2, this, null);
				httpAuthorizationAttribute.OnRequestResolveModel(name2, value2, ref model, this, null);
			}
			HttpRequestModel.HttpRequestModelResult httpRequestModelResult = model.Build();
			Log(httpRequestModelResult.Summary(), LogSeverity.VERBOSE);
			Configuration.Log(httpRequestModelResult.LogSummary(), LogSeverity.DEBUG);
			return new HttpRequest(httpRequestModelResult);
		}

		public virtual void Send(params string[] parameters)
		{
			HttpRequest request = ToRequest(parameters);
			SendRequest(request);
		}

		protected void SendRequest(HttpRequest request)
		{
			_httpRequest = request;
			HttpRequest.Send(request, delegate(HttpResponse response)
			{
				FromResponse(response);
				OnRequestComplete(response);
				tryUserAction("on-complete", this, response);
			}, delegate(float progress, float elapsed, float ttl)
			{
				OnTransferProgressUpdate(progress, elapsed, ttl);
				tryUserAction("on-progress", this, progress, elapsed, ttl);
			}, delegate(HttpRequestState from, HttpRequestState to)
			{
				OnRequestStateChange(from, to);
				tryUserAction("on-state-change", this, from, to);
			});
		}

		private void tryUserAction(string action, params object[] args)
		{
			Delegate @delegate = this[action];
			if ((object)@delegate != null)
			{
				Log("Invoking '" + action + "' User Action", LogSeverity.VERBOSE);
				try
				{
					@delegate.DynamicInvoke(args);
				}
				catch (Exception ex)
				{
					Log("'" + action + "' User Action invocation failed hard with error: " + ex.Message, LogSeverity.ERROR);
					if (ex.InnerException != null)
					{
						Log("'" + action + "' User Action invocation inner error: " + ex.InnerException.Message, LogSeverity.ERROR);
					}
				}
			}
		}

		public void InvokeUserAction(string actionName, params object[] args)
		{
			tryUserAction(actionName, args);
		}

		protected virtual void OnRequestComplete(HttpResponse response)
		{
			Log("Request Completed", LogSeverity.VERBOSE);
		}

		protected virtual void OnTransferProgressUpdate(float progress, float elapsed_time, float ttl)
		{
			Log("Request Progress Updated : " + progress, LogSeverity.VERBOSE);
		}

		protected virtual void OnRequestStateChange(HttpRequestState from, HttpRequestState to)
		{
			Log(string.Concat("Request State Changed : ", from, " => ", to), LogSeverity.VERBOSE);
		}
	}
}
