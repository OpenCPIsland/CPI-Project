using System;
using System.Collections.Generic;

namespace DI.HTTP.Security.Pinning
{
	public class PinningInfo : IPinningInfo
	{
		private IDigestSet certificate;

		private IDigestSet subject;

		private DateTime expiration;

		private PinningMode mode;

		public PinningInfo()
		{
		}

		public PinningInfo(IDictionary<string, object> blob)
		{
			try
			{
				setCertificate(new DigestSet((IDictionary<string, object>)blob["certificate"]));
				setSubject(new DigestSet((IDictionary<string, object>)blob["subject"]));
				setExpiration(DateTime.SpecifyKind(DateTime.Parse((string)blob["expiration"]), DateTimeKind.Utc));
				setMode(modeFromString((string)blob["mode"]));
			}
			catch (Exception ex)
			{
				throw new HTTPException("Unable to parse Pinning Info. " + ex.Message);
			}
		}

		protected PinningMode modeFromString(string mode)
		{
			if (string.Compare("strict", mode, true) == 0)
			{
				return PinningMode.STRICT;
			}
			if (string.Compare("permissive", mode, true) == 0)
			{
				return PinningMode.PERMISSIVE;
			}
			if (string.Compare("advisory", mode, true) == 0)
			{
				return PinningMode.ADVISORY;
			}
			throw new HTTPException("The mode is invalid. (" + ((mode == null) ? "null" : mode) + ")");
		}

		public IDigestSet getCertificate()
		{
			return certificate;
		}

		public void setCertificate(IDigestSet certificate)
		{
			this.certificate = certificate;
		}

		public IDigestSet getSubject()
		{
			return subject;
		}

		public void setSubject(IDigestSet subject)
		{
			this.subject = subject;
		}

		public DateTime getExpiration()
		{
			return expiration;
		}

		public void setExpiration(DateTime expiration)
		{
			this.expiration = expiration;
		}

		public PinningMode getMode()
		{
			return mode;
		}

		public void setMode(PinningMode mode)
		{
			this.mode = mode;
		}
	}
}
