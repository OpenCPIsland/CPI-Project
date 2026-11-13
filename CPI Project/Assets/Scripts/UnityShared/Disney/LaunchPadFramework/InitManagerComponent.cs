using Disney.Kelowna.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Disney.LaunchPadFramework
{
	public class InitManagerComponent : MonoBehaviour
	{
		public enum PassName
		{
			FirstPass,
			SecondPass,
			CompletedPass
		}

		public class ActionDuration
		{
			private readonly Dictionary<PassName, TimeSpan> passNameToDuration = new Dictionary<PassName, TimeSpan>();

			public void SetPassDuration(PassName passName, TimeSpan duration)
			{
				passNameToDuration[passName] = duration;
			}

			public TimeSpan GetPassDuration(PassName passName)
			{
				TimeSpan value;
				if (!passNameToDuration.TryGetValue(passName, out value))
				{
					value = default(TimeSpan);
				}
				return value;
			}

			public TimeSpan GetTotalDuration()
			{
				TimeSpan result = default(TimeSpan);
				foreach (KeyValuePair<PassName, TimeSpan> item in passNameToDuration)
				{
					result += item.Value;
				}
				return result;
			}
		}

		private class ActionProcessor
		{
			private Dictionary<string, List<InitActionComponent>> dependencies;

			private Func<InitActionComponent, IEnumerator> functor;

			private Func<InitActionComponent, bool> conditionalRun;

			private Dictionary<Type, ActionDuration> actionDurations;

			private PassName passName;

			private int activeActions;

			private bool complete;

			private Dictionary<string, KeyValuePair<string, TimeSpan>> longestPaths = new Dictionary<string, KeyValuePair<string, TimeSpan>>();

			public ActionProcessor(Func<InitActionComponent, IEnumerator> functor, Dictionary<string, List<InitActionComponent>> dependencies, Dictionary<Type, ActionDuration> actionDurations, PassName passName, Func<InitActionComponent, bool> conditionalRun)
			{
				this.dependencies = new Dictionary<string, List<InitActionComponent>>(dependencies);
				this.functor = functor;
				this.actionDurations = actionDurations;
				this.passName = passName;
				this.conditionalRun = conditionalRun;
				activeActions = 0;
				complete = false;
			}

			public IEnumerator Execute(List<InitActionComponent> actions)
			{
				runActions(actions);
				while (!complete)
				{
					yield return null;
				}
			}

			private void runActions(List<InitActionComponent> actions)
			{
				List<InitActionComponent> list = new List<InitActionComponent>();
				foreach (InitActionComponent action in actions)
				{
					bool flag = true;
					foreach (string topologicalDependency in action.TopologicalDependencies)
					{
						if (dependencies.ContainsKey(topologicalDependency))
						{
							flag = false;
							break;
						}
					}
					if (flag)
					{
						list.Add(action);
					}
				}
				activeActions += list.Count;
				foreach (InitActionComponent item in list)
				{
					CoroutineRunner.StartPersistent(runActionAsyc(item), this, "processInitActions:" + item.TopologicalIdentifier);
				}
			}

			private IEnumerator runActionAsyc(InitActionComponent action)
			{
				Stopwatch timer = new Stopwatch();
				timer.Start();
				if (conditionalRun(action))
				{
					yield return functor(action);
				}
				activeActions--;
				timer.Stop();
				logActionDuration(action, timer.ElapsedTicks);
				if (dependencies.ContainsKey(action.TopologicalIdentifier))
				{
					List<InitActionComponent> actions = dependencies[action.TopologicalIdentifier];
					dependencies.Remove(action.TopologicalIdentifier);
					runActions(actions);
				}
				else if (activeActions < 1)
				{
					complete = true;
				}
			}

			[Conditional("NOT_RC_BUILD")]
			private void trackLongestPath(InitActionComponent action, List<InitActionComponent> dependants, long ticks)
			{
				foreach (InitActionComponent dependant in dependants)
				{
					longestPaths[dependant.TopologicalIdentifier] = new KeyValuePair<string, TimeSpan>(action.TopologicalIdentifier, new TimeSpan(ticks));
				}
			}

			[Conditional("NOT_RC_BUILD")]
			private void printLongestPath(InitActionComponent action, TimeSpan actionsTime)
			{
				TimeSpan timeSpan = new TimeSpan(actionsTime.Ticks);
				Stack<KeyValuePair<string, TimeSpan>> stack = new Stack<KeyValuePair<string, TimeSpan>>();
				string key = action.TopologicalIdentifier;
				KeyValuePair<string, TimeSpan> value;
				while (longestPaths.TryGetValue(key, out value))
				{
					stack.Push(value);
					key = value.Key;
					timeSpan = timeSpan.Add(value.Value);
				}
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.AppendFormat("Longest Path for {0}: {1:0.####} seconds\n", passName, timeSpan.TotalSeconds);
				while (stack.Count > 0)
				{
					value = stack.Pop();
					stringBuilder.AppendFormat("\t{0} {1:0.####}\n", value.Key, value.Value.TotalSeconds);
				}
				stringBuilder.AppendFormat("\t{0} {1:0.####}\n", action.TopologicalIdentifier, actionsTime.TotalSeconds);
			}

			private void logActionDuration(InitActionComponent action, long ticks)
			{
				Type type = action.GetType();
				ActionDuration value;
				if (!actionDurations.TryGetValue(type, out value))
				{
					value = new ActionDuration();
					actionDurations[type] = value;
				}
				value.SetPassDuration(passName, new TimeSpan(ticks));
			}
		}

		protected Dictionary<Type, ActionDuration> actionDurations = new Dictionary<Type, ActionDuration>();

		protected TimeSpan totalDuration;

		protected virtual IEnumerator init()
		{
			int frameCount = Time.frameCount;
			Stopwatch totalTimer = new Stopwatch();
			totalTimer.Start();
			List<InitActionComponent> initActions = GetComponents<InitActionComponent>().ToList();
			TimeSpan getComponentsDuration = new TimeSpan(totalTimer.ElapsedTicks);
			Dictionary<string, List<InitActionComponent>> dependencies = new Dictionary<string, List<InitActionComponent>>();
			List<InitActionComponent> dependencyFree = new List<InitActionComponent>();
			foreach (InitActionComponent item in initActions)
			{
				if (item.enabled)
				{
					if (item.TopologicalDependencies.Count == 0)
					{
						dependencyFree.Add(item);
					}
					else
					{
						foreach (string topologicalDependency in item.TopologicalDependencies)
						{
							if (!dependencies.ContainsKey(topologicalDependency))
							{
								dependencies.Add(topologicalDependency, new List<InitActionComponent>());
							}
							dependencies[topologicalDependency].Add(item);
						}
					}
				}
			}
			new TimeSpan(totalTimer.ElapsedTicks - getComponentsDuration.Ticks);
			yield return new ActionProcessor((InitActionComponent a) => a.PerformFirstPass(), dependencies, actionDurations, PassName.FirstPass, hasFirstPass).Execute(dependencyFree);
			yield return new ActionProcessor((InitActionComponent a) => a.PerformSecondPass(), dependencies, actionDurations, PassName.SecondPass, hasSecondPass).Execute(dependencyFree);
			yield return new ActionProcessor(initCompleteCoroutineWrapper, dependencies, actionDurations, PassName.CompletedPass, hasCompletePass).Execute(dependencyFree);
			totalDuration = new TimeSpan(totalTimer.ElapsedTicks);
		}

		private IEnumerator initCompleteCoroutineWrapper(InitActionComponent action)
		{
			action.OnInitializationComplete();
			yield break;
		}

		private bool hasFirstPass(InitActionComponent action)
		{
			return true;
		}

		private bool hasSecondPass(InitActionComponent action)
		{
			return action.HasSecondPass;
		}

		private bool hasCompletePass(InitActionComponent action)
		{
			return action.HasCompletedPass;
		}
	}
}
