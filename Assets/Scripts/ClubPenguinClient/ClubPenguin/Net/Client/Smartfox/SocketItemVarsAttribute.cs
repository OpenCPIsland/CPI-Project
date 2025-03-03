using Sfs2X.Entities.Variables;
using System;

namespace ClubPenguin.Net.Client.Smartfox
{
	internal class SocketItemVarsAttribute : Attribute
	{
		public string Key
		{
			get;
			private set;
		}

		public VariableType Type
		{
			get;
			private set;
		}

		internal SocketItemVarsAttribute(string key, VariableType type)
		{
			Key = key;
			Type = type;
		}
	}
}
