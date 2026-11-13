using System;
using System.Collections.Generic;

namespace AssemblyCSharpEditor
{
	public class ClientManifestsC
	{
		public ClientManifestsC (List<ClientPaths> cm)
		{
			this.paths = cm;
		}

		public List<ClientPaths> paths;
	}
}

