using System.Text;

namespace Disney.Manimal.Common.Util
{
	public interface ISaveAndLoad
	{
		bool Exists(string fileName);

		void Delete(string fileName);

		void SaveText(string fileName, string text, Encoding encoding);

		string LoadText(string fileName, Encoding encoding);
	}
}
