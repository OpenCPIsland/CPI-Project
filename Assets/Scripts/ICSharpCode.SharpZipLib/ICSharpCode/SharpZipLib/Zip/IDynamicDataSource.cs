using System.IO;

namespace ICSharpCode.SharpZipLib.Zip
{
	public interface IDynamicDataSource
	{
		Stream GetSource(ZipEntry entry, string name);
	}
}
