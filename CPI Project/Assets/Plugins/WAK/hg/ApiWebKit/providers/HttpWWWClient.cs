using hg.ApiWebKit.core.http;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit.providers
{
	[Obsolete("Use hg.ApiWebKit.providers.UnityWebRequestClient instead.")]
	public class HttpWWWClient : HttpAbstractProvider
	{
		private WWW _www = null;

		protected override IEnumerator sendImplementation()
		{
			if (Request.RequestModelResult.Verb == "GET")
			{
				_www = new WWW(Request.RequestModelResult.Uri, null, Request.RequestModelResult.Headers);
			}
			else
			{
				if (!(Request.RequestModelResult.Verb == "POST") && !(Request.RequestModelResult.Verb == "PUT") && !(Request.RequestModelResult.Verb == "DELETE"))
				{
					throw new NotSupportedException(Request.RequestModelResult.Verb + " verb is not supported by the HttpWWWClient.");
				}
				if (Request.RequestModelResult.Verb == "PUT")
				{
					Request.RequestModelResult.Headers.Add("X-HTTP-Method-Override", "PUT");
					Request.RequestModelResult.Operation.Log("WWW does not support PUT. Added \"X-HTTP-Method-Override: PUT\" header to request and sent as a POST.", LogSeverity.VERBOSE);
				}
				else if (Request.RequestModelResult.Verb == "DELETE")
				{
					Request.RequestModelResult.Headers.Add("X-HTTP-Method-Override", "DELETE");
					Request.RequestModelResult.Operation.Log("WWW does not support DELETE. Added \"X-HTTP-Method-Override: DELETE\" header to request and sent as a POST.", LogSeverity.VERBOSE);
				}
				_www = new WWW(Request.RequestModelResult.Uri, (Request.RequestModelResult.Data != null && Request.RequestModelResult.Data.Length != 0) ? Request.RequestModelResult.Data : new byte[1], Request.RequestModelResult.Headers);
			}
			while (base.TimeElapsed <= Request.RequestModelResult.Timeout && !_www.isDone && !RequestCancelFlag)
			{
				UpdateTransferProgress();
				yield return null;
			}
			UpdateTransferProgress();
			if (RequestCancelFlag)
			{
				RequestCancelFlag = false;
				disposeInternal();
				ChangeState(HttpRequestState.CANCELLED);
			}
			else if (base.TimeElapsed > Request.RequestModelResult.Timeout)
			{
				disposeInternal();
				ChangeState(HttpRequestState.TIMEOUT);
			}
			else if (!string.IsNullOrEmpty(getError()))
			{
				ChangeState(HttpRequestState.ERROR);
			}
			else
			{
				ChangeState(HttpRequestState.COMPLETED);
			}
			BehaviorComplete();
			Cleanup();
		}

		protected override float getTransferProgress()
		{
			if (Request.RequestModelResult.Verb == "GET")
			{
				return _www.progress;
			}
			if (Request.RequestModelResult.Verb == "POST" || Request.RequestModelResult.Verb == "PUT" || Request.RequestModelResult.Verb == "DELETE")
			{
				return _www.uploadProgress;
			}
			throw new NotSupportedException(Request.RequestModelResult.Verb + " verb is not supported by the WWWHttpClient.");
		}

		protected override void disposeInternal()
		{
			if (_www != null)
			{
				_www.Dispose();
				_www = null;
			}
		}

		protected override Dictionary<string, string> getResponseHeaders()
		{
			if (_www == null)
			{
				return null;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			foreach (KeyValuePair<string, string> responseHeader in _www.responseHeaders)
			{
				dictionary.Add(responseHeader.Key, responseHeader.Value);
			}
			return dictionary;
		}

		protected override string getError()
		{
			return (_www != null) ? _www.error : null;
		}

		protected override string getText()
		{
			return (_www != null) ? _www.text : null;
		}

		protected override byte[] getData()
		{
			return (_www != null) ? _www.bytes : null;
		}

		protected override HttpStatusCode getStatusCode()
		{
			Dictionary<string, string> responseHeaders = getResponseHeaders();
			HttpStatusCode result = HttpStatusCode.Unknown;
			if (responseHeaders == null)
			{
				Request.RequestModelResult.Operation.Log("Unable to parse HTTP status code because response headers could not be found.", LogSeverity.WARNING);
				return result;
			}
			if (responseHeaders.ContainsKey("STATUS"))
			{
				string text = responseHeaders["STATUS"];
				result = HttpStatusCode.Unknown;
				if (!string.IsNullOrEmpty(text))
				{
					int result2 = 0;
					string[] array = text.Split(' ');
					if (!int.TryParse(array[1], out result2))
					{
						Request.RequestModelResult.Operation.Log("Unable to parse HTTP status code from STATUS header.", LogSeverity.WARNING);
					}
					else
					{
						result = (HttpStatusCode)result2;
					}
				}
				else
				{
					Request.RequestModelResult.Operation.Log("Unable to parse HTTP status code because STATUS response header could not be found.", LogSeverity.WARNING);
				}
			}
			else if (Request.CompletionState == HttpRequestState.COMPLETED && string.IsNullOrEmpty(getError()))
			{
				Request.RequestModelResult.Operation.Log("Unable to parse HTTP status code.");
			}
			else
			{
				Request.RequestModelResult.Operation.Log("Unable to parse HTTP status code.", LogSeverity.WARNING);
			}
			return result;
		}
	}
}
