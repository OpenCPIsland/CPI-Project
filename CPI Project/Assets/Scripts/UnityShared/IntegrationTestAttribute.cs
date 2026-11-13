using System;
using System.IO;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class IntegrationTestAttribute : Attribute
{
	private readonly string m_Path;

	public IntegrationTestAttribute(string path)
	{
		if (path.EndsWith(".unity"))
		{
			path = path.Substring(0, path.Length - ".unity".Length);
		}
		m_Path = path;
	}

	public bool IncludeOnScene(string scenePath)
	{
		if (scenePath == m_Path)
		{
			return true;
		}
		string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(scenePath);
		return fileNameWithoutExtension == m_Path;
	}
}
