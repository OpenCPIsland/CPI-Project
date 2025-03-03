using System;

namespace DI.CMS.FileManagement
{
	public interface IFMSListener
	{
		void OnManifestLoadSuccess();

		void OnManifestLoadFailure(Exception cause);

		void OnManifestLoadComplete();
	}
}
