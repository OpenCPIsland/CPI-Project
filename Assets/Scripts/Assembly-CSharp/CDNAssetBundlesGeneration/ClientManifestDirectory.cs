using System;
using System.Collections.Generic;

namespace AssemblyCSharpEditor
{
	public class ClientManifestDirectory
	{
		public ClientManifestDirectory (List<ClientManifest> cm)
		{
			this.directory = cm;
		}

		public List<ClientManifest> directory;
	}
}

