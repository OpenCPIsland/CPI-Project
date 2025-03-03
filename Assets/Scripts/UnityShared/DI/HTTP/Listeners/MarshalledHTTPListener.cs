using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DI.HTTP.Listeners
{
	public class MarshalledHTTPListener : IHTTPListener
	{
		public abstract class ListenerCallHolder
		{
			protected IHTTPRequest request = null;

			public ListenerCallHolder(IHTTPRequest request)
			{
				this.request = request;
			}

			public abstract void perform(IHTTPListener inner);
		}

		protected abstract class ListenerCallHolderWithResponse : ListenerCallHolder
		{
			protected IHTTPResponse response = null;

			public ListenerCallHolderWithResponse(IHTTPRequest request, IHTTPResponse response)
				: base(request)
			{
				this.response = response;
			}
		}

		protected class OnStartHolder : ListenerCallHolder
		{
			public OnStartHolder(IHTTPRequest request)
				: base(request)
			{
			}

			public override void perform(IHTTPListener inner)
			{
				inner.OnStart(request);
			}
		}

		protected class OnProgressHolder : ListenerCallHolder
		{
			protected byte[] data;

			protected int bytesRead;

			protected int bytesReceived;

			protected int bytesExpected;

			public OnProgressHolder(IHTTPRequest request, byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
				: base(request)
			{
				this.data = data;
				this.bytesRead = bytesRead;
				this.bytesReceived = bytesReceived;
				this.bytesExpected = bytesExpected;
			}

			public override void perform(IHTTPListener inner)
			{
				inner.OnProgress(request, data, bytesRead, bytesReceived, bytesExpected);
			}
		}

		protected class OnSuccessHolder : ListenerCallHolderWithResponse
		{
			public OnSuccessHolder(IHTTPRequest request, IHTTPResponse response)
				: base(request, response)
			{
			}

			public override void perform(IHTTPListener inner)
			{
				inner.OnSuccess(request, response);
			}
		}

		protected class OnErrorHolder : ListenerCallHolderWithResponse
		{
			protected Exception exception = null;

			public OnErrorHolder(IHTTPRequest request, IHTTPResponse response, Exception exception)
				: base(request, response)
			{
				this.exception = exception;
			}

			public override void perform(IHTTPListener inner)
			{
				inner.OnError(request, response, exception);
			}
		}

		protected class OnCompleteHolder : ListenerCallHolder
		{
			public OnCompleteHolder(IHTTPRequest request)
				: base(request)
			{
			}

			public override void perform(IHTTPListener inner)
			{
				inner.OnComplete(request);
			}
		}

		private IHTTPListener inner = null;

		private bool running = false;

		private MonoBehaviour context = null;

		private Queue<ListenerCallHolder> queue = new Queue<ListenerCallHolder>();

		public MarshalledHTTPListener(MonoBehaviour context, IHTTPListener inner)
		{
			if (context == null || inner == null)
			{
				throw new HTTPException("Marshalled listener requires a MonoBehaviour and a delegate listener.");
			}
			this.inner = inner;
			this.context = context;
			start();
		}

		protected void enqueue(ListenerCallHolder call)
		{
			lock (queue)
			{
				queue.Enqueue(call);
			}
		}

		public virtual void OnStart(IHTTPRequest request)
		{
			enqueue(new OnStartHolder(request));
		}

		public virtual void OnProgress(IHTTPRequest request, byte[] data, int bytesRead, int bytesReceived, int bytesExpected)
		{
			enqueue(new OnProgressHolder(request, data, bytesRead, bytesReceived, bytesExpected));
		}

		public virtual void OnSuccess(IHTTPRequest request, IHTTPResponse response)
		{
			enqueue(new OnSuccessHolder(request, response));
		}

		public virtual void OnError(IHTTPRequest request, IHTTPResponse response, Exception exception)
		{
			enqueue(new OnErrorHolder(request, response, exception));
		}

		public virtual void OnComplete(IHTTPRequest request)
		{
			enqueue(new OnCompleteHolder(request));
		}

		private IEnumerator dequeue()
		{
			while (isRunning())
			{
				ListenerCallHolder op;
				do
				{
					op = null;
					lock (queue)
					{
						if (queue.Count > 0)
						{
							op = queue.Dequeue();
						}
					}
					if (op != null)
					{
						op.perform(inner);
					}
				}
				while (op != null);
				yield return true;
			}
		}

		protected bool isRunning()
		{
			lock (this)
			{
				return running;
			}
		}

		public void start()
		{
			lock (this)
			{
				running = true;
			}
			context.StartCoroutine(dequeue());
		}

		public void stop()
		{
			lock (this)
			{
				running = false;
			}
		}
	}
}
