using hg.ApiWebKit.core.attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace hg.ApiWebKit.core.http
{
	public class HttpRequestModel
	{
		[Serializable]
		public class HttpRequestModelResult
		{
			[HideInInspector]
			public HttpOperation Operation = null;

			public string TransactionId = null;

			public float Timeout = 0f;

			public string Verb = null;

			public string Uri = null;

			public string EscapedUri = null;

			public Dictionary<string, string> Headers = null;

			[HideInInspector]
			public byte[] Data = null;

			public Type ProviderType = null;

			public string Summary()
			{
				string text = "";
				foreach (KeyValuePair<string, string> header in Headers)
				{
					string text2 = text;
					text = text2 + "\t<color=grey>Key: " + header.Key + " Value: " + header.Value + "</color>\n";
				}
				return "<color=white><b>HTTP Request Model Result</b></color>\n<color=grey>Transaction-Id: </color><color=cyan>" + TransactionId + "</color>\n<color=grey>Client-Type: </color><color=cyan>" + ProviderType.FullName + "</color>\n<color=grey>Verb: </color><color=cyan>" + Verb + "</color>\n<color=grey>Uri: </color><color=cyan>" + Uri + "</color>\n<color=grey>Escaped-Uri: </color><color=cyan>" + EscapedUri + "</color>\n<color=grey>Timeout: </color><color=cyan>" + Timeout + "</color>\n<color=grey>Data-Length: </color><color=cyan>" + ((Data != null) ? Data.Length.ToString() : "(null)") + "</color>\n<color=grey>Headers: </color><color=cyan>" + Headers.Count + "</color>\n" + text;
			}

			public string LogSummary()
			{
				string text = "";
				foreach (KeyValuePair<string, string> header in Headers)
				{
					string text2 = text;
					text = text2 + "\t<color=grey>Key: " + header.Key + " Value: " + header.Value + "</color>\n";
				}
				string text3 = (Data != null) ? Encoding.UTF8.GetString(Data) : "(no body)";
				return "<color=white><b>[" + TransactionId + "]</b></color> <color=grey>HTTP Request:</color>\n<color=green>" + Verb + " </color><color=cyan>" + Uri + "</color>\n<color=white>" + text3.Truncate(100, true) + "</color>\n\n<color=grey>Transaction-Id: </color><color=cyan>" + TransactionId + "</color>\n<color=grey>Client-Type: </color><color=cyan>" + ProviderType.FullName + "</color>\n<color=grey>Verb: </color><color=cyan>" + Verb + "</color>\n<color=grey>Uri: </color><color=cyan>" + Uri + "</color>\n<color=grey>Escaped-Uri: </color><color=cyan>" + EscapedUri + "</color>\n<color=grey>Timeout: </color><color=cyan>" + Timeout + "</color>\n<color=grey>Data-Length: </color><color=cyan>" + ((Data != null) ? Data.Length.ToString() : "(null)") + "</color>\n<color=grey>Headers: </color><color=cyan>" + Headers.Count + "</color>\n" + text + "\n<color=grey>Data-Text: </color><color=cyan>" + text3 + "</color>\n";
			}
		}

		private class HttpBinaryFormField
		{
			public string FieldName = null;

			public string FileName = null;

			public string MimeType = null;

			public byte[] Content = null;
		}

		private HttpOperation _owner;

		private HttpProviderAttribute _httpClient = null;

		private HttpPathAttribute _httpPath = null;

		private HttpMethodAttribute _httpVerb = null;

		private HttpTimeoutAttribute _httpTimeout = null;

		private Hashtable _uriTemplates;

		private Hashtable _formFields;

		private Dictionary<string, HttpBinaryFormField> _binaryFormFields;

		private Dictionary<string, string> _httpHeaders;

		private List<DictionaryEntry> _queryStrings;

		private string _stringBody;

		private byte[] _binaryBody;

		public HttpRequestModel(HttpOperation owner, HttpProviderAttribute httpClient, HttpPathAttribute httpPath, HttpMethodAttribute httpVerb, HttpTimeoutAttribute httpTimeout, HttpHeaderAttribute[] httpClassHeaders)
		{
			_owner = owner;
			_owner.Log("HttpRequestModel Initializing", LogSeverity.VERBOSE);
			_httpClient = httpClient;
			_httpPath = httpPath;
			_httpVerb = httpVerb;
			_httpTimeout = httpTimeout;
			_uriTemplates = new Hashtable();
			_formFields = new Hashtable();
			_binaryFormFields = new Dictionary<string, HttpBinaryFormField>();
			_httpHeaders = new Dictionary<string, string>();
			_queryStrings = new List<DictionaryEntry>();
			_stringBody = null;
			_binaryBody = null;
			_owner.Log("Processing class level maps.", LogSeverity.VERBOSE);
			foreach (HttpHeaderAttribute httpHeaderAttribute in httpClassHeaders)
			{
				if (httpHeaderAttribute.MapOnRequest())
				{
					_owner.Log("Processing map '" + httpHeaderAttribute.GetType().FullName + "'.", LogSeverity.VERBOSE);
					httpHeaderAttribute.Initialize();
					string text = httpHeaderAttribute.OnRequestResolveName(_owner, null);
					object value = httpHeaderAttribute.OnRequestResolveValue(text, _owner, null);
					value = httpHeaderAttribute.OnRequestApplyConverters(value, _owner, null);
					if (!string.IsNullOrEmpty(text))
					{
						AddHttpHeader(text, (value != null) ? value.ToString() : "");
					}
				}
			}
		}

		public void SetStringBody(string payload)
		{
			if (string.IsNullOrEmpty(payload))
			{
				_owner.Log("(HttpRequestBody) SetStringBody : payload cannot be empty.", LogSeverity.WARNING);
				return;
			}
			_stringBody = payload;
			_owner.Log("(HttpRequestBody) SetStringBody : '" + ((payload.Length >= 1000) ? ("[TRUNCATED] " + payload.Substring(0, 999)) : payload) + "'", LogSeverity.VERBOSE);
			SetBinaryBody(Encoding.UTF8.GetBytes(payload));
		}

		public void SetBinaryBody(byte[] payload)
		{
			_binaryBody = payload;
			if (payload == null || payload.Length < 1)
			{
				_owner.Log("(HttpRequestBody) SetBinaryBody is empty.", LogSeverity.WARNING);
			}
			else
			{
				_owner.Log("(HttpRequestBody) SetBinaryBody Length : " + payload.Length, LogSeverity.VERBOSE);
			}
		}

		public void AddFormField(string name, string value)
		{
			try
			{
				_formFields.Add(name, value);
				_owner.Log("(HttpRequestBody) AddFormField key:" + name + " value:" + value, LogSeverity.VERBOSE);
				if (string.IsNullOrEmpty(value))
				{
					_owner.Log("(HttpRequestBody) AddFormField value of key:" + name + " is empty.", LogSeverity.WARNING);
				}
			}
			catch (Exception)
			{
				_owner.Log("(HttpRequestBody) AddFormField FAILED key:" + name + " value:" + value, LogSeverity.WARNING);
			}
		}

		public void AddBinaryFormField(string name, string fileName, string mimeType, byte[] content)
		{
			try
			{
				_binaryFormFields.Add(name, new HttpBinaryFormField
				{
					FieldName = name,
					FileName = fileName,
					MimeType = mimeType,
					Content = content
				});
				_owner.Log("(HttpRequestBody) AddBinaryFormField key:" + name + " size:" + content.Length, LogSeverity.VERBOSE);
				if (content == null || content.Length < 1)
				{
					_owner.Log("(HttpRequestBody) AddBinaryFormField content of key:" + name + " is empty.", LogSeverity.WARNING);
				}
			}
			catch (Exception)
			{
				_owner.Log("(HttpRequestBody) AddBinaryFormField FAILED key:" + name + " size:" + content.Length, LogSeverity.WARNING);
			}
		}

		public void AddHttpHeader(string name, string value)
		{
			try
			{
				_httpHeaders.Add(name, value);
				_owner.Log("(HttpRequestBody) AddHttpHeader key:" + name + " value:" + value, LogSeverity.VERBOSE);
				if (string.IsNullOrEmpty(value))
				{
					_owner.Log("(HttpRequestBody) AddHttpHeader value of key:" + name + " is empty.", LogSeverity.WARNING);
				}
			}
			catch (Exception)
			{
				_owner.Log("(HttpRequestBody) AddHttpHeader FAILED key:" + name + " value:" + value, LogSeverity.WARNING);
			}
		}

		public void AddQueryString(string name, string value)
		{
			try
			{
				_queryStrings.Add(new DictionaryEntry(name, value));
				_owner.Log("(HttpRequestBody) AddQueryString key:" + name + " value:" + value, LogSeverity.VERBOSE);
				if (string.IsNullOrEmpty(value))
				{
					_owner.Log("(HttpRequestBody) AddQueryString value of key:" + name + " is empty.", LogSeverity.WARNING);
				}
			}
			catch (Exception)
			{
				_owner.Log("(HttpRequestBody) AddQueryString FAILED key:" + name + " value:" + value, LogSeverity.WARNING);
			}
		}

		public void AddUriTemplate(string name, string value)
		{
			try
			{
				_uriTemplates.Add(name, value);
				_owner.Log("(HttpRequestBody) AddUriTemplate key:" + name + " value:" + value, LogSeverity.VERBOSE);
				if (string.IsNullOrEmpty(value))
				{
					_owner.Log("(HttpRequestBody) AddUriTemplate value of key:" + name + " is empty.", LogSeverity.WARNING);
				}
			}
			catch (Exception)
			{
				_owner.Log("(HttpRequestBody) AddUriTemplate FAILED key:" + name + " value:" + value, LogSeverity.WARNING);
			}
		}

		private string buildQueryString()
		{
			string text = "";
			if (_queryStrings.Count > 0)
			{
				text = "?";
				int num = 1;
				foreach (DictionaryEntry queryString in _queryStrings)
				{
					string text2 = text;
					text = text2 + (string)queryString.Key + "=" + (string)queryString.Value + ((num >= _queryStrings.Count) ? "" : "&");
					num++;
				}
			}
			return text;
		}

		private string expandUriTemplate()
		{
			string text = _httpPath.Uri;
			IDictionaryEnumerator enumerator = _uriTemplates.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
					text = text.Replace((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return text;
		}

		private string buildUri()
		{
			return expandUriTemplate() + buildQueryString();
		}

		private WWWForm buildForm()
		{
			WWWForm wWWForm = new WWWForm();
			foreach (KeyValuePair<string, HttpBinaryFormField> binaryFormField in _binaryFormFields)
			{
				if (binaryFormField.Value.FileName == null && binaryFormField.Value.MimeType == null)
				{
					wWWForm.AddBinaryData(binaryFormField.Value.FieldName, binaryFormField.Value.Content);
				}
				else if (binaryFormField.Value.MimeType == null)
				{
					wWWForm.AddBinaryData(binaryFormField.Value.FieldName, binaryFormField.Value.Content, binaryFormField.Value.FileName);
				}
				else
				{
					wWWForm.AddBinaryData(binaryFormField.Value.FieldName, binaryFormField.Value.Content, binaryFormField.Value.FileName, binaryFormField.Value.MimeType);
				}
			}
			IDictionaryEnumerator enumerator2 = _formFields.GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator2.Current;
					wWWForm.AddField((string)dictionaryEntry.Key, (string)dictionaryEntry.Value);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator2 as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			foreach (KeyValuePair<string, string> header in wWWForm.headers)
			{
				if (!_httpHeaders.ContainsKey(header.Key))
				{
					AddHttpHeader(header.Key, header.Value);
				}
			}
			return wWWForm;
		}

		private byte[] buildData(WWWForm form)
		{
			if (_binaryBody == null || _binaryBody.Length == 0)
			{
				return form.data;
			}
			return _binaryBody;
		}

		public HttpRequestModelResult Build()
		{
			string text = buildUri();
			WWWForm form = buildForm();
			byte[] data = buildData(form);
			HttpRequestModelResult httpRequestModelResult = new HttpRequestModelResult();
			httpRequestModelResult.Operation = _owner;
			httpRequestModelResult.TransactionId = _owner.TransactionId;
			httpRequestModelResult.ProviderType = _httpClient.ProviderType;
			httpRequestModelResult.Timeout = _httpTimeout.Timeout;
			httpRequestModelResult.Verb = _httpVerb.Verb;
			httpRequestModelResult.Uri = text;
			httpRequestModelResult.EscapedUri = WWW.EscapeURL(text);
			httpRequestModelResult.Headers = _httpHeaders;
			httpRequestModelResult.Data = data;
			return httpRequestModelResult;
		}
	}
}
