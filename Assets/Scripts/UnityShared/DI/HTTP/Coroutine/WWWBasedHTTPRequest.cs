using System;
using System.Collections;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace DI.HTTP.Coroutine
{
	public class WWWBasedHTTPRequest : HTTPBaseRequestImpl, IHTTPRequest
	{
		public WWWBasedHTTPRequest(WWWBasedHTTPClient client)
			: base(client)
		{
		}

		public override IHTTPResponse performSync()
		{
			OnStart();
			WWW wWW = new WWW(getUrl());
			ServicePoint servicePoint = ServicePointManager.FindServicePoint(getUrl(), null);
			if (servicePoint != null)
			{
				Debug.Log("Located service point.");
				X509Certificate certificate = servicePoint.Certificate;
				if (certificate != null)
				{
					Debug.Log("Certificate hash: " + certificate.GetCertHash());
				}
			}
			else
			{
				Debug.Log("None found");
			}
			HTTPBaseResponseImpl hTTPBaseResponseImpl = new HTTPBaseResponseImpl(this);
			if (!string.IsNullOrEmpty(wWW.error))
			{
				hTTPBaseResponseImpl.setStatusCode(parseStatusFromMessage(wWW.error));
				OnError(hTTPBaseResponseImpl, new HTTPException(wWW.error));
			}
			else
			{
				hTTPBaseResponseImpl.setStatusCode(200);
				hTTPBaseResponseImpl.setDocument(new HTTPBaseDocumentImpl(wWW.bytes));
				OnSuccess(hTTPBaseResponseImpl);
			}
			OnComplete();
			return hTTPBaseResponseImpl;
		}

		public override void performAsync()
		{
			WWWBasedHTTPFactory wWWBasedHTTPFactory = (WWWBasedHTTPFactory)getClient().getFactory();
			MonoBehaviour context = wWWBasedHTTPFactory.getContext();
			context.StartCoroutine(request());
		}

		public override bool validateCertificate(X509Certificate certificate, SslPolicyErrors sslPolicyErrors)
		{
			return sslPolicyErrors == SslPolicyErrors.None;
		}

		private IEnumerator request()
		{
			OnStart();
			WWW www = new WWW(getUrl());
			ServicePoint sp = ServicePointManager.FindServicePoint(getUrl(), null);
			if (sp != null)
			{
				Debug.Log("Located service point.");
				X509Certificate certificate = sp.Certificate;
				if (certificate != null)
				{
					Debug.Log("Certificate hash: " + certificate.GetCertHash());
				}
			}
			else
			{
				Debug.Log("None found");
			}
			yield return www;
			if (!www.isDone)
			{
				yield return www;
			}
			HTTPBaseResponseImpl response = new HTTPBaseResponseImpl(this);
			if (!string.IsNullOrEmpty(www.error))
			{
				response.setStatusCode(parseStatusFromMessage(www.error));
				OnError(response, new HTTPException(www.error));
			}
			else
			{
				response.setStatusCode(200);
				response.setDocument(new HTTPBaseDocumentImpl(www.bytes));
				OnSuccess(response);
			}
			OnComplete();
		}

		protected int parseStatusFromMessage(string message)
		{
			int result = -1;
			if (message != null)
			{
				int num = message.IndexOf(' ');
				if (num != -1)
				{
					try
					{
						result = int.Parse(message.Substring(0, num));
					}
					catch (Exception)
					{
					}
				}
			}
			return result;
		}
	}
}
