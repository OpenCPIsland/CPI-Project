using System;
using System.IO;

namespace ICSharpCode.SharpZipLib.Core
{
	public class FileSystemScanner
	{
		public ProcessDirectoryHandler ProcessDirectory;

		public ProcessFileHandler ProcessFile;

		public CompletedFileHandler CompletedFile;

		public DirectoryFailureHandler DirectoryFailure;

		public FileFailureHandler FileFailure;

		private IScanFilter fileFilter_;

		private IScanFilter directoryFilter_;

		private bool alive_;

		public FileSystemScanner(string filter)
		{
			fileFilter_ = new PathFilter(filter);
		}

		public FileSystemScanner(string fileFilter, string directoryFilter)
		{
			fileFilter_ = new PathFilter(fileFilter);
			directoryFilter_ = new PathFilter(directoryFilter);
		}

		public FileSystemScanner(IScanFilter fileFilter)
		{
			fileFilter_ = fileFilter;
		}

		public FileSystemScanner(IScanFilter fileFilter, IScanFilter directoryFilter)
		{
			fileFilter_ = fileFilter;
			directoryFilter_ = directoryFilter;
		}

		private bool OnDirectoryFailure(string directory, Exception e)
		{
			DirectoryFailureHandler directoryFailure = DirectoryFailure;
			bool flag = directoryFailure != null;
			if (flag)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(directory, e);
				directoryFailure(this, scanFailureEventArgs);
				alive_ = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		private bool OnFileFailure(string file, Exception e)
		{
			FileFailureHandler fileFailure = FileFailure;
			bool flag = fileFailure != null;
			if (flag)
			{
				ScanFailureEventArgs scanFailureEventArgs = new ScanFailureEventArgs(file, e);
				FileFailure(this, scanFailureEventArgs);
				alive_ = scanFailureEventArgs.ContinueRunning;
			}
			return flag;
		}

		private void OnProcessFile(string file)
		{
			ProcessFileHandler processFile = ProcessFile;
			if (processFile != null)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				processFile(this, scanEventArgs);
				alive_ = scanEventArgs.ContinueRunning;
			}
		}

		private void OnCompleteFile(string file)
		{
			CompletedFileHandler completedFile = CompletedFile;
			if (completedFile != null)
			{
				ScanEventArgs scanEventArgs = new ScanEventArgs(file);
				completedFile(this, scanEventArgs);
				alive_ = scanEventArgs.ContinueRunning;
			}
		}

		private void OnProcessDirectory(string directory, bool hasMatchingFiles)
		{
			ProcessDirectoryHandler processDirectory = ProcessDirectory;
			if (processDirectory != null)
			{
				DirectoryEventArgs directoryEventArgs = new DirectoryEventArgs(directory, hasMatchingFiles);
				processDirectory(this, directoryEventArgs);
				alive_ = directoryEventArgs.ContinueRunning;
			}
		}

		public void Scan(string directory, bool recurse)
		{
			alive_ = true;
			ScanDir(directory, recurse);
		}

		private void ScanDir(string directory, bool recurse)
		{
			try
			{
				string[] files = Directory.GetFiles(directory);
				bool flag = false;
				for (int i = 0; i < files.Length; i++)
				{
					if (!fileFilter_.IsMatch(files[i]))
					{
						files[i] = null;
					}
					else
					{
						flag = true;
					}
				}
				OnProcessDirectory(directory, flag);
				if (alive_ && flag)
				{
					string[] array = files;
					foreach (string text in array)
					{
						try
						{
							if (text != null)
							{
								OnProcessFile(text);
								if (!alive_)
								{
									goto IL_0098;
								}
							}
						}
						catch (Exception e)
						{
							if (!OnFileFailure(text, e))
							{
								throw;
							}
						}
					}
				}
			}
			catch (Exception e2)
			{
				if (!OnDirectoryFailure(directory, e2))
				{
					throw;
				}
			}
			goto IL_0098;
			IL_0098:
			if (alive_ && recurse)
			{
				try
				{
					string[] directories = Directory.GetDirectories(directory);
					string[] array2 = directories;
					foreach (string text2 in array2)
					{
						if (directoryFilter_ == null || directoryFilter_.IsMatch(text2))
						{
							ScanDir(text2, true);
							if (!alive_)
							{
								break;
							}
						}
					}
				}
				catch (Exception e3)
				{
					if (!OnDirectoryFailure(directory, e3))
					{
						throw;
					}
				}
			}
		}
	}
}
