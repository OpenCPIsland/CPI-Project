using Sfs3.Client.Entities.Data;
using Sfs3.Client.Entities.Variables;
using System;

namespace ClubPenguin.Net.Utils
{
	public class SmartFoxUtil
	{
		public static Type GetTypeFromSFSVariableType(VariableType sfsType)
		{
			switch (sfsType)
			{
			case VariableType.BOOL:
				return typeof(bool);
			case VariableType.DOUBLE:
				return typeof(double);
			case VariableType.INT:
				return typeof(int);
			case VariableType.STRING:
				return typeof(string);
			case VariableType.OBJECT:
				return typeof(SFSObject);
			default:
				return typeof(ISFSObject);
			}
		}

		public static object GetValueFromSFSUserValue(UserVariable sfsValue, Type type)
		{
			if (sfsValue.IsNull)
			{
				return null;
			}
			if (type == typeof(bool))
			{
				return sfsValue.BoolValue.Value;
			}
			if (type == typeof(double))
			{
				return sfsValue.DoubleValue.Value;
			}
			if (type == typeof(int))
			{
				return sfsValue.IntValue.Value;
			}
			if (type == typeof(string))
			{
				return sfsValue.StringValue;
			}
			if (type == typeof(SFSObject))
			{
				return sfsValue.SFSObjectValue;
			}
			return sfsValue.SFSObjectValue;
		}
	}
}
