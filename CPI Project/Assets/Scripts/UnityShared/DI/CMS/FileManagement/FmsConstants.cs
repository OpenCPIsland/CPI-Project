namespace DI.CMS.FileManagement
{
	public class FmsConstants
	{
		public const string STATIC_MANIFEST = "{0}manifest/{1}/{2}/{3}.json";

		public const string TOKEN_ROOT = "{root}";

		public const string TOKEN_CODENAME = "{codename}";

		public const string TOKEN_ENVIRONMENT = "{environment}";

		public const string TOKEN_RELATIVE_PATH = "{relativePath}";

		public const string TOKEN_HASH = "{hash}";

		public const string TOKEN_FILENAME = "{filename}";

		public const string TOKEN_VERSION = "{version}";

		public const string URL_FORMAT = "{root}{codename}/{environment}/{relativePath}/{hash}.{filename}";
	}
}
