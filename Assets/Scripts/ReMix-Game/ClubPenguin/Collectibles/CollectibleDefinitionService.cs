using ClubPenguin.Core;
using DevonLocalization.Core;
using Disney.MobileNetwork;
using System.Collections.Generic;
using Tweaker.Core;

namespace ClubPenguin.Collectibles
{
	public class CollectibleDefinitionService
	{
		public class CollectibleTypeGenerator : NamedToggleValueAttribute.NamedToggleValueGenerator
		{
			public IEnumerable<NamedToggleValueAttribute.NamedToggleValue> GetNameToggleValues()
			{
				List<NamedToggleValueAttribute.NamedToggleValue> list = new List<NamedToggleValueAttribute.NamedToggleValue>();
				if (Service.IsSet<CollectibleDefinitionService>())
				{
					foreach (KeyValuePair<string, CollectibleDefinition> item in Service.Get<CollectibleDefinitionService>().dictionary)
					{
						list.Add(new NamedToggleValueAttribute.NamedToggleValue(Service.Get<Localizer>().GetTokenTranslation(item.Value.NameToken), item.Key));
					}
				}
				return list;
			}
		}

		private Dictionary<string, CollectibleDefinition> dictionary = new Dictionary<string, CollectibleDefinition>();

		public CollectibleDefinitionService(Manifest manifest)
		{
			for (int i = 0; i < manifest.Assets.Length; i++)
			{
				CollectibleDefinition collectibleDefinition = manifest.Assets[i] as CollectibleDefinition;
				if (collectibleDefinition != null)
				{
					dictionary[collectibleDefinition.CollectibleType] = collectibleDefinition;
				}
			}
		}

		public bool Contains(string key)
		{
			return dictionary.ContainsKey(key);
		}

		public CollectibleDefinition Get(string key)
		{
			if (dictionary.ContainsKey(key))
			{
				return dictionary[key];
			}
			return null;
		}
	}
}
