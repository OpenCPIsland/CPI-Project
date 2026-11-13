using ClubPenguin.Net.Domain;
using Disney.Kelowna.Common;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using hg.ApiWebKit;
using hg.ApiWebKit.core.http;
using hg.ApiWebKit.mappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace ClubPenguin.Net.Client
{
	public class APICall<T> where T : CPAPIHttpOperation
	{
		private ClubPenguinClient clubPenguinClient;

		private T operation;

		public event Action<T, HttpResponse> OnResponse;

		public event Action<T, HttpResponse> OnError;

		public event Action<T, HttpResponse> OnComplete;

		public APICall(ClubPenguinClient clubPenguinClient, T operation)
		{
			this.clubPenguinClient = clubPenguinClient;
			this.operation = operation;
		}

		public void Execute()
		{
			ICommonGameSettings commonGameSettings = Service.Get<ICommonGameSettings>();
			if (clubPenguinClient.OfflineMode && string.IsNullOrEmpty(commonGameSettings.CPAPIServicehost))
			{
				CoroutineRunner.StartPersistent(runOffline(), this, "execute");
				return;
			}
			if (string.IsNullOrEmpty(commonGameSettings.CPAPIServicehost))
			{
				OnResponse += delegate
				{
					operation.UpdateOfflineData();
				};
			}
			clubPenguinClient.RecoverableOperationService.SendOperation(operation, this.OnResponse, this.OnError, this.OnComplete);
		}

		private IEnumerator runOffline()
		{
			yield return null;
			if (Log.Instance.ShouldLogMessage(Log.PriorityFlags.DEBUG, this))
			{
				try
				{
					FieldInfo[] fields = operation.GetType().GetFields();
					foreach (FieldInfo fieldInfo in fields)
					{
						if (fieldInfo.GetCustomAttributes(typeof(HttpRequestJsonBodyAttribute), true).Length > 0)
						{
							object obj = fieldInfo.GetValue(operation);
							if (isSubclassOfRawGeneric(fieldInfo.FieldType, typeof(SignedResponse<>)))
							{
								MethodInfo method = GetType().GetMethod("castSignedResponse", BindingFlags.Static | BindingFlags.NonPublic);
								if (method != null)
								{
									MethodInfo methodInfo = method.MakeGenericMethod(fieldInfo.FieldType.GetGenericArguments()[0]);
									if (methodInfo != null)
									{
										obj = methodInfo.Invoke(null, new object[1]
										{
											obj
										});
									}
								}
							}
							Service.Get<JsonService>().Serialize(obj);
							break;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			Log.PriorityFlags responseLogLevel = Log.PriorityFlags.DEBUG;
			HttpResponse response;
			try
			{
				response = operation.GetOfflineResponse();
			}
			catch (Exception ex2)
			{
				Log.LogException(this, ex2);
				response = new HttpResponse(new HttpRequest(new HttpRequestModel.HttpRequestModelResult()), 0.1f, new Dictionary<string, string>(), "{\"code\":999}", "{\"code\":999}", new byte[0], HttpStatusCode.InternalServerError);
				responseLogLevel = Log.PriorityFlags.ERROR;
			}
			if (Log.Instance.ShouldLogMessage(responseLogLevel, this))
			{
				try
				{
					FieldInfo[] fields = operation.GetType().GetFields();
					foreach (FieldInfo fieldInfo in fields)
					{
						if (fieldInfo.GetCustomAttributes(typeof(HttpResponseJsonBodyAttribute), true).Length > 0)
						{
							object obj = fieldInfo.GetValue(operation);
							if (isSubclassOfRawGeneric(fieldInfo.FieldType, typeof(SignedResponse<>)))
							{
								MethodInfo method = GetType().GetMethod("castSignedResponse", BindingFlags.Static | BindingFlags.NonPublic);
								if (method != null)
								{
									MethodInfo methodInfo = method.MakeGenericMethod(fieldInfo.FieldType.GetGenericArguments()[0]);
									if (methodInfo != null)
									{
										obj = methodInfo.Invoke(null, new object[1]
										{
											obj
										});
									}
								}
							}
							if (obj != null)
							{
								Service.Get<JsonService>().Serialize(obj);
							}
							break;
						}
					}
				}
				catch (Exception)
				{
				}
			}
			if (response.Is2XX || response.Is100)
			{
				this.OnResponse.InvokeSafe(operation, response);
			}
			if (!response.Is2XX && !response.Is100)
			{
				this.OnError.InvokeSafe(operation, response);
			}
			this.OnComplete.InvokeSafe(operation, response);
		}

		private static bool isSubclassOfRawGeneric(Type type, Type generic)
		{
			while (type != null && type != typeof(object))
			{
				Type type2 = type.IsGenericType ? type.GetGenericTypeDefinition() : type;
				if (generic == type2)
				{
					return true;
				}
				type = type.BaseType;
			}
			return false;
		}

		private static T castSignedResponse<T>(object o)
		{
			if (o == null)
			{
				return default(T);
			}
			return ((SignedResponse<T>)o).Data;
		}
	}
}
