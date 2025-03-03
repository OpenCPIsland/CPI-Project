using System;
using System.Collections;
using System.Collections.Generic;

namespace DeviceDB
{
	public interface IDocumentCollection<TDocument> : IEnumerable<TDocument>, IDisposable, IEnumerable where TDocument : AbstractDocument
	{
		LogHandler DebugLogHandler
		{
			get;
			set;
		}

		IEnumerable<string> MissingIndexedFieldNames
		{
			get;
		}

		void Insert(TDocument document);

		void Update(TDocument document);

		void Delete(uint documentId);

		void Delete();

		void Drop();

		bool Contains(uint documentId);

		TDocument Find(uint documentId);

		IEnumerable<uint> FindDocumentIds<TField>(string fieldName, TField value, Predicate<TField> matcher) where TField : IComparable<TField>;

		uint? FindDocumentIdMax<TField>(string fieldName) where TField : IComparable<TField>;

		uint? FindDocumentIdMin<TField>(string fieldName) where TField : IComparable<TField>;

		IEnumerable<KeyValuePair<uint, TField>> FindDocuments<TField>(string fieldName, TField value, Predicate<TField> matcher) where TField : IComparable<TField>;

		KeyValuePair<uint, TField>? FindDocumentMax<TField>(string fieldName) where TField : IComparable<TField>;

		KeyValuePair<uint, TField>? FindDocumentMin<TField>(string fieldName) where TField : IComparable<TField>;

		void ChangeKey(byte[] key);

		void IndexField(string fieldName);
	}
}
