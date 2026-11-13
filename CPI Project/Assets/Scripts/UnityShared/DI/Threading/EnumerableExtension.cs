using System;
using System.Collections.Generic;

namespace DI.Threading
{
	public static class EnumerableExtension
	{
		public static IEnumerable<Task> ParallelForEach<T>(this IEnumerable<T> that, Action<T> action)
		{
			return that.ParallelForEach(action, null);
		}

		public static IEnumerable<Task> ParallelForEach<T>(this IEnumerable<T> that, Action<T> action, TaskDistributor target)
		{
			return (IEnumerable<Task>)that.ParallelForEach(delegate(T element)
			{
				action(element);
				return default(Task.Unit);
			}, target);
		}

		public static IEnumerable<Task<TResult>> ParallelForEach<TResult, T>(this IEnumerable<T> that, Func<T, TResult> action)
		{
			return that.ParallelForEach(action);
		}

		public static IEnumerable<Task<TResult>> ParallelForEach<TResult, T>(this IEnumerable<T> that, Func<T, TResult> action, TaskDistributor target)
		{
			List<Task<TResult>> list = new List<Task<TResult>>();
			foreach (T item2 in that)
			{
				T tmp = item2;
				Task<TResult> item = Task.Create(() => action(tmp)).Run(target);
				list.Add(item);
			}
			return list;
		}

		public static IEnumerable<Task> SequentialForEach<T>(this IEnumerable<T> that, Action<T> action)
		{
			return that.SequentialForEach(action, null);
		}

		public static IEnumerable<Task> SequentialForEach<T>(this IEnumerable<T> that, Action<T> action, TaskDistributor target)
		{
			return (IEnumerable<Task>)that.SequentialForEach(delegate(T element)
			{
				action(element);
				return default(Task.Unit);
			}, target);
		}

		public static IEnumerable<Task<TResult>> SequentialForEach<TResult, T>(this IEnumerable<T> that, Func<T, TResult> action)
		{
			return that.SequentialForEach(action);
		}

		public static IEnumerable<Task<TResult>> SequentialForEach<TResult, T>(this IEnumerable<T> that, Func<T, TResult> action, TaskDistributor target)
		{
			List<Task<TResult>> list = new List<Task<TResult>>();
			Task task2 = null;
			foreach (T item in that)
			{
				T tmp = item;
				Task<TResult> task = Task.Create(() => action(tmp));
				if (task2 == null)
				{
					task.Run(target);
				}
				else
				{
					task2.WhenEnded((Action)delegate
					{
						task.Run(target);
					});
				}
				task2 = task;
				list.Add(task);
			}
			return list;
		}
	}
}
