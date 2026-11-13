using System;
using System.Collections.Generic;

namespace hg.ApiWebKit.tinyworkflow
{
	public abstract class WorkflowStateObject
	{
		public IWorkflow Workflow;

		private Dictionary<string, object> _bucket;

		public Action<bool>[] StepResultCallbacks
		{
			get;
			private set;
		}

		public object this[string key]
		{
			get
			{
				if (_bucket.ContainsKey(key))
				{
					return _bucket[key];
				}
				return null;
			}
			set
			{
				if (_bucket.ContainsKey(key))
				{
					_bucket[key] = value;
				}
				else
				{
					_bucket.Add(key, value);
				}
			}
		}

		public WorkflowStateObject(params Action<bool>[] stepResultCallbacks)
			: this()
		{
			StepResultCallbacks = stepResultCallbacks;
		}

		public WorkflowStateObject()
		{
			_bucket = new Dictionary<string, object>();
		}

		public void SetResultCallbacks(params Action<bool>[] stepResultCallbacks)
		{
			StepResultCallbacks = stepResultCallbacks;
		}

		public virtual void OnWorkflowStart()
		{
		}

		public virtual void OnWorkflowStop()
		{
		}

		public virtual void OnWorkflowStepCompletion(int stepNumber, bool success, WorkflowStateObject state)
		{
		}
	}
}
