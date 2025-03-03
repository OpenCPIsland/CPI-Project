using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal.GuestControllerDomain
{
	public class FieldRequirements : BaseFieldRequirements
	{
		public Dictionary<string, Dictionary<string, BaseFieldRequirements>> type
		{
			get;
			set;
		}
	}
}
