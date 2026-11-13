using System;
using System.Collections.Generic;
using System.Linq;

namespace DI.Threading
{
	public static class TaskExtension
	{
		public static Task WithName(this Task task, string name)
		{
			task.Name = name;
			return task;
		}

		public static Task<T> WithName<T>(this Task<T> task, string name)
		{
			task.Name = name;
			return task;
		}

		public static void WaitAll(this IEnumerable<Task> tasks)
		{
			foreach (Task task in tasks)
			{
				task.Wait();
			}
		}

		public static IEnumerable<Task> Then(this IEnumerable<Task> that, Task followingTask, DispatcherBase target)
		{
			int remaining = that.Count();
			object syncRoot = new object();
			foreach (Task item in that)
			{
				item.WhenFailed((Action)delegate
				{
					if (!followingTask.ShouldAbort)
					{
						followingTask.Abort();
					}
				});
				item.WhenSucceeded((Action)delegate
				{
					if (!followingTask.ShouldAbort)
					{
						lock (syncRoot)
						{
							remaining--;
							if (remaining == 0)
							{
								if (target != null)
								{
									followingTask.Run(target);
								}
								else if (ThreadBase.CurrentThread is TaskWorker)
								{
									followingTask.Run(((TaskWorker)ThreadBase.CurrentThread).TaskDistributor);
								}
								else
								{
									followingTask.Run();
								}
							}
						}
					}
				});
			}
			return that;
		}

		public static IEnumerable<Task> WhenSucceeded(this IEnumerable<Task> that, Action action, DispatcherBase target)
		{
			int remaining = that.Count();
			object syncRoot = new object();
			foreach (Task item in that)
			{
				item.WhenSucceeded((Action)delegate
				{
					lock (syncRoot)
					{
						remaining--;
						if (remaining == 0)
						{
							if (target == null)
							{
								action();
							}
							else
							{
								target.Dispatch(delegate
								{
									action();
								});
							}
						}
					}
				});
			}
			return that;
		}

		public static IEnumerable<Task> WhenFailed(this IEnumerable<Task> that, Action action, DispatcherBase target)
		{
			bool hasFailed = false;
			object syncRoot = new object();
			foreach (Task item in that)
			{
				item.WhenFailed((Action)delegate
				{
					lock (syncRoot)
					{
						if (!hasFailed)
						{
							hasFailed = true;
							if (target == null)
							{
								action();
							}
							else
							{
								target.Dispatch(delegate
								{
									action();
								});
							}
						}
					}
				});
			}
			return that;
		}

		public static Task OnResult(this Task task, Action<object> action)
		{
			return task.OnResult(action, null);
		}

		public static Task OnResult(this Task task, Action<object> action, DispatcherBase target)
		{
			return task.WhenSucceeded(delegate(Task t)
			{
				action(t.RawResult);
			}, target);
		}

		public static Task OnResult<T>(this Task task, Action<T> action)
		{
			return task.OnResult(action, null);
		}

		public static Task OnResult<T>(this Task task, Action<T> action, DispatcherBase target)
		{
			return task.WhenSucceeded(delegate(Task t)
			{
				action((T)t.RawResult);
			}, target);
		}

		public static Task<T> OnResult<T>(this Task<T> task, Action<T> action)
		{
			return task.OnResult(action, null);
		}

		public static Task<T> OnResult<T>(this Task<T> task, Action<T> action, DispatcherBase actionTarget)
		{
			return task.WhenSucceeded(delegate(Task<T> t)
			{
				action(t.Result);
			}, actionTarget);
		}

		public static Task<T> WhenSucceeded<T>(this Task<T> task, Action action)
		{
			return task.WhenSucceeded(delegate
			{
				action();
			}, null);
		}

		public static Task<T> WhenSucceeded<T>(this Task<T> task, Action<Task<T>> action)
		{
			return task.WhenSucceeded(action, null);
		}

		public static Task<T> WhenSucceeded<T>(this Task<T> task, Action<Task<T>> action, DispatcherBase target)
		{
			Action<Task<T>> perform = delegate(Task<T> t)
			{
				if (target == null)
				{
					action(t);
				}
				else
				{
					target.Dispatch(delegate
					{
						if (t.IsSucceeded)
						{
							action(t);
						}
					});
				}
			};
			return task.WhenEnded(delegate(Task<T> t)
			{
				if (t.IsSucceeded)
				{
					perform(t);
				}
			}, null);
		}

