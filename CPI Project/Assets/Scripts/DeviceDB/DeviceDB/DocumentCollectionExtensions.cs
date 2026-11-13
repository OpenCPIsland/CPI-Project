using System;
using System.Collections.Generic;

namespace DeviceDB
{
	public static class DocumentCollectionExtensions
	{
		public static IEnumerable<uint> FindDocumentIdsEqual<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) == 0);
		}

		public static IEnumerable<uint> FindDocumentIdsLessThan<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) < 0);
		}

		public static IEnumerable<uint> FindDocumentIdsLessThanOrEqualTo<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) <= 0);
		}

		public static IEnumerable<uint> FindDocumentIdsGreaterThan<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) > 0);
		}

		public static IEnumerable<uint> FindDocumentIdsGreaterThanOrEqualTo<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) >= 0);
		}

		public static IEnumerable<uint> FindDocumentIdsRangeInclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) >= 0 && NullSafeComparer.Compare(v, endValue) <= 0);
		}

		public static IEnumerable<uint> FindDocumentIdsRangeExclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) > 0 && NullSafeComparer.Compare(v, endValue) < 0);
		}

		public static IEnumerable<uint> FindDocumentIdsRangeInclusiveExclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) >= 0 && NullSafeComparer.Compare(v, endValue) < 0);
		}

		public static IEnumerable<uint> FindDocumentIdsRangeExclusiveInclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocumentIds(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) > 0 && NullSafeComparer.Compare(v, endValue) <= 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsEqual<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) == 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsLessThan<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) < 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsLessThanOrEqualTo<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) <= 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsGreaterThan<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) > 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsGreaterThanOrEqualTo<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField value) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, value, (TField v) => NullSafeComparer.Compare(v, value) >= 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsRangeInclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) >= 0 && NullSafeComparer.Compare(v, endValue) <= 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsRangeExclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) > 0 && NullSafeComparer.Compare(v, endValue) < 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsRangeInclusiveExclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) >= 0 && NullSafeComparer.Compare(v, endValue) < 0);
		}

		public static IEnumerable<KeyValuePair<uint, TField>> FindDocumentsRangeExclusiveInclusive<TDocument, TField>(this IDocumentCollection<TDocument> collection, string fieldName, TField startValue, TField endValue) where TDocument : AbstractDocument, new()where TField : IComparable<TField>
		{
			return collection.FindDocuments(fieldName, startValue, (TField v) => NullSafeComparer.Compare(v, startValue) > 0 && NullSafeComparer.Compare(v, endValue) <= 0);
		}
	}
}
