using DI.JSON;
using DI.Storage;
using DI.Storage.FileSystemStorage;

namespace DI.HTTP.Security.Pinning
{
	public class DefaultPinsetFactory : IPinsetFactory
	{
		private static DefaultPinsetFactory factory = null;

		private static IStorage _storage = null;

		private static IJSONParser _parser = null;

		private IStorage storage;

		private IJSONParser parser;

		private IPinset defaultPinset;

		public static IPinsetFactory getFactory()
		{
			if (factory == null)
			{
				if (_parser == null)
				{
					IJSONParser iJSONParser = new BasicJSONParser();
					setParser(iJSONParser);
				}
				if (_storage == null)
				{
					FileSystemStorageFactory.setParser(_parser);
					IStorage storage = FileSystemStorageFactory.getFactory().getStorage();
					setStorage(storage);
				}
				factory = new DefaultPinsetFactory(_storage, _parser);
			}
			return factory;
		}

		public static void setStorage(IStorage storage)
		{
			if (_storage != null)
			{
				throw new PinningException("IStorage interface has already been initialized.");
			}
			_storage = storage;
		}

		public static void setParser(IJSONParser parser)
		{
			if (_parser != null)
			{
				throw new PinningException("IJSONParser interface has already been initialized.");
			}
			_parser = parser;
		}

		public DefaultPinsetFactory(IStorage storage, IJSONParser parser)
		{
			this.storage = storage;
			this.parser = parser;
			loadDefaultPinset();
		}

		public IStorage getStorage()
		{
			return storage;
		}

		public IJSONParser getParser()
		{
			return parser;
		}

		public void loadDefaultPinset()
		{
			defaultPinset = new AssetPinset("pinset", getParser());
		}

		public IPinset getPinset()
		{
			return defaultPinset;
		}
	}
}