		public static Task WhenSucceeded(this Task task, Action action)
		{
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsSucceeded)
				{
					action();
				}
			});
		}

		public static Task WhenSucceeded(this Task task, Action<Task> action)
		{
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsSucceeded)
				{
					action(t);
				}
			});
		}

		public static Task WhenSucceeded(this Task task, Action<Task> action, DispatcherBase actiontargetTarget)
		{
			Action<Task> perform = delegate(Task t)
			{
				if (actiontargetTarget == null)
				{
					action(t);
				}
				else
				{
					actiontargetTarget.Dispatch(delegate
					{
						if (t.IsSucceeded)
						{
							action(t);
						}
					});
				}
			};
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsSucceeded)
				{
					perform(t);
				}
			}, null);
		}

		public static Task<T> WhenFailed<T>(this Task<T> task, Action action)
		{
			return task.WhenFailed(delegate
			{
				action();
			}, null);
		}

		public static Task<T> WhenFailed<T>(this Task<T> task, Action<Task<T>> action)
		{
			return task.WhenFailed(action, null);
		}

		public static Task<T> WhenFailed<T>(this Task<T> task, Action<Task<T>> action, DispatcherBase target)
		{
			return task.WhenEnded(delegate(Task<T> t)
			{
				if (t.IsFailed)
				{
					action(t);
				}
			}, target);
		}

		public static Task WhenFailed(this Task task, Action action)
		{
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsFailed)
				{
					action();
				}
			});
		}

		public static Task WhenFailed(this Task task, Action<Task> action)
		{
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsFailed)
				{
					action(t);
				}
			});
		}

		public static Task WhenFailed(this Task task, Action<Task> action, DispatcherBase target)
		{
			return task.WhenEnded(delegate(Task t)
			{
				if (t.IsFailed)
				{
					action(t);
				}
			}, target);
		}

		public static Task<T> WhenEnded<T>(this Task<T> task, Action action)
		{
			return task.WhenEnded(delegate
			{
				action();
			}, null);
		}

		public static Task<T> WhenEnded<T>(this Task<T> task, Action<Task<T>> action)
		{
			return task.WhenEnded(action, null);
		}

		public static Task<T> WhenEnded<T>(this Task<T> task, Action<Task<T>> action, DispatcherBase target)
		{
			task.TaskEnded += delegate
			{
				if (target == null)
				{
					action(task);
				}
				else
				{
					target.Dispatch(delegate
					{
						action(task);
					});
				}
			};
			return task;
		}

		public static Task WhenEnded(this Task task, Action action)
		{
			return task.WhenEnded((Action<Task>)delegate
			{
				action();
			});
		}

		public static Task WhenEnded(this Task task, Action<Task> action)
		{
			return task.WhenEnded(delegate(Task t)
			{
				action(t);
			}, null);
		}

		public static Task WhenEnded(this Task task, Action<Task> action, DispatcherBase target)
		{
			task.TaskEnded += delegate
			{
				if (target == null)
				{
					action(task);
				}
				else
				{
					target.Dispatch(delegate
					{
						action(task);
					});
				}
			};
			return task;
		}

		public static Task Then(this Task that, Task followingTask)
		{
			TaskDistributor target = null;
			if (ThreadBase.CurrentThread is TaskWorker)
			{
				target = ((TaskWorker)ThreadBase.CurrentThread).TaskDistributor;
			}
			return that.Then(followingTask, target);
		}

		public static Task Then(this Task that, Task followingTask, DispatcherBase target)
		{
			that.WhenFailed((Action)delegate
			{
				followingTask.Abort();
			});
			that.WhenSucceeded((Action)delegate
			{
				if (target != null)
				{
					followingTask.Run(target);
				}
				else if (ThreadBase.CurrentThread is TaskWorker)
				{
					followingTask.Run(((TaskWorker)ThreadBase.CurrentThread).TaskDistributor);
				}
				else
				{
					followingTask.Run();
				}
			});
			return that;
		}

		public static Task Await(this Task that, Task taskToWaitFor)
		{
			taskToWaitFor.Then(that);
			return that;
		}

		public static Task Await(this Task that, Task taskToWaitFor, DispatcherBase target)
		{
			taskToWaitFor.Then(that, target);
			return that;
		}

		public static Task<T> As<T>(this Task that)
		{
			return (Task<T>)that;
		}

		public static IEnumerable<Task> ContinueWhenAnyEnded(this IEnumerable<Task> tasks, Action action)
		{
			return tasks.ContinueWhenAnyEnded((Action<Task>)delegate
			{
				action();
			});
		}

		public static IEnumerable<Task> ContinueWhenAnyEnded(this IEnumerable<Task> tasks, Action<Task> action)
		{
			object syncRoot = new object();
			bool done = false;
			foreach (Task task in tasks)
			{
				task.WhenEnded(delegate(Task t)
				{
					lock (syncRoot)
					{
						if (!done)
						{
							done = true;
							action(t);
						}
					}
				});
			}
			return tasks;
		}

		public static IEnumerable<Task> ContinueWhenAllEnded(this IEnumerable<Task> tasks, Action action)
		{
			return tasks.ContinueWhenAllEnded((Action<IEnumerable<Task>>)delegate
			{
				action();
			});
		}

		public static IEnumerable<Task> ContinueWhenAllEnded(this IEnumerable<Task> tasks, Action<IEnumerable<Task>> action)
		{
			int count = tasks.Count();
			if (count == 0)
			{
				action(new Task[0]);
			}
			List<Task> finishedTasks = new List<Task>();
			object syncRoot = new object();
			foreach (Task task in tasks)
			{
				Task task2 = task;
				Action<Task> action2 = delegate
				{
					lock (syncRoot)
					{
						finishedTasks.Add(task);
						if (finishedTasks.Count == count)
						{
							action(finishedTasks);
						}
					}
				};
				task2.WhenEnded(action2);
			}
			return tasks;
		}
	}
}
