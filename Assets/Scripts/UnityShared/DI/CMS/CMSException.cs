using System;

namespace DI.CMS
{
	public class CMSException : Exception
	{
		public CMSException(string message)
			: base(message)
		{
		}
	}
}
