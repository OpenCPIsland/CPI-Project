using DI.JSON;
using UnityEngine;

namespace DI.CMS.FileManagement
{
	public class FmsOptions
	{
		public string BaseURL
		{
			get;
			set;
		}

		public string CodeName
		{
			get;
			set;
		}

		public FmsEnvironment Env
		{
			get;
			set;
		}

		public FmsMode Mode
		{
			get;
			set;
		}

		public string ManifestVersion
		{
			get;
			set;
		}

		public IJSONParser JSONParser
		{
			get;
			set;
		}

		public IFMSListener FMSListener
		{
			get;
			set;
		}

		public MonoBehaviour Context
		{
			get;
			set;
		}

		public string RootUrl
		{
			get;
			set;
		}

		public override string ToString()
		{
			return string.Format("BaseURL: {0}, CodeName: {1}, Env {2}, Mode {3}, ManifestVersion {4}, RootUrl {5}", BaseURL, CodeName, Env, Mode, ManifestVersion, RootUrl);
		}
	}
}
