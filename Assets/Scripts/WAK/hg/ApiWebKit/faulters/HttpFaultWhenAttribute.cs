using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;
using System.Reflection;

namespace hg.ApiWebKit.faulters
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public sealed class HttpFaultWhenAttribute : HttpFaultAttribute
	{
		public string FieldName = null;

		public HttpFaultWhenCondition FieldOperator = HttpFaultWhenCondition.Unset;

		public object FieldValue = null;

		public HttpFaultWhenAttribute(string fieldName, HttpFaultWhenCondition fieldOperator, object fieldValue)
		{
			FieldName = fieldName;
			FieldOperator = fieldOperator;
			FieldValue = fieldValue;
		}

		public override void CheckFaults(HttpOperation operation, HttpResponse response)
		{
			string[] array = FieldName.Split(new char[1]
			{
				'.'
			}, StringSplitOptions.RemoveEmptyEntries);
			FieldInfo fieldInfo = null;
			object obj = operation;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (obj == null)
				{
					break;
				}
				fieldInfo = obj.GetType().GetField(text, BindingFlags.Instance | BindingFlags.Public);
				if (fieldInfo == null)
				{
					throw new NotSupportedException("HttpFaultWhen could not find '" + text + "' field when parsing '" + FieldName + "'");
				}
				obj = fieldInfo.GetValue(obj);
			}
			switch (FieldOperator)
			{
			case HttpFaultWhenCondition.Is:
				if (object.Equals(obj, FieldValue))
				{
					operation.Fault("Field '" + FieldName + "' is " + ((FieldValue != null) ? FieldValue.ToString() : "(null)") + " on response.");
				}
				break;
			case HttpFaultWhenCondition.IsNot:
				if (!object.Equals(obj, FieldValue))
				{
					operation.Fault("Field '" + FieldName + "' is not " + ((FieldValue != null) ? FieldValue.ToString() : "(null)") + " on response.");
				}
				break;
			}
		}
	}
}
