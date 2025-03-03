using System;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit.core.http
{
	[Serializable]
	public class HttpResponse
	{
		public HttpRequest Request;

		public float TimeToComplete;

		public Dictionary<string, string> Headers;

		public bool HasError;

		public string Error;

		[Multiline(50)]
		public string Text;

		[HideInInspector]
		public byte[] Data;

		public HttpStatusCode StatusCode;

		public bool Is100
		{
			get
			{
				return StatusCode == HttpStatusCode.Continue;
			}
		}

		public bool Is2XX
		{
			get
			{
				return StatusCode >= HttpStatusCode.OK && StatusCode < HttpStatusCode.MultipleChoices;
			}
		}

		public HttpResponse(HttpRequest request, float timeToComplete, Dictionary<string, string> responseHeaders, string responseError, string responseText, byte[] responseData, HttpStatusCode statusCode)
		{
			Request = request;
			TimeToComplete = timeToComplete;
			Headers = responseHeaders;
			Error = responseError;
			Text = responseText;
			Data = responseData;
			StatusCode = statusCode;
			if (Request.CompletionState == HttpRequestState.TIMEOUT)
			{
				Request.RequestModelResult.Operation.Log("Request has timed out.", LogSeverity.ERROR);
			}
			if (!string.IsNullOrEmpty(Error) || Request.CompletionState != HttpRequestState.COMPLETED || StatusCode == HttpStatusCode.Unknown)
			{
				HasError = true;
			}
			Action<HttpResponse> setting = Configuration.GetSetting<Action<HttpResponse>>("on-http-finish");
			if (setting != null)
			{
				setting(this);
			}
		}

		public string Summary()
		{
			string text = "";
			if (Headers != null)
			{
				foreach (KeyValuePair<string, string> header in Headers)
				{
					string text2 = text;
					text = text2 + "\t<color=grey>Key: " + header.Key + " Value: " + header.Value + "</color>\n";
				}
			}
			string text3 = "";
			if (Request.RequestModelResult.Operation.FaultReasons != null)
			{
				foreach (string faultReason in Request.RequestModelResult.Operation.FaultReasons)
				{
					text3 = text3 + faultReason + "\n";
				}
			}
			return string.Concat("<color=", (!HasError) ? "white" : "red", "><b>HTTP Response</b> [", (int)StatusCode, " ", StatusCode, "] (", TimeToComplete, ")</color>\n<color=grey>Transaction-Id: </color><color=cyan>", Request.RequestModelResult.TransactionId, "</color>\n<color=grey>Verb: </color><color=cyan>", Request.RequestModelResult.Verb, "</color>\n<color=grey>Uri: </color><color=cyan>", Request.RequestModelResult.Uri, "</color>\n<color=grey>Request-Completion-State: </color><color=cyan>", Request.CompletionState, "</color>\n<color=grey>Status-Code: </color><color=cyan>(", (int)StatusCode, ") ", StatusCode, "</color>\n<color=grey>Is-100: </color><color=cyan>", Is100, "</color>\n<color=grey>Is-200s: </color><color=cyan>", Is2XX, "</color>\n<color=grey>Time-To-Complete: </color><color=cyan>", TimeToComplete, "</color>\n<color=grey>Has-Error: </color><color=cyan>", HasError, "</color>\n<color=grey>Error-Text: </color><color=cyan>", Error, "</color>\n<color=grey>Is-Faulted: </color><color=cyan>", Request.RequestModelResult.Operation.IsFaulted, "</color>\n<color=grey>Fault-Reason: </color><color=cyan>", text3, "</color>\n<color=grey>Data-Length: </color><color=cyan>", (Data != null) ? Data.Length.ToString() : "(null)", "</color>\n<color=grey>Headers: </color><color=cyan>", (Headers != null) ? Headers.Count.ToString() : "(null)", "</color>\n", text);
		}

		public string LogSummary()
		{
			string text = "";
			if (Headers != null)
			{
				foreach (KeyValuePair<string, string> header in Headers)
				{
					string text2 = text;
					text = text2 + "\t<color=grey>Key: " + header.Key + " Value: " + header.Value + "</color>\n";
				}
			}
			string text3 = "";
			if (Request.RequestModelResult.Operation.FaultReasons != null)
			{
				foreach (string faultReason in Request.RequestModelResult.Operation.FaultReasons)
				{
					text3 = text3 + faultReason + "\n";
				}
			}
			string transactionId = Request.RequestModelResult.TransactionId;
			string verb = Request.RequestModelResult.Verb;
			string uri = Request.RequestModelResult.Uri;
			string text4 = (Text != null) ? Text.Replace("<", "&lt;").Replace(">", "&gt;") : "(no body)";
			return string.Concat("<color=white><b>[", transactionId, "]</b></color> <color=grey>HTTP Response:</color> <color=", (!HasError) ? "white" : "red", ">[", (int)StatusCode, " ", StatusCode, "]</color> <color=grey>(", TimeToComplete, ")</color>\n<color=green>", verb, "</color> <color=cyan>", uri, "</color>\n<color=white>", text4.Truncate(100, true), "</color>\n\n<color=grey>Transaction-Id: </color><color=cyan>", transactionId, "</color>\n<color=grey>Verb: </color><color=cyan>", verb, "</color>\n<color=grey>Uri: </color><color=cyan>", uri, "</color>\n<color=grey>Request-Completion-State: </color><color=cyan>", Request.CompletionState, "</color>\n<color=grey>Status-Code: </color><color=cyan>(", (int)StatusCode, ") ", StatusCode, "</color>\n<color=grey>Is-100: </color><color=cyan>", Is100, "</color>\n<color=grey>Is-200s: </color><color=cyan>", Is2XX, "</color>\n<color=grey>Time-To-Complete: </color><color=cyan>", TimeToComplete, "</color>\n<color=grey>Has-Error: </color><color=cyan>", HasError, "</color>\n<color=grey>Error-Text: </color><color=cyan>", Error, "</color>\n<color=grey>Is-Faulted: </color><color=cyan>", Request.RequestModelResult.Operation.IsFaulted, "</color>\n<color=grey>Fault-Reason: </color><color=cyan>", text3, "</color>\n<color=grey>Data-Length: </color><color=cyan>", (Data != null) ? Data.Length.ToString() : "(null)", "</color>\n<color=grey>Headers: </color><color=cyan>", (Headers != null) ? Headers.Count.ToString() : "(null)", "</color>\n", text, "\n<color=grey>Data-Text: </color><color=cyan>", text4, "</color>\n");
		}
	}
}
