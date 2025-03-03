using System;

namespace ICSharpCode.SharpZipLib.Zip
{
	[Flags]
	public enum GeneralBitFlags
	{
		Encrypted = 0x1,
		Method = 0x6,
		Descriptor = 0x8,
		ReservedPKware4 = 0x10,
		Patched = 0x20,
		StrongEncryption = 0x40,
		Unused7 = 0x80,
		Unused8 = 0x100,
		Unused9 = 0x200,
		Unused10 = 0x400,
		UnicodeText = 0x800,
		EnhancedCompress = 0x1000,
		HeaderMasked = 0x2000,
		ReservedPkware14 = 0x4000,
		ReservedPkware15 = 0x8000
	}
}
