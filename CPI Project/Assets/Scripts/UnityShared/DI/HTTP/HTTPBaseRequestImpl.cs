using DI.HTTP.Security;
using DI.HTTP.Security.Pinning;
using System;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace DI.HTTP
{
	public abstract class HTTPBaseRequestImpl : HTTPListenerHelper, IHTTPRequest
	{
		private IHTTPClient client;

		private string url;

		private HTTPMethod method = HTTPMethod.GET;

		private IDictionary<string, IList<string>> headers = new Dictionary<string, IList<string>>();

		private IHTTPDocument document = null;

		private IDigestSet certificateDigest = null;

		private IDigestSet subjectDigest = null;

		public HTTPBaseRequestImpl(IHTTPClient client)
		{
			this.client = client;
		}

		public virtual IHTTPClient getClient()
		{
			return client;
		}

		public virtual string getUrl()
		{
			return url;
		}

		public virtual void setUrl(string url)
		{
			this.url = url;
		}

		public virtual HTTPMethod getMethod()
		{
			return method;
		}

		public virtual void setMethod(HTTPMethod method)
		{
			this.method = method;
		}

		public IDictionary<string, IList<string>> getRequestHeaders()
		{
			return headers;
		}

		public void setRequestHeaders(IDictionary<string, IList<string>> headers)
		{
			this.headers = headers;
		}

		public virtual void setDocument(IHTTPDocument document)
		{
			this.document = document;
		}

		public IHTTPDocument getDocument()
		{
			return document;
		}

		public abstract IHTTPResponse performSync();

		public abstract void performAsync();

		public virtual bool validateCertificate(X509Certificate certificate, SslPolicyErrors sslPolicyError)
		{
			if (certificate != null)
			{
				DigestSet digestSet = new DigestSet();
				byte[] rawCertData = certificate.GetRawCertData();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				setCertificateDigest(digestSet);
				digestSet = new DigestSet();
				rawCertData = certificate.GetPublicKey();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				setSubjectDigest(digestSet);
				return true;
			}
			return false;
		}

		public IDigestSet getCertificateDigest()
		{
			return certificateDigest;
		}

		protected void setCertificateDigest(IDigestSet certificateDigest)
		{
			this.certificateDigest = certificateDigest;
		}

		public IDigestSet getSubjectDigest()
		{
			return subjectDigest;
		}

		protected void setSubjectDigest(IDigestSet subjectDigest)
		{
			this.subjectDigest = subjectDigest;
		}

		public virtual void OnStart()
		{
			IHTTPListener effectiveListener = getEffectiveListener();
			if (effectiveListener != null)
			{
				try
				{
					effectiveListener.OnStart(this);
				}
				catch (Exception exception)
				{
					Debug.LogError("A listener threw an exception.");
					Debug.LogException(exception);
				}
			}
		}

		public virtual void OnProgress(byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
		{
			IHTTPListener effectiveListener = getEffectiveListener();
			if (effectiveListener != null)
			{
				try
				{
					effectiveListener.OnProgress(this, data, bytesRead, bytesReceived, bytesExpected);
				}
				catch (Exception exception)
				{
					Debug.LogError("A listener threw an exception.");
					Debug.LogException(exception);
				}
			}
		}

		public virtual void OnSuccess(IHTTPResponse response)
		{
			IHTTPListener effectiveListener = getEffectiveListener();
			if (effectiveListener != null)
			{
				try
				{
					effectiveListener.OnSuccess(this, response);
				}
				catch (Exception exception)
				{
					Debug.LogError("A listener threw an exception.");
					Debug.LogException(exception);
				}
			}
		}

		public virtual void OnError(IHTTPResponse response, Exception exception)
		{
			IHTTPListener effectiveListener = getEffectiveListener();
			if (effectiveListener != null)
			{
				try
				{
					effectiveListener.OnError(this, response, exception);
				}
				catch (Exception exception2)
				{
					Debug.LogError("A listener threw an exception.");
					Debug.LogException(exception2);
				}
			}
		}

		public virtual void OnComplete()
		{
			IHTTPListener effectiveListener = getEffectiveListener();
			if (effectiveListener != null)
			{
				try
				{
					effectiveListener.OnComplete(this);
				}
				catch (Exception exception)
				{
					Debug.LogError("A listener threw an exception.");
					Debug.LogException(exception);
				}
			}
		}

		internal virtual IHTTPListener getEffectiveListener()
		{
			IHTTPListener listener = getListener();
			return (listener != null) ? listener : getClient().getListener();
		}
	}
}
