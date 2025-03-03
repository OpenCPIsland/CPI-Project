using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit
{
	public static class Utilities
	{
		public static string InstanceToString(object instance)
		{
			string text = "";
			FieldInfo[] fields = instance.GetType().GetFields();
			foreach (FieldInfo fieldInfo in fields)
			{
				string text2 = text;
				text = string.Concat(text2, "<b>[", fieldInfo.Name, "]</b> = ", fieldInfo.GetValue(instance), "\n");
			}
			return "<color=red>{Class}</color> <color=white><b>[" + instance.GetType().FullName + "]</b></color>\n<color=white>{Public Fields}</color> \n" + text + "\n\n";
		}

		public static void Send<T>(this T operation, Action<T, HttpResponse> on_success = null, Action<T, HttpResponse> on_failure = null, Action<T, HttpResponse> on_complete = null, params string[] parameters) where T : HttpOperation
		{
			operation["on-complete"] = (Action<T, HttpResponse>)delegate(T self, HttpResponse response)
			{
				if (on_success != null && (response.Is2XX || response.Is100) && !self.IsFaulted)
				{
					on_success(self, response);
				}
				if (on_failure != null && ((!response.Is2XX && !response.Is100) || self.IsFaulted))
				{
					on_failure(self, response);
				}
				if (on_complete != null)
				{
					on_complete(self, response);
				}
			};
			operation.Send(parameters);
		}

		public static void Send<T>(this T operation, Action<T, HttpRequest> on_start = null, Action<T, HttpResponse> on_success = null, Action<T, HttpResponse> on_failure = null, Action<T, HttpResponse> on_complete = null, params string[] parameters) where T : HttpOperation
		{
			operation["on-start"] = (Action<T, HttpRequest>)delegate(T self, HttpRequest http_request)
			{
				if (on_start != null)
				{
					on_start(self, http_request);
				}
			};
			operation.Send(on_success, on_failure, on_complete, parameters);
		}

		public static void Send<T>(this T operation, string batchName, Action<T, HttpRequest> on_start = null, Action<T, HttpResponse> on_success = null, Action<T, HttpResponse> on_failure = null, Action<T, HttpResponse> on_complete = null, params string[] parameters) where T : HttpOperation
		{
			operation.Send(on_success, on_failure, on_complete, parameters);
		}
	}
}
