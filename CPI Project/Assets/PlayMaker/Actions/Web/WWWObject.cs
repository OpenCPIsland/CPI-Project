// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

#if UNITY_2018_2_OR_NEWER
#pragma warning disable 618  
#endif

#if !(UNITY_SWITCH || UNITY_TVOS || UNITY_IPHONE || UNITY_IOS || UNITY_ANDROID || UNITY_FLASH || UNITY_PS3 || UNITY_PS4 || UNITY_XBOXONE || UNITY_BLACKBERRY || UNITY_WP8 || UNITY_PSM || UNITY_WEBGL || UNITY_SWITCH)

using UnityEngine;

#if UNITY_2018_3_OR_NEWER
using UnityEngine.Networking;
#endif

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory("WWW")]
	[Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
	public class WWWObject : FsmStateAction
	{
		[RequiredField]
		[Tooltip("Url to download data from.")]
		public FsmString url;

		[ActionSection("Results")]

		[UIHint(UIHint.Variable)]
		[Tooltip("Gets text from the url.")]
		public FsmString storeText;
		
		[UIHint(UIHint.Variable)]
		[Tooltip("Gets a Texture from the url.")]
		public FsmTexture storeTexture;

		#if ! UNITY_2018_3_OR_NEWER
        [UIHint(UIHint.Variable)]
		[ObjectType(typeof(MovieTexture))]
		[Tooltip("Gets a Texture from the url.")]
		public FsmObject storeMovieTexture;
		#endif

		[UIHint(UIHint.Variable)]
		[Tooltip("Error message if there was an error during the download.")]
		public FsmString errorString;

		[UIHint(UIHint.Variable)] 
		[Tooltip("How far the download progressed (0-1).")]
		public FsmFloat progress;

		[ActionSection("Events")] 
		
		[Tooltip("Event to send when the data has finished loading (progress = 1).")]
		public FsmEvent isDone;
		
		[Tooltip("Event to send if there was an error.")]
		public FsmEvent isError;

#if ! UNITY_2018_3_OR_NEWER
		private WWW wwwObject;
		#else
		private UnityWebRequest uwr;

		DownloadHandlerBuffer d;
#endif
		public override void Reset()
		{
			url = null;
			storeText = null;
			storeTexture = null;
			errorString = null;
			progress = null;
			isDone = null;
		}

		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(url.Value))
			{
				Finish();
				return;
			}

#if UNITY_2018_3_OR_NEWER
			if (!storeTexture.IsNone)
			{
				uwr = UnityWebRequestTexture.GetTexture(url.Value);
			}else{
				uwr = new UnityWebRequest(url.Value);
				d = new DownloadHandlerBuffer();
				uwr.downloadHandler = d;
			}

			uwr.SendWebRequest();

#else
			wwwObject = new WWW(url.Value);
#endif
		}


#if UNITY_2018_3_OR_NEWER
		
		public override void OnUpdate()
		{
			if (uwr == null)
			{
				errorString.Value = "Unity Web Request is Null!";
				Finish();
				return;
			}

			errorString.Value = uwr.error;

			if (!string.IsNullOrEmpty(uwr.error))
			{
				uwr.Dispose();
				Finish();
				Fsm.Event(isError);
				return;
			}

			progress.Value = uwr.downloadProgress;

			if (progress.Value.Equals(1f) && uwr.isDone)
			{
				if (!storeText.IsNone)
				{
					storeText.Value = uwr.downloadHandler.text;
				}

				if (!storeTexture.IsNone)
				{
					storeTexture.Value = ((DownloadHandlerTexture)uwr.downloadHandler).texture as Texture;
				}

				errorString.Value = uwr.error;

				uwr.Dispose();

				Fsm.Event(string.IsNullOrEmpty(errorString.Value) ? isDone : isError);

				Finish();
			}
		}

#else

		public override void OnUpdate()
		{
			if (wwwObject == null)
			{
				errorString.Value = "WWW Object is Null!";
				Finish();
				return;
			}

			errorString.Value = wwwObject.error;

			if (!string.IsNullOrEmpty(wwwObject.error))
			{
				wwwObject.Dispose();
				Finish();
				Fsm.Event(isError);
				return;
			}

			progress.Value = wwwObject.progress;

			if (progress.Value.Equals (1f) && wwwObject.isDone)
			{
				storeText.Value = wwwObject.text;
				storeTexture.Value = wwwObject.texture;

#if UNITY_2018_2_OR_NEWER
#if UNITY_5_6_OR_NEWER
                storeMovieTexture.Value = wwwObject.GetMovieTexture();
#else
                storeMovieTexture.Value = wwwObject.movie;
#endif
#endif
				
				
				errorString.Value = wwwObject.error;

				wwwObject.Dispose();
				
				Fsm.Event(string.IsNullOrEmpty(errorString.Value) ? isDone : isError);

				Finish();
			}
		}
#endif

		#if UNITY_EDITOR

		public override float GetProgress()
		{
#if !UNITY_2018_3_OR_NEWER
			if (wwwObject!=null)
			{
				return wwwObject.progress;
			}
#else
		    if (uwr != null)
		    {
		        return uwr.downloadProgress;
		    }
#endif
			return 0f;
		}

		#endif
		
	}
}

#endif