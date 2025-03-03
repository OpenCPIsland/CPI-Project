using hg.ApiWebKit;
using hg.ApiWebKit.core.attributes;
using hg.ApiWebKit.core.http;
using System;
using System.Reflection;
using System.Text;

namespace ClubPenguin.Net.Client.Mappers
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public sealed class HttpBasicAuthorizationAttribute : HttpHeaderAttribute
	{
		private string usernameConfig;

		private string passwordConfig;

		public HttpBasicAuthorizationAttribute(string usernameConfig, string passwordConfig)
			: base(MappingDirection.REQUEST, "Authorization")
		{
			this.usernameConfig = usernameConfig;
			this.passwordConfig = passwordConfig;
		}

		public override object OnRequestResolveValue(string name, HttpOperation operation, FieldInfo fi)
		{
			string str = "";
			if (Configuration.HasSetting(usernameConfig))
			{
				str = Configuration.GetSetting<string>(usernameConfig);
			}
			string str2 = "";
			if (Configuration.HasSetting(passwordConfig))
			{
				str2 = Configuration.GetSetting<string>(passwordConfig);
			}
			byte[] bytes = Encoding.UTF8.GetBytes(str + ":" + str2);
			string str3 = Convert.ToBase64String(bytes);
			Value = "Basic " + str3;
			return Value;
		}
	}
}
