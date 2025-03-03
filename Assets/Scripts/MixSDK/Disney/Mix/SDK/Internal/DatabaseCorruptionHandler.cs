using DeviceDB;
using System;
using System.Collections.Generic;

namespace Disney.Mix.SDK.Internal
{
	public class DatabaseCorruptionHandler
	{
		private readonly AbstractLogger logger;

		private readonly Dictionary<object, Action> deleters;

		private readonly IFileSystem fileSystem;

		private readonly string sdkStorageDirPath;

		public event EventHandler<CorruptionDetectedEventArgs> OnCorruptionDetected = delegate
		{
		};

		public DatabaseCorruptionHandler(AbstractLogger logger, IFileSystem fileSystem, string sdkStorageDirPath)
		{
			this.logger = logger;
			this.fileSystem = fileSystem;
			this.sdkStorageDirPath = sdkStorageDirPath;
			deleters = new Dictionary<object, Action>();
		}

		public void Add<TDocument>(IDocumentCollection<TDocument> collection) where TDocument : AbstractDocument
		{
			deleters[collection] = collection.Delete;
		}

		public void Remove<TDocument>(IDocumentCollection<TDocument> collection) where TDocument : AbstractDocument
		{
			deleters.Remove(collection);
		}

		public void HandleCorruption(CorruptionException ex)
		{
			logger.Fatal("Corruption detected: " + ex);
			bool recovered = true;
			foreach (KeyValuePair<object, Action> deleter in deleters)
			{
				deleter.Value();
			}
			if (fileSystem.DirectoryExists(sdkStorageDirPath))
			{
				try
				{
					fileSystem.DeleteDirectory(sdkStorageDirPath);
				}
				catch (Exception ex2)
				{
					logger.Fatal("Unable to delete storage directory: " + sdkStorageDirPath + ", " + ex2);
					recovered = false;
				}
			}
			this.OnCorruptionDetected(this, new CorruptionDetectedEventArgs(recovered));
		}
	}
}
