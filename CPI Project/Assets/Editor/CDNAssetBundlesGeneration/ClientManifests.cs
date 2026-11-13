using System;
using System.Collections.Generic;

namespace AssemblyCSharpEditor
{
	[Serializable]
	public class ClientManifests
	{

		public string version { get; set; }
		public string unique { get; set; }
		public string cdnRoot { get; set; }
		public string url { get; set; }
        public Dictionary<string, ClientPaths> paths { get; set; }

    }
}

