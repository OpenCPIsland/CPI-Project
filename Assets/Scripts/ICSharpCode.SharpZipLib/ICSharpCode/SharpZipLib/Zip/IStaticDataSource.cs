using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public interface IStaticDataSource
	{
		Stream GetSource();
	}
}
