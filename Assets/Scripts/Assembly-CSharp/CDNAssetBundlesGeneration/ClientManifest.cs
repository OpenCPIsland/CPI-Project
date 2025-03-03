using System;

namespace AssemblyCSharpEditor
{
	[Serializable]
	public class ClientManifest
	{
		public ClientManifest (string client, string platform, string environment, string content, bool useEmbeddedContent, string url, string abtest, string contentVersion, string releaseScheduleName, string releaseDate)
		{
			this.client = client;
			this.platform = platform;
			this.environment = environment;
			this.content = content;
			this.useEmbeddedContent = useEmbeddedContent;
			this.url = url;
			this.abtest = abtest;
			this.contentVersion = contentVersion;
			this.releaseScheduleName = releaseScheduleName;
			this.releaseDate = releaseDate;
			this.decryptionKey = null;
		}

		// Token: 0x0400022F RID: 559
		public string client { get; set; }

		// Token: 0x04000230 RID: 560
		public string platform { get; set; }

		// Token: 0x04000231 RID: 561
		public string environment { get; set; }

		// Token: 0x04000232 RID: 562
		public string content { get; set; }

		// Token: 0x04000233 RID: 563
		public bool useEmbeddedContent  { get; set; }

		// Token: 0x04000234 RID: 564
		public string url  { get; set; }

		// Token: 0x04000235 RID: 565
		public string abtest  { get; set; }

		// Token: 0x04000236 RID: 566
		public string contentVersion { get; set; }

		// Token: 0x04000237 RID: 567
		public string releaseScheduleName { get; set; }

		// Token: 0x04000238 RID: 568
		public string releaseDate { get; set; }

		// Token: 0x04000239 RID: 569
		public string decryptionKey  { get; set; }
	}
}

