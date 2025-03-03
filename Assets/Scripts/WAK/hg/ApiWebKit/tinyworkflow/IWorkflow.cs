using System.Collections;
using UnityEngine;

namespace hg.ApiWebKit.tinyworkflow
{
	public interface IWorkflow
	{
		string CurrentWorkflowName
		{
			get;
		}

		void StartWorkflow(string name, object stateObject);

		void StepComplete(bool success = true);

		void RepeatStep();

		void NextStep(bool success = true, bool notifyCompletion = true);

		void Stop();

		Coroutine StartCoroutine(IEnumerator routine);
	}
}
