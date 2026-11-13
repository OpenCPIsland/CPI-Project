namespace Disney.MobileNetwork
{
	internal interface IServiceWrapper
	{
		object Instance
		{
			get;
		}

		void Unset();
	}
}
