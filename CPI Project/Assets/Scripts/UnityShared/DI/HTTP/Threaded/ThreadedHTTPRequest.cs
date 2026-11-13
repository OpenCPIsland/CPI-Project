using DI.HTTP.Security;
using DI.HTTP.Security.Pinning;
using DI.Threading;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using UnityEngine;

namespace DI.HTTP.Threaded
{
	public class ThreadedHTTPRequest : HTTPBaseRequestImpl, IHTTPRequest
	{
		public static int DOWNLOAD_BUFFER_SIZE = 1024;

		private static string[] sizes = new string[4]
		{
			"B",
			"KB",
			"MB",
			"GB"
		};

		public ThreadedHTTPRequest(ThreadedHTTPClient client)
			: base(client)
		{
		}

		public override IHTTPResponse performSync()
		{
			return send();
		}

		public override void performAsync()
		{
			UnityThreadHelper.CreateThread((Action)delegate
			{
				send();
			});
		}

		public override string getUrl()
		{
			lock (this)
			{
				return base.getUrl();
			}
		}

		public override void setUrl(string url)
		{
			lock (this)
			{
				base.setUrl(url);
			}
		}

		public static bool ThreadSafeBaseValidateCertificate(RequestDigestSet requestDigestSet, X509Certificate certificate, SslPolicyErrors sslPolicyError)
		{
			if (certificate != null)
			{
				DigestSet digestSet = new DigestSet();
				byte[] rawCertData = certificate.GetRawCertData();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				requestDigestSet.CertificateDigest = digestSet;
				digestSet = new DigestSet();
				rawCertData = certificate.GetPublicKey();
				digestSet.setSha1(DigestHelper.sha1(rawCertData));
				digestSet.setSha256(DigestHelper.sha256(rawCertData));
				requestDigestSet.SubjectDigest = digestSet;
				return true;
			}
			return false;
		}

		public static bool ThreadSafeValidateCertificate(IPinset pinset, HttpWebRequest httpWebRequest, X509Certificate certificate, SslPolicyErrors sslPolicyErrors)
		{
			try
			{
				RequestDigestSet requestDigestSet = new RequestDigestSet();
				if (requestDigestSet.ValidateCertificate(certificate, sslPolicyErrors))
				{
					if (pinset == null)
					{
						return sslPolicyErrors == SslPolicyErrors.None;
					}
					string text = httpWebRequest.RequestUri.ToString();
					IList<IPinningInfo> pinningInfo = pinset.getPinningInfo(text);
					bool result = true;
					if (pinningInfo != null)
					{
						foreach (IPinningInfo item in pinningInfo)
						{
							if (verified(item, requestDigestSet))
							{
								return true;
							}
							if (item.getMode() != PinningMode.ADVISORY)
							{
								result = false;
							}
						}
						return result;
					}
					UnityEngine.Debug.LogError("The URL '" + text + "' does not match any patterns in the certificate pinset file. The certificate it is presenting will not be trusted.");
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				UnityEngine.Debug.LogException(ex);
			}
			return false;
		}

		protected static bool verified(IPinningInfo pin, RequestDigestSet requestDigestSet)
		{
			bool flag = false;
			try
			{
				if (pin.getExpiration().CompareTo(DateTime.Now) <= 0)
				{
					return false;
				}
				switch (pin.getMode())
				{
				case PinningMode.STRICT:
					flag = pin.getCertificate().compareDigests(requestDigestSet.CertificateDigest);
					break;
				case PinningMode.PERMISSIVE:
					flag = pin.getCertificate().compareDigests(requestDigestSet.CertificateDigest);
					if (!flag)
					{
						logFailure(PinningMode.PERMISSIVE, PinningTarget.CERTIFICATE);
						flag = pin.getSubject().compareDigests(requestDigestSet.SubjectDigest);
					}
					break;
				case PinningMode.ADVISORY:
					flag = pin.getCertificate().compareDigests(requestDigestSet.CertificateDigest);
					if (!flag)
					{
						logFailure(PinningMode.ADVISORY, PinningTarget.CERTIFICATE);
						flag = pin.getSubject().compareDigests(requestDigestSet.SubjectDigest);
						if (!flag)
						{
							logFailure(PinningMode.ADVISORY, PinningTarget.SUBJECT);
						}
					}
					break;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				UnityEngine.Debug.LogException(ex);
			}
			return flag;
		}

		protected static void logFailure(PinningMode mode, PinningTarget target)
		{
			UnityEngine.Debug.LogWarning(string.Concat("Failed to validate certificate against pinset. mode=", mode, ", target=", target.ToString()));
		}

		public override void OnStart()
		{
			IHTTPListener listener = getEffectiveListener();
			if (listener != null)
			{
				UnityThreadHelper.Dispatcher.Dispatch(delegate
				{
					listener.OnStart(this);
				});
			}
		}

		public override void OnProgress(byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
		{
			IHTTPListener listener = getEffectiveListener();
			if (listener != null)
			{
				UnityThreadHelper.Dispatcher.Dispatch(delegate
				{
					listener.OnProgress(this, data, bytesRead, bytesReceived, bytesExpected);
				});
			}
		}

		public override void OnSuccess(IHTTPResponse response)
		{
			IHTTPListener listener = getEffectiveListener();
			if (listener != null)
			{
				UnityThreadHelper.Dispatcher.Dispatch(delegate
				{
					listener.OnSuccess(this, response);
				});
			}
		}

		public override void OnError(IHTTPResponse response, Exception exception)
		{
			IHTTPListener listener = getEffectiveListener();
			if (listener != null)
			{
				UnityThreadHelper.Dispatcher.Dispatch(delegate
				{
					listener.OnError(this, response, exception);
				});
			}
		}

		public override void OnComplete()
		{
			IHTTPListener listener = getEffectiveListener();
			if (listener != null)
			{
				UnityThreadHelper.Dispatcher.Dispatch(delegate
				{
					listener.OnComplete(this);
				});
			}
		}

		public bool ServerCertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			IPinset pinset = ((ThreadedHTTPFactory)getClient().getFactory()).getPinset();
			return ThreadSafeValidateCertificate(pinset, (HttpWebRequest)sender, certificate, sslPolicyErrors);
		}

		private IHTTPResponse send()
		{
			byte[] array = new byte[DOWNLOAD_BUFFER_SIZE];
			ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
			Stopwatch stopwatch = new Stopwatch();
			HttpWebRequest httpWebRequest = null;
			byte[] array2 = null;
			int num = 0;
			HTTPBaseResponseImpl hTTPBaseResponseImpl = new HTTPBaseResponseImpl(this);
			try
			{
				httpWebRequest = (HttpWebRequest)WebRequest.Create(getUrl());
				httpWebRequest.Method = getMethod().ToString();
				ServicePointManager.Expect100Continue = false;
				foreach (string key in getRequestHeaders().Keys)
				{
					foreach (string item in getRequestHeaders()[key])
					{
						try
						{
							if (key.Equals("Content-Type"))
							{
								httpWebRequest.ContentType = item;
							}
							else if (key.Equals("Content-Length"))
							{
								httpWebRequest.ContentLength = Convert.ToInt64(item);
							}
							else if (key.Equals("Accept"))
							{
								httpWebRequest.Accept = item;
							}
							else
							{
								httpWebRequest.Headers.Set(key, item);
							}
						}
						catch (ArgumentException ex)
						{
							UnityEngine.Debug.LogWarning("Failed to add the header named '" + key + "' to the request. Exception: " + ex.Message);
						}
					}
				}
				if ((getMethod() == HTTPMethod.POST || getMethod() == HTTPMethod.PUT) && getDocument() != null)
				{
					array2 = getDocument().getData();
					if (array2 != null && array2.Length > 0)
					{
						httpWebRequest.ContentLength = array2.Length;
						using (Stream stream = httpWebRequest.GetRequestStream())
						{
							stream.Write(array2, 0, array2.Length);
							stream.Close();
						}
					}
				}
				stopwatch.Start();
				HttpWebResponse httpWebResponse = null;
				Exception ex2 = null;
				try
				{
					httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
				}
				catch (WebException ex3)
				{
					UnityEngine.Debug.LogException(ex3);
					ex2 = ex3;
					if (ex3.Status == WebExceptionStatus.ProtocolError)
					{
						httpWebResponse = (HttpWebResponse)ex3.Response;
					}
				}
				catch (SocketException ex4)
				{
					UnityEngine.Debug.LogException(ex4);
					ex2 = ex4;
				}
				long elapsedMilliseconds = stopwatch.ElapsedMilliseconds;
				if (httpWebResponse == null)
				{
					if (ThreadedHTTPFactory.LogAllRequests)
					{
						UnityEngine.Debug.Log(InfoString(httpWebRequest, array2, null, null, elapsedMilliseconds, ThreadedHTTPFactory.VerboseLogging));
					}
				}
				else
				{
					byte[] array3 = null;
					using (Stream stream2 = httpWebResponse.GetResponseStream())
					{
						using (MemoryStream memoryStream = new MemoryStream())
						{
							int num2 = 0;
							while ((num2 = stream2.Read(array, 0, array.Length)) > 0 && (ThreadBase.CurrentThread == null || !ThreadBase.CurrentThread.ShouldStop))
							{
								memoryStream.Write(array, 0, num2);
								num += num2;
								OnProgress(array, num2, num, -1);
							}
							array3 = memoryStream.ToArray();
							memoryStream.Close();
						}
						stream2.Close();
					}
					if (ThreadedHTTPFactory.LogAllRequests)
					{
						UnityEngine.Debug.Log(InfoString(httpWebRequest, array2, httpWebResponse, array3, elapsedMilliseconds, ThreadedHTTPFactory.VerboseLogging));
					}
					hTTPBaseResponseImpl.setStatusCode((int)httpWebResponse.StatusCode);
					hTTPBaseResponseImpl.setReasonPhrase(httpWebResponse.StatusDescription);
					HTTPBaseDocumentImpl document = new HTTPBaseDocumentImpl(array3);
					hTTPBaseResponseImpl.setDocument(document);
				}
				if (ex2 == null)
				{
					OnSuccess(hTTPBaseResponseImpl);
				}
				else
				{
					OnError(hTTPBaseResponseImpl, ex2);
				}
			}
			catch (Exception exception)
			{
				UnityEngine.Debug.LogException(exception);
				OnError(hTTPBaseResponseImpl, exception);
			}
			OnComplete();
			return hTTPBaseResponseImpl;
		}

		public string InfoString(HttpWebRequest httpWebRequest, byte[] requestBytes, HttpWebResponse httpWebResponse, byte[] responseBytes, long responseTime, bool verbose)
		{
			Uri requestUri = httpWebRequest.RequestUri;
			string text = httpWebRequest.Method.ToString();
			string text2 = (httpWebResponse != null) ? Convert.ToString((int)httpWebResponse.StatusCode) : "---";
			string text3 = (httpWebResponse != null) ? httpWebResponse.StatusDescription : "Unknown";
			double num = (responseBytes != null) ? ((float)responseBytes.Length) : 0f;
			int num2 = 0;
			while (num >= 1024.0 && num2 + 1 < sizes.Length)
			{
				num2++;
				num /= 1024.0;
			}
			string text4 = string.Format("{0:0.##}{1}", num, sizes[num2]);
			string text5 = requestUri.ToString() + " [ " + text.ToUpper() + " ] [ " + text2 + " " + text3 + " ] [ " + text4 + " ] [ " + responseTime + "ms ]\n";
			if (verbose)
			{
				text5 = text5 + "\nRequest Headers:\n\n" + httpWebRequest.Headers.ToString();
				if (requestBytes != null)
				{
					text5 = text5 + "\nRequest Body:\n\n" + Encoding.UTF8.GetString(requestBytes);
				}
				text5 = ((httpWebResponse == null || httpWebResponse.Headers == null) ? (text5 + "n/a") : (text5 + "\n\nResponse Headers:\n\n" + httpWebResponse.Headers.ToString()));
				if (responseBytes != null)
				{
					text5 = text5 + "\nResponse Body:\n\n" + Encoding.UTF8.GetString(responseBytes);
				}
				text5 += "\n";
			}
			return text5;
		}
	}
}
