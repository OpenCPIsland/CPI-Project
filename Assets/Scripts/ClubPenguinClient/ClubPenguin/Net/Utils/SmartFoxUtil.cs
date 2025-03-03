using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
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
			if (sfsValue.IsNull())
			{
				return null;
			}
			if (type == typeof(bool))
			{
				return Convert.ChangeType(sfsValue.GetBoolValue(), type);
			}
			if (type == typeof(double))
			{
				return Convert.ChangeType(sfsValue.GetDoubleValue(), type);
			}
			if (type == typeof(int))
			{
				return Convert.ChangeType(sfsValue.GetIntValue(), type);
			}
			if (type == typeof(string))
			{
				return Convert.ChangeType(sfsValue.GetStringValue(), type);
			}
			if (type == typeof(SFSObject))
			{
				return Convert.ChangeType(sfsValue.GetSFSObjectValue(), type);
			}
			return Convert.ChangeType(sfsValue.GetSFSObjectValue(), type);
		}
	}
}
