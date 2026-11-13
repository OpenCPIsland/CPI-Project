namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class GuestApiError
	{
		public string code
		{
			get;
			set;
		}

		public string inputName
		{
			get;
			set;
		}

		public string developerMessage
		{
			get;
			set;
		}

		public TemporaryToken data
		{
			get;
			set;
		}
	}
}
