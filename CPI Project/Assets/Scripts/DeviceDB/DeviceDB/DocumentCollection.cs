using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace DeviceDB
{
	internal class DocumentCollection<TDocument> : IDocumentCollection<TDocument>, IEnumerable<TDocument>, IDisposable, IEnumerable where TDocument : AbstractDocument, new()
	{
		private readonly PackedFile packedFile;

		private readonly JournalPlayer journalPlayer;

		private readonly JournalWriter journalWriter;

		private readonly FieldIndexes<TDocument> fieldIndexes;

		private readonly object transactionLockObject;

		private Aes256Encryptor encryptor;

		private uint metadataDocumentId;

		private FieldInfo[] fields;

		private bool isFieldIndexesInUse;

		private bool isDisposed;

		private Type documentType;

		internal FieldIndexes<TDocument> FieldIndexes
		{
			get
			{
				return fieldIndexes;
			}
		}

		public LogHandler DebugLogHandler
		{
			get;
			set;
		}

		public IEnumerable<string> MissingIndexedFieldNames
		{
			get
			{
				lock (transactionLockObject)
				{
					if (isDisposed)
					{
						throw new ObjectDisposedException("Can't use MissingIndexedFieldNames after Dispose() or Delete()");
					}
					if (isFieldIndexesInUse)
					{
						throw new InvalidOperationException("Can't use MissingIndexedFieldNames before disposing the enumerable  from a FindDocumentIds call");
					}
					if (DebugLogHandler != null)
					{
						DebugLogHandler("MissingIndexedFieldNames: get");
					}
					return fieldIndexes.GetMissingIndexedFieldNames(packedFile.Count - 1);
				}
			}
		}

		public DocumentCollection(PackedFile packedFile, IndexFactory indexFactory, byte[] key, JournalPlayer journalPlayer, JournalWriter journalWriter)
		{
			transactionLockObject = new object();
			EnsureValidKey(key);
			documentType = typeof(TDocument);
			try
			{
				journalPlayer.Play();
				SerializerReflectionCache.AddTypes(typeof(MetadataDocument), documentType);
				this.packedFile = packedFile;
				this.journalPlayer = journalPlayer;
				this.journalWriter = journalWriter;
				byte[] initializationVector;
				if (packedFile.IsEmpty)
				{
					initializationVector = CryptoRandomNumberGenerator.GenerateBytes(16u);
					journalWriter.Start();
					WriteMetadataDocument(initializationVector);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				else
				{
					KeyValuePair<uint, byte[]> keyValuePair = packedFile.Documents.First();
					metadataDocumentId = keyValuePair.Key;
					MetadataDocument metadataDocument = BinarySerializer.Deserialize<MetadataDocument>(keyValuePair.Value);
					initializationVector = metadataDocument.InitializationVector;
				}
				encryptor = new Aes256Encryptor(key, initializationVector);
				fieldIndexes = new FieldIndexes<TDocument>(indexFactory, encryptor);
			}
			catch (Exception)
			{
				journalWriter.Discard();
				throw;
			}
		}

		public void ChangeKey(byte[] key)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use ChangeKey() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use ChangeKey() before disposing the enumerable from a FindDocumentIds call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("ChangeKey: key=" + ((key != null) ? Convert.ToBase64String(key) : "null"));
				}
				EnsureValidKey(key);
				byte[] initializationVector = CryptoRandomNumberGenerator.GenerateBytes(16u);
				Aes256Encryptor aes256Encryptor = new Aes256Encryptor(key, initializationVector);
				journalWriter.Start();
				try
				{
					foreach (KeyValuePair<uint, byte[]> document2 in packedFile.Documents)
					{
						if (document2.Key != metadataDocumentId)
						{
							byte[] value = document2.Value;
							TDocument val = DecryptAndDeserialize(value, document2.Key);
							byte[] document = SerializeAndEncrypt(val, aes256Encryptor);
							packedFile.Update(val.Id, document);
						}
					}
					WriteMetadataDocument(initializationVector);
					encryptor = aes256Encryptor;
					fieldIndexes.ChangeEncryptor(encryptor);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		public void Insert(TDocument document)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Insert() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use Insert() before disposing the enumerable from a FindDocumentIds call");
				}
				if (document == null)
				{
					if (DebugLogHandler != null)
					{
						DebugLogHandler("Insert: null");
					}
					throw new ArgumentException("Document to insert can't be null", "document");
				}
				if (DebugLogHandler != null)
				{
					if (fields == null)
					{
						fields = documentType.GetFields();
					}
					StringBuilder stringBuilder = new StringBuilder("Insert: document=\n");
					FieldInfo[] array = fields;
					foreach (FieldInfo fieldInfo in array)
					{
						stringBuilder.Append(fieldInfo.Name);
						stringBuilder.Append(" = ");
						stringBuilder.Append(fieldInfo.GetValue(document));
						stringBuilder.Append('\n');
					}
					DebugLogHandler(stringBuilder.ToString());
				}
				if (document.Id != 0)
				{
					throw new ArgumentException("Document already has an ID. Update Update() instead.");
				}
				byte[] document2 = SerializeAndEncrypt(document);
				journalWriter.Start();
				try
				{
					document.Id = packedFile.Insert(document2);
					fieldIndexes.Insert(document);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		public void Update(TDocument document)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Update() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use Update() before disposing the enumerable from a FindDocumentIds call");
				}
				if (document == null)
				{
					if (DebugLogHandler != null)
					{
						DebugLogHandler("Update: null");
					}
					throw new ArgumentException("Can't update a null document");
				}
				if (DebugLogHandler != null)
				{
					if (fields == null)
					{
						fields = documentType.GetFields();
					}
					StringBuilder stringBuilder = new StringBuilder("Update: document=\n");
					FieldInfo[] array = fields;
					foreach (FieldInfo fieldInfo in array)
					{
						stringBuilder.Append(fieldInfo.Name);
						stringBuilder.Append(" = ");
						stringBuilder.Append(fieldInfo.GetValue(document));
						stringBuilder.Append('\n');
					}
					DebugLogHandler(stringBuilder.ToString());
				}
				if (document.Id == metadataDocumentId)
				{
					throw new ArgumentException("Document " + document.Id + " not found");
				}
				byte[] array2 = packedFile.Find(document.Id);
				if (array2 == null)
				{
					throw new ArgumentException("Document " + document.Id + " not found");
				}
				TDocument oldDocument = DecryptAndDeserialize(array2, document.Id);
				byte[] document2 = SerializeAndEncrypt(document);
				journalWriter.Start();
				try
				{
					document.Id = packedFile.Update(document.Id, document2);
					fieldIndexes.Update(document, oldDocument);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		public void Delete(uint documentId)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Delete(id) after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use Delete(id) before disposing the enumerable from a FindDocumentIds call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("Delete: documentId=" + documentId);
				}
				if (documentId == metadataDocumentId)
				{
					throw new ArgumentException("Document " + documentId + " not found");
				}
				TDocument val = Find(documentId);
				if (val == null)
				{
					throw new ArgumentException("Document " + documentId + " not found");
				}
				journalWriter.Start();
				try
				{
					packedFile.Remove(documentId);
					fieldIndexes.Delete(val);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		public void Delete()
		{
			lock (transactionLockObject)
			{
				if (!isDisposed)
				{
					isDisposed = true;
					if (DebugLogHandler != null)
					{
						DebugLogHandler("Delete: whole collection");
					}
					packedFile.Delete();
					fieldIndexes.Delete();
					journalWriter.Delete();
				}
			}
		}

		public void Drop()
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Drop() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use Drop() before disposing the enumerable from a FindDocumentIds call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("Drop");
				}
				try
				{
					journalWriter.Start();
					packedFile.JournaledClear();
					fieldIndexes.JournaledClear();
					journalWriter.Finish();
					journalPlayer.Play();
					metadataDocumentId = 0u;
					journalWriter.Start();
					WriteMetadataDocument(encryptor.InitializationVector);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		public bool Contains(uint documentId)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Contains() after Dispose() or Delete()");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("Contains: documentId=" + documentId);
				}
				return documentId != metadataDocumentId && packedFile.Contains(documentId);
			}
		}

		public TDocument Find(uint documentId)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use Find() after Dispose() or Delete()");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("Find: documentId=" + documentId);
				}
				if (documentId == metadataDocumentId)
				{
					return (TDocument)null;
				}
				byte[] array = packedFile.Find(documentId);
				return (array != null) ? DecryptAndDeserialize(array, documentId) : ((TDocument)null);
			}
		}

		public IEnumerable<uint> FindDocumentIds<TField>(string fieldName, TField value, Predicate<TField> matcher) where TField : IComparable<TField>
		{
			object obj = transactionLockObject;
			Monitor.Enter(obj);
			try
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocumentIds() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocumentIds() before disposing the enumerable from a previous FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler(string.Concat("FindDocumentIds: fieldName=", fieldName, ", value=", value, ", matcher=", matcher));
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				isFieldIndexesInUse = true;
				try
				{
					foreach (KeyValuePair<uint, TField> item in index.Find(value, matcher))
					{
						yield return item.Key;
						if (isDisposed)
						{
							break;
						}
					}
				}
				finally
				{
                    this.isFieldIndexesInUse = false; //base._003C_003E__Finally0();
				}
			}
			finally
			{
                this.isFieldIndexesInUse = false; //base._003C_003E__Finally1();
            }
		}

		public uint? FindDocumentIdMax<TField>(string fieldName) where TField : IComparable<TField>
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocumentIdMax() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocumentIdMax() before disposing the enumerable from a FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("FindDocumentIdMax: fieldName=" + fieldName);
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				return index.FindMaxId();
			}
		}

		public uint? FindDocumentIdMin<TField>(string fieldName) where TField : IComparable<TField>
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocumentIdMin() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocumentIdMin() before disposing the enumerable from a FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("FindDocumentIdMin: fieldName=" + fieldName);
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				return index.FindMinId();
			}
		}

		public IEnumerable<KeyValuePair<uint, TField>> FindDocuments<TField>(string fieldName, TField value, Predicate<TField> matcher) where TField : IComparable<TField>
		{
			object obj = transactionLockObject;
			Monitor.Enter(obj);
			try
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocuments() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocuments() before disposing the enumerable from a previous FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler(string.Concat("FindDocuments: fieldName=", fieldName, ", value=", value, ", matcher=", matcher));
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				isFieldIndexesInUse = true;
				try
				{
					foreach (KeyValuePair<uint, TField> item in index.Find(value, matcher))
					{
						yield return item;
						if (isDisposed)
						{
							break;
						}
					}
				}
				finally
				{
                    this.isFieldIndexesInUse = false; //base._003C_003E__Finally0();
                }
			}
			finally
			{
                this.isFieldIndexesInUse = false; //base._003C_003E__Finally1();
            }
		}

		public KeyValuePair<uint, TField>? FindDocumentMax<TField>(string fieldName) where TField : IComparable<TField>
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocumentMax() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocumentMax() before disposing the enumerable from a FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("FindDocumentMax: fieldName=" + fieldName);
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				return index.FindMax();
			}
		}

		public KeyValuePair<uint, TField>? FindDocumentMin<TField>(string fieldName) where TField : IComparable<TField>
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use FindDocumentMin() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use FindDocumentMin() before disposing the enumerable from a FindDocumentIds or FindDocuments call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("FindDocumentMin: fieldName=" + fieldName);
				}
				Index<TField> index = fieldIndexes.GetIndex<TField>(fieldName);
				return index.FindMin();
			}
		}

		IEnumerator<TDocument> IEnumerable<TDocument>.GetEnumerator()
		{
			object obj = transactionLockObject;
			Monitor.Enter(obj);
			try
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use GetEnumerator() or foreach after Dispose() or Delete()");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("GetEnumerator<TDocument>");
				}
				foreach (KeyValuePair<uint, byte[]> pair in packedFile.Documents)
				{
					if (pair.Key != metadataDocumentId)
					{
						yield return DecryptAndDeserialize(pair.Value, pair.Key);
					}
				}
			}
			finally
			{
                this.isFieldIndexesInUse = false; //base._003C_003E__Finally0();
            }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			object obj = transactionLockObject;
			Monitor.Enter(obj);
			try
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use GetEnumerator() or foreach after Dispose() or Delete()");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("GetEnumerator");
				}
				foreach (KeyValuePair<uint, byte[]> pair in packedFile.Documents)
				{
					if (pair.Key != metadataDocumentId)
					{
						yield return DecryptAndDeserialize(pair.Value, pair.Key);
					}
				}
			}
			finally
			{
                this.isFieldIndexesInUse = false; //base._003C_003E__Finally0();
            }
		}

		public void Dispose()
		{
			lock (transactionLockObject)
			{
				if (!isDisposed)
				{
					isDisposed = true;
					if (DebugLogHandler != null)
					{
						DebugLogHandler("Dispose");
					}
					packedFile.Dispose();
					fieldIndexes.Dispose();
					journalWriter.Dispose();
					journalPlayer.Dispose();
				}
			}
		}

		public void IndexField(string fieldName)
		{
			lock (transactionLockObject)
			{
				if (isDisposed)
				{
					throw new ObjectDisposedException("Can't use IndexField() after Dispose() or Delete()");
				}
				if (isFieldIndexesInUse)
				{
					throw new InvalidOperationException("Can't use IndexField() before disposing the enumerable from a FindDocumentIds call");
				}
				if (DebugLogHandler != null)
				{
					DebugLogHandler("IndexField: fieldName=" + fieldName);
				}
				journalWriter.Start();
				try
				{
					fieldIndexes.ClearField(fieldName);
					List<KeyValuePair<uint, IComparable>> list = new List<KeyValuePair<uint, IComparable>>();
					foreach (KeyValuePair<uint, byte[]> document2 in packedFile.Documents)
					{
						if (document2.Key != metadataDocumentId)
						{
							TDocument document = DecryptAndDeserialize(document2.Value, document2.Key);
							IComparable fieldValue = fieldIndexes.GetFieldValue(fieldName, document);
							list.Add(new KeyValuePair<uint, IComparable>(document2.Key, fieldValue));
						}
					}
					list.Sort((KeyValuePair<uint, IComparable> a, KeyValuePair<uint, IComparable> b) => NullSafeComparer.Compare(a.Value, b.Value));
					fieldIndexes.InsertPreSorted(fieldName, list);
					journalWriter.Finish();
					journalPlayer.Play();
				}
				catch (Exception)
				{
					journalWriter.Discard();
					throw;
				}
			}
		}

		private byte[] SerializeAndEncrypt(TDocument document)
		{
			return SerializeAndEncrypt(document, encryptor);
		}

		private byte[] SerializeAndEncrypt(TDocument document, Aes256Encryptor encryptor)
		{
			byte[] bytes = BinarySerializer.Serialize(document, documentType);
			return encryptor.Encrypt(bytes);
		}

		private TDocument DecryptAndDeserialize(byte[] inBytes, uint documentId)
		{
			byte[] bytes = encryptor.Decrypt(inBytes);
			TDocument val = BinarySerializer.Deserialize<TDocument>(bytes);
			val.Id = documentId;
			return val;
		}

		private void WriteMetadataDocument(byte[] initializationVector)
		{
			MetadataDocument metadataDocument = new MetadataDocument();
			metadataDocument.InitializationVector = initializationVector;
			MetadataDocument obj = metadataDocument;
			byte[] document = BinarySerializer.Serialize(obj, typeof(MetadataDocument));
			metadataDocumentId = ((metadataDocumentId == 0) ? packedFile.Insert(document) : packedFile.Update(metadataDocumentId, document));
		}

		private static void EnsureValidKey(byte[] key)
		{
			if (key == null || key.Length != 32)
			{
				throw new ArgumentException("key is null or the wrong size");
			}
		}
	}
}
