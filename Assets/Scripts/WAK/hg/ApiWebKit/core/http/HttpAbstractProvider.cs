using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace hg.ApiWebKit.core.http
{
	[ExecuteInEditMode]
	public abstract class HttpAbstractProvider : MonoBehaviour
	{
		[SerializeField]
		private string _id;

		[SerializeField]
		private string _targetUri;

		[SerializeField]
		private float _transferProgress;

		[SerializeField]
		private float _elapsedTime;

		[SerializeField]
		private float _timeToLive;

		[SerializeField]
		private HttpRequestState _state;

		protected HttpRequest Request = null;

		private Action<HttpResponse> _onCompleteCallback = null;

		private Action<float, float, float> _onTransferProgressUpdateCallback = null;

		private Action<HttpRequestState, HttpRequestState> _onStateChangeCallback = null;

		protected bool RequestCancelFlag = false;

		public HttpRequestState CurrentState
		{
			get;
			private set;
		}

		public HttpRequestState PreviousState
		{
			get;
			private set;
		}

		public float TimeElapsed
		{
			get;
			private set;
		}

		public float TTL
		{
			get
			{
				return Request.RequestModelResult.Timeout - TimeElapsed;
			}
		}

		public float TransferProgress
		{
			get;
			private set;
		}

		public bool IsIdle
		{
			get
			{
				return CurrentState == HttpRequestState.IDLE;
			}
		}

		public bool IsRunning
		{
			get
			{
				return CurrentState == HttpRequestState.BUSY || CurrentState == HttpRequestState.STARTED;
			}
		}

		protected abstract IEnumerator sendImplementation();

		protected abstract float getTransferProgress();

		protected abstract void disposeInternal();

		protected abstract Dictionary<string, string> getResponseHeaders();

		protected abstract string getError();

		protected abstract string getText();

		protected abstract byte[] getData();

		protected abstract HttpStatusCode getStatusCode();

		public bool Send(HttpRequest httpRequest, Action<HttpResponse> onCompleteCallback, Action<float, float, float> onTransferProgressUpdateCallback, Action<HttpRequestState, HttpRequestState> onStateChangeCallback)
		{
			_id = httpRequest.RequestModelResult.TransactionId;
			_targetUri = httpRequest.RequestModelResult.Uri;
			Request = httpRequest;
			Request.Client = this;
			_onCompleteCallback = onCompleteCallback;
			_onTransferProgressUpdateCallback = onTransferProgressUpdateCallback;
			_onStateChangeCallback = onStateChangeCallback;
			if (!IsIdle)
			{
				BehaviorComplete();
				return false;
			}
			bool flag = false;
			if (Application.internetReachability == NetworkReachability.NotReachable || flag)
			{
				ChangeState(HttpRequestState.DISCONNECTED);
				BehaviorComplete();
				return false;
			}
			ChangeState(HttpRequestState.STARTED);
			ChangeState(HttpRequestState.BUSY);
			StartCoroutine(sendImplementation());
			return true;
		}

		protected void BehaviorComplete()
		{
			Request.CompletionState = CurrentState;
			if (_onCompleteCallback != null)
			{
				_onCompleteCallback(new HttpResponse(Request, TimeElapsed, getResponseHeaders(), getError(), getText(), getData(), getStatusCode()));
			}
			Dispose();
		}

		protected void ChangeState(HttpRequestState newState)
		{
			if (_onStateChangeCallback != null)
			{
				_onStateChangeCallback(CurrentState, newState);
			}
			PreviousState = CurrentState;
			CurrentState = newState;
			_state = CurrentState;
		}

		protected void UpdateTransferProgress()
		{
			TransferProgress = getTransferProgress();
			_transferProgress = TransferProgress;
			_elapsedTime = TimeElapsed;
			_timeToLive = TTL;
			if (_onTransferProgressUpdateCallback != null)
			{
				_onTransferProgressUpdateCallback(TransferProgress, TimeElapsed, TTL);
			}
		}

		public void Reset()
		{
			if (IsRunning)
			{
				RequestCancelFlag = true;
				return;
			}
			Cleanup();
			ChangeState(HttpRequestState.IDLE);
		}

		public void Dispose()
		{
			Reset();
			ChangeState(HttpRequestState.NONE);
			if (Configuration.GetSetting<bool>("destroy-operation-on-completion"))
			{
				UnityEngine.Object.Destroy(this);
			}
		}

		protected void Cleanup()
		{
			TimeElapsed = 0f;
			disposeInternal();
		}

		public void Awake()
		{
			ChangeState(HttpRequestState.IDLE);
		}

		public void Update()
		{
			if (IsRunning)
			{
				TimeElapsed += Time.deltaTime;
			}
		}
	}
}
