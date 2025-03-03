using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace HTTP
{
	public class DiskCache : MonoBehaviour
	{
		private string cachePath = null;

		private static DiskCache _instance = null;

		public static DiskCache Instance
		{
			get
			{
				if (_instance == null)
				{
					GameObject gameObject = new GameObject("DiskCache", typeof(DiskCache));
					gameObject.hideFlags = HideFlags.HideAndDontSave;
					_instance = gameObject.GetComponent<DiskCache>();
				}
				return _instance;
			}
		}

		private void Awake()
		{
			cachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "uwcache");
			if (!Directory.Exists(cachePath))
			{
				Directory.CreateDirectory(cachePath);
			}
		}

		public DiskCacheOperation Fetch(Request request)
		{
			string text = "";
			using (MD5 mD = MD5.Create())
			{
				byte[] array = mD.ComputeHash(Encoding.ASCII.GetBytes(request.uri.ToString()));
				foreach (byte b in array)
				{
					text += b.ToString("X2");
				}
			}
			string text2 = Path.Combine(cachePath, text);
			if (File.Exists(text2) && File.Exists(text2 + ".etag"))
			{
				request.SetHeader("If-None-Match", File.ReadAllText(text2 + ".etag"));
			}
			DiskCacheOperation diskCacheOperation = new DiskCacheOperation();
			diskCacheOperation.request = request;
			StartCoroutine(DownloadAndSave(request, text2, diskCacheOperation));
			return diskCacheOperation;
		}

		private IEnumerator DownloadAndSave(Request request, string filename, DiskCacheOperation handle)
		{
			bool useCachedVersion = File.Exists(filename);
			Action<Request> callback = request.completedCallback;
			request.Send();
			while (!request.isDone)
			{
				yield return new WaitForEndOfFrame();
			}
			if (request.exception == null && request.response != null && request.response.status == 200)
			{
				string header = request.response.GetHeader("etag");
				if (header != "")
				{
					File.WriteAllBytes(filename, request.response.bytes);
					File.WriteAllText(filename + ".etag", header);
				}
				useCachedVersion = false;
			}
			if (useCachedVersion)
			{
				if (request.exception != null)
				{
					Debug.LogWarning("Using cached version due to exception:" + request.exception);
					request.exception = null;
				}
				request.response.status = 304;
				request.response.bytes = File.ReadAllBytes(filename);
				request.isDone = true;
			}
			handle.isDone = true;
			if (callback != null)
			{
				callback(request);
			}
		}
	}
}
