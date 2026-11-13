namespace SwrveUnity.Device
{
	public interface ICarrierInfo
	{
		string GetName();

		string GetIsoCountryCode();

		string GetCarrierCode();
	}
}
