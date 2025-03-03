using Disney.Kelowna.Common;
using Disney.Kelowna.Common.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClubPenguin.Core
{
	[Serializable]
	public class SceneLayoutData : ScopedData
	{
		public class LayoutEnumerator : IEnumerable<DecorationLayoutData>, IEnumerable
		{
			private IEnumerator<DecorationLayoutData> enumerator;

			public LayoutEnumerator(IEnumerator<DecorationLayoutData> enumerator)
			{
				this.enumerator = enumerator;
			}

			public IEnumerator<DecorationLayoutData> GetEnumerator()
			{
				return enumerator;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public interface OrderedLayoutEnumerator : IEnumerable<ParentedDecorationData>, IEnumerable
		{
		}

		private class OrderedLayoutEnumeratorImpl : OrderedLayoutEnumerator, IEnumerable<ParentedDecorationData>, IEnumerable
		{
			private readonly ParentedSet<string, DecorationLayoutData> layout;

			private readonly string elementKey;

			public OrderedLayoutEnumeratorImpl(ParentedSet<string, DecorationLayoutData> layout)
			{
				this.layout = layout;
			}

			public OrderedLayoutEnumeratorImpl(ParentedSet<string, DecorationLayoutData> layout, string elementKey)
				: this(layout)
			{
				this.elementKey = elementKey;
			}

			public IEnumerator<ParentedDecorationData> GetEnumerator()
			{
				if (string.IsNullOrEmpty(elementKey))
				{
					foreach (string key in layout.Roots)
					{
						yield return getParentedDecorationData(key);
					}
				}
				else
				{
					yield return getParentedDecorationData(elementKey);
				}
			}

			private ParentedDecorationData getParentedDecorationData(string key)
			{
				ParentedDecorationData parentedDecorationData = new ParentedDecorationData(layout.Get(key));
				foreach (string child in layout.GetChildren(key))
				{
					parentedDecorationData.Children.Add(new OrderedLayoutEnumeratorImpl(layout, child));
				}
				return parentedDecorationData;
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public class ParentedDecorationData
		{
			public DecorationLayoutData Data;

			public List<OrderedLayoutEnumerator> Children;

			public ParentedDecorationData(DecorationLayoutData decorationLayoutData)
			{
				Data = decorationLayoutData;
				Children = new List<OrderedLayoutEnumerator>();
			}
		}

		protected class ParentedSet<TKey, TValue> : IEnumerable<TValue>, IEnumerable
		{
			private class Node
			{
				public readonly List<TKey> children;

				public TValue value;

				public Node(TValue value)
				{
					this.value = value;
					children = new List<TKey>();
				}
			}

			private readonly Dictionary<TKey, Node> dictionary;

			private readonly Dictionary<TKey, List<TKey>> roots;

			private readonly List<TKey> nullRoots;

			public IEnumerable<TKey> Roots
			{
				get
				{
					foreach (TKey nullRoot in nullRoots)
					{
						yield return nullRoot;
					}
					foreach (List<TKey> root in roots.Values)
					{
						foreach (TKey item in root)
						{
							yield return item;
						}
					}
				}
			}

			internal int Count
			{
				get
				{
					return dictionary.Count;
				}
			}

			internal ParentedSet()
			{
				dictionary = new Dictionary<TKey, Node>();
				roots = new Dictionary<TKey, List<TKey>>();
				nullRoots = new List<TKey>();
			}

			public IEnumerator<TValue> GetEnumerator()
			{
				foreach (Node node in dictionary.Values)
				{
					yield return node.value;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			internal void Add(TKey parentKey, TKey key, TValue value)
			{
				addToParent(parentKey, key);
				Node node = new Node(value);
				dictionary.Add(key, node);
				if (roots.ContainsKey(key))
				{
					node.children.AddRange(roots[key]);
					roots.Remove(key);
				}
			}

			internal List<TKey> GetChildren(TKey key)
			{
				return dictionary[key].children;
			}

			internal bool ContainsKey(TKey key)
			{
				return dictionary.ContainsKey(key);
			}

			internal TValue Get(TKey key)
			{
				return dictionary[key].value;
			}

			internal bool Contains(TKey key)
			{
				return dictionary.ContainsKey(key);
			}

			internal void Remove(TKey parentKey, TKey key, bool removeChildren)
			{
				if (!dictionary.ContainsKey(key))
				{
					return;
				}
				Node node = dictionary[key];
				if (removeChildren)
				{
					TKey[] array = new TKey[node.children.Count];
					node.children.CopyTo(array);
					for (int i = 0; i < array.Length; i++)
					{
						Remove(key, array[i], true);
					}
				}
				else if (node.children.Count > 0)
				{
					roots.Add(key, node.children);
				}
				removeFromParent(parentKey, key);
				dictionary.Remove(key);
			}

			internal void Update(TKey oldParentKey, TKey oldKey, TKey newParentKey, TKey newKey, TValue value)
			{
				Node node = dictionary[oldKey];
				node.value = value;
				bool flag = (oldParentKey == null && newParentKey != null) || (oldParentKey != null && !oldParentKey.Equals(newParentKey));
				bool flag2 = !oldKey.Equals(newKey);
				if (flag || flag2)
				{
					removeFromParent(oldParentKey, oldKey);
					addToParent(newParentKey, newKey);
				}
				if (flag2)
				{
					dictionary.Remove(oldKey);
					dictionary[newKey] = node;
					List<TKey> value2;
					if (roots.TryGetValue(newKey, out value2))
					{
						node.children.AddRange(value2);
						roots.Remove(newKey);
					}
				}
			}

			private void addToParent(TKey parentKey, TKey key)
			{
				Node value;
				if (parentKey != null && dictionary.TryGetValue(parentKey, out value))
				{
					value.children.Add(key);
					return;
				}
				if (parentKey == null)
				{
					nullRoots.Add(key);
					return;
				}
				if (!roots.ContainsKey(parentKey))
				{
					roots.Add(parentKey, new List<TKey>());
				}
				roots[parentKey].Add(key);
			}

			private void removeFromParent(TKey parentKey, TKey key)
			{
				Node value;
				if (parentKey != null && dictionary.TryGetValue(parentKey, out value))
				{
					dictionary[parentKey].children.Remove(key);
					return;
				}
				if (parentKey == null)
				{
					nullRoots.Remove(key);
					return;
				}
				List<TKey> list = roots[parentKey];
				list.Remove(key);
				if (list.Count == 0)
				{
					roots.Remove(parentKey);
				}
			}
		}

		private string lotZoneName;

		private int lightingId;

		private int musicTrackId;

		private Dictionary<string, string> extraInfo;

		private ParentedSet<string, DecorationLayoutData> layout = new ParentedSet<string, DecorationLayoutData>();

		public long LayoutId;

		public bool IsDirty;

		public int MaxLayoutItems;

		public bool ItemLimitWarningShown = false;

		public long CreatedDate;

		public long LastModifiedDate;

		public bool MemberOnly;

		public string LotZoneName
		{
			get
			{
				return lotZoneName;
			}
			set
			{
				lotZoneName = value;
				dispatchSceneLayoutDataUpdated();
			}
		}

		public int LightingId
		{
			get
			{
				return lightingId;
			}
			set
			{
				lightingId = value;
				if (this.LightingIdUpdated != null)
				{
					this.LightingIdUpdated();
				}
				dispatchSceneLayoutDataUpdated();
			}
		}

		public int MusicTrackId
		{
			get
			{
				return musicTrackId;
			}
			set
			{
				musicTrackId = value;
				dispatchSceneLayoutDataUpdated();
			}
		}

		public Dictionary<string, string> ExtraInfo
		{
			get
			{
				return extraInfo;
			}
		}

		public int LayoutCount
		{
			get
			{
				return layout.Count;
			}
		}

		protected override string scopeID
		{
			get
			{
				return CPDataScopes.Zone.ToString();
			}
		}

		protected override Type monoBehaviourType
		{
			get
			{
				return typeof(SceneLayoutDataMonoBehaviour);
			}
		}

		public event Action<SceneLayoutData> SceneLayoutDataUpdated;

		public event Action<SceneLayoutData, DecorationLayoutData> DecorationAdded;

		public event Action<SceneLayoutData> DecorationRemoved;

		public event Action MaxLayoutItemsReached;

		public event Action LightingIdUpdated;

		public void AddExtraInfo(ExtraLayoutInfo extraLayoutInfo)
		{
			extraInfo.Add(extraLayoutInfo.Key, extraLayoutInfo.Value);
			dispatchSceneLayoutDataUpdated();
		}

		public void RemoveExtraInfo(string extraLayoutInfoKey)
		{
			extraInfo.Remove(extraLayoutInfoKey);
			dispatchSceneLayoutDataUpdated();
		}

		public bool IsSameLayout(SceneLayoutData other)
		{
			if (LayoutId != other.LayoutId)
			{
				return false;
			}
			if (lightingId != other.lightingId || musicTrackId != other.musicTrackId)
			{
				return false;
			}
			if (layout.Count != other.layout.Count)
			{
				return false;
			}
			LayoutEnumerator layoutEnumerator = GetLayoutEnumerator();
			foreach (DecorationLayoutData item in layoutEnumerator)
			{
				ParentedSet<string, DecorationLayoutData> parentedSet = other.layout;
				DecorationLayoutData.ID id = item.Id;
				if (!parentedSet.ContainsKey(id.GetFullPath()))
				{
					return false;
				}
				ParentedSet<string, DecorationLayoutData> parentedSet2 = other.layout;
				id = item.Id;
				DecorationLayoutData other2 = parentedSet2.Get(id.GetFullPath());
				if (!item.Equals(other2))
				{
					return false;
				}
			}
			return true;
		}

		public void UpdateData(long layoutId, string lotZoneName, long createdDate, long lastModifiedDate, bool memberOnly, int lightingId, int musicTrackId, Dictionary<string, string> extraInfo, List<DecorationLayoutData> layout)
		{
			LayoutId = layoutId;
			this.lotZoneName = lotZoneName;
			CreatedDate = createdDate;
			LastModifiedDate = lastModifiedDate;
			MemberOnly = memberOnly;
			this.lightingId = lightingId;
			this.musicTrackId = musicTrackId;
			this.extraInfo = extraInfo;
			setLayout(layout);
			dispatchSceneLayoutDataUpdated();
		}

		private void setLayout(List<DecorationLayoutData> layout)
		{
			this.layout = new ParentedSet<string, DecorationLayoutData>();
			for (int i = 0; i < layout.Count; i++)
			{
				this.layout.Add(layout[i].Id.ParentPath, layout[i].Id.GetFullPath(), layout[i]);
			}
		}

		public void AddDecoration(DecorationLayoutData decoration)
		{
			if (IsLayoutAtMaxItemLimit())
			{
				if (this.MaxLayoutItemsReached != null)
				{
					this.MaxLayoutItemsReached.InvokeSafe();
				}
			}
			else if (!layout.Contains(decoration.Id.GetFullPath()))
			{
				layout.Add(decoration.Id.ParentPath, decoration.Id.GetFullPath(), decoration);
				dispatchSceneLayoutDataUpdated();
				if (this.DecorationAdded != null)
				{
					this.DecorationAdded.InvokeSafe(this, decoration);
				}
			}
		}

		public bool IsLayoutAtMaxItemLimit()
		{
			return LayoutCount >= MaxLayoutItems;
		}

		public bool IsLayoutAtMaxItemLimit(DecorationLayoutData selectedItem, bool selectedItemIsAPair = false)
		{
			int num = 0;
			if (!layout.Contains(selectedItem.Id.GetFullPath()))
			{
				num = 1;
				if (selectedItemIsAPair)
				{
					num++;
				}
			}
			return LayoutCount + num >= MaxLayoutItems;
		}

		public void RemoveDecoration(DecorationLayoutData decoration, bool deleteChildren)
		{
			layout.Remove(decoration.Id.ParentPath, decoration.Id.GetFullPath(), deleteChildren);
			dispatchSceneLayoutDataUpdated();
			if (this.DecorationRemoved != null)
			{
				this.DecorationRemoved(this);
			}
		}

		public bool ContainsKey(string id)
		{
			return layout.ContainsKey(id);
		}

		public void UpdateDecoration(string previousId, DecorationLayoutData decoration)
		{
			if (layout.ContainsKey(previousId))
			{
				DecorationLayoutData decorationLayoutData = layout.Get(previousId);
				layout.Update(decorationLayoutData.Id.ParentPath, previousId, decoration.Id.ParentPath, decoration.Id.GetFullPath(), decoration);
				if (previousId != decoration.Id.GetFullPath())
				{
					updateChildIdReferences(decoration);
				}
				dispatchSceneLayoutDataUpdated();
			}
		}

		private void updateChildIdReferences(DecorationLayoutData parent)
		{
			List<string> children = layout.GetChildren(parent.Id.GetFullPath());
			for (int i = 0; i < children.Count; i++)
			{
				DecorationLayoutData decorationLayoutData = layout.Get(children[i]);
				string fullPath = decorationLayoutData.Id.GetFullPath();
				decorationLayoutData.Id.ParentPath = parent.Id.GetFullPath();
				layout.Update(decorationLayoutData.Id.ParentPath, fullPath, decorationLayoutData.Id.ParentPath, decorationLayoutData.Id.GetFullPath(), decorationLayoutData);
				updateChildIdReferences(decorationLayoutData);
			}
		}

		private void dispatchSceneLayoutDataUpdated()
		{
			IsDirty = true;
			if (this.SceneLayoutDataUpdated != null)
			{
				this.SceneLayoutDataUpdated(this);
			}
		}

		protected override void notifyWillBeDestroyed()
		{
			this.SceneLayoutDataUpdated = null;
			this.DecorationAdded = null;
			this.MaxLayoutItemsReached = null;
			this.DecorationRemoved = null;
			this.LightingIdUpdated = null;
		}

		public LayoutEnumerator GetLayoutEnumerator()
		{
			return new LayoutEnumerator(layout.GetEnumerator());
		}

		public OrderedLayoutEnumerator GetOrderedLayoutEnumerator()
		{
			return new OrderedLayoutEnumeratorImpl(layout);
		}

		public void DebugPrintLayout()
		{
			debugPrintLayout(GetOrderedLayoutEnumerator(), 0);
		}

		private void debugPrintLayout(OrderedLayoutEnumerator layout, int indent)
		{
			foreach (ParentedDecorationData item in layout)
			{
				debugPrintDecoration(item.Data, indent);
				foreach (OrderedLayoutEnumerator child in item.Children)
				{
					debugPrintLayout(child, indent + 1);
				}
			}
		}

		private void debugPrintDecoration(DecorationLayoutData data, int indent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append('\t', indent);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append(stringBuilder).Append(data.Id.GetFullPath()).Append(" => {\n");
			stringBuilder2.Append(stringBuilder).AppendFormat("\tName: {0},\n", data.Id.Name);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tParent: {0},\n", data.Id.ParentPath);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tDefinitionId: {0},\n", data.DefinitionId);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tPosition: {0},\n", data.Position);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tRotation: {0},\n", data.Rotation);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tType: {0},\n", data.Type);
			stringBuilder2.Append(stringBuilder).AppendFormat("\tUniformScale: {0},\n", data.UniformScale);
			stringBuilder2.Append(stringBuilder).Append("},\n");
		}
	}
}
