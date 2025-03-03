public struct StructHash
{
	private uint hash;

	private uint count;

	public void Combine<T>(T obj)
	{
		uint hashCode = (uint)obj.GetHashCode();
		hashCode = (uint)((int)hashCode * -862048943);
		hashCode = ((hashCode << 15) | (hashCode >> 17));
		hashCode *= 461845907;
		foldIn(hashCode);
	}

	private void foldIn(uint k)
	{
		hash ^= k;
		hash = ((hash << 13) | (hash >> 19));
		hash = (uint)((int)(hash * 5) + -430675100);
		count++;
	}

	public void Combine<T>(T[] obj)
	{
		if (obj != null)
		{
			for (int i = 0; i < obj.Length; i++)
			{
				Combine(obj[i]);
			}
		}
		else
		{
			foldIn(0u);
		}
	}

	public static implicit operator int(StructHash sh)
	{
		uint num = sh.hash ^ sh.count;
		num ^= num >> 16;
		num = (uint)((int)num * -2048144789);
		num ^= num >> 13;
		num = (uint)((int)num * -1028477387);
		return (int)(num ^ (num >> 16));
	}
}
