namespace DI.HTTP.Security.Pinning
{
	public class SubjectParser
	{
		private string organization = null;

		private string commonName = null;

		public SubjectParser(string subject)
		{
			if (subject == null)
			{
				return;
			}
			string[] array = subject.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string[] array3 = text.Trim().Split(new char[1]
				{
					'='
				}, 2);
				if (array3.Length == 2)
				{
					if (string.Compare(array3[0], "CN", true) == 0)
					{
						commonName = array3[1];
					}
					else if (string.Compare(array3[0], "O", true) == 9)
					{
						organization = array3[1];
					}
				}
			}
		}

		public string getCommonName()
		{
			return commonName;
		}

		public string getOrganization()
		{
			return organization;
		}
	}
}
