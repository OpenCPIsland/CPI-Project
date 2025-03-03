using DeviceDB;
using Disney.LaunchPadFramework;
using Disney.MobileNetwork;
using LitJson;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace ClubPenguin.Net.Client
{
	public class CPKeyValueDatabase : ICPKeyValueDatabase
	{
		private const string RsaParametersKey = "RsaParametersKey";

		private readonly byte[] databaseEncryptionKey;

		private readonly string localStorageDirPath;

		private readonly IDocumentCollectionFactory documentCollectionFactory;

		private RSAParameters? rsaParameters;

		public CPKeyValueDatabase(byte[] databaseEncryptionKey, string localStorageDirPath, IDocumentCollectionFactory documentCollectionFactory)
		{
			this.databaseEncryptionKey = databaseEncryptionKey;
			this.localStorageDirPath = localStorageDirPath;
			this.documentCollectionFactory = documentCollectionFactory;
		}

		public RSAParameters? GetRsaParameters()
		{
			if (!rsaParameters.HasValue)
			{
				CreateKeyValues(delegate(IDocumentCollection<KeyValueDocument> keyValues)
				{
					uint[] array = keyValues.FindDocumentIdsEqual(KeyValueDocument.KeyFieldName, "RsaParametersKey").ToArray();
					if (array.Length > 0)
					{
						uint documentId = array[0];
						KeyValueDocument keyValueDocument = keyValues.Find(documentId);
						string value = keyValueDocument.Value;
						rsaParameters = nullableFromJson<RSAParameters>(value);
					}
				});
			}
			return rsaParameters;
		}

		public void SetRsaParameters(RSAParameters rsaParameters)
		{
			this.rsaParameters = rsaParameters;
			CreateKeyValues(delegate(IDocumentCollection<KeyValueDocument> keyValues)
			{
				string value = toJson(rsaParameters);
				KeyValueDocument keyValueDocument = new KeyValueDocument
				{
					Key = "RsaParametersKey",
					Value = value
				};
				uint[] array = keyValues.FindDocumentIdsEqual(KeyValueDocument.KeyFieldName, "RsaParametersKey").ToArray();
				if (array.Length == 0)
				{
					keyValues.Insert(keyValueDocument);
				}
				else
				{
					keyValueDocument.Id = array[0];
					keyValues.Update(keyValueDocument);
				}
			});
		}

		private void CreateKeyValues(Action<IDocumentCollection<KeyValueDocument>> operation)
		{
			if (!Directory.Exists(localStorageDirPath))
			{
				Directory.CreateDirectory(localStorageDirPath);
			}
			try
			{
				using (IDocumentCollection<KeyValueDocument> obj = documentCollectionFactory.CreateHighSecurityFileSystemCollection<KeyValueDocument>(localStorageDirPath, databaseEncryptionKey))
				{
					operation(obj);
				}
			}
			catch
			{
				bool recovered = true;
				if (Directory.Exists(localStorageDirPath))
				{
					try
					{
						Directory.Delete(localStorageDirPath, true);
					}
					catch (Exception ex)
					{
						Log.LogErrorFormatted(this, "Unable to delete storage directory: {0}, {1}", localStorageDirPath, ex);
						recovered = false;
					}
				}
				Service.Get<EventDispatcher>().DispatchEvent(new CPKeyValueDatabaseErrorEvents.CorruptionErrorEvent(recovered));
			}
		}

		private string toJson<T>(T obj)
		{
			if (obj == null)
			{
				return "null";
			}
			return JsonMapper.ToJson(obj);
		}

		private T? nullableFromJson<T>(string json) where T : struct
		{
			if (json == null || json == "null")
			{
				return null;
			}
			return JsonMapper.ToObject<T>(json);
		}
	}
}
