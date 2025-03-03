using ClubPenguin.Collectibles;
using UnityEngine;

public class ExchangeItem
{
	public const string COLLECTIBLE_TYPE_EMPTY = "EmptyCollectible";

	public int QuantityEarned
	{
		get;
		set;
	}

	public string CollectibleType
	{
		get;
		set;
	}

	public CollectibleDefinition CollectibleDefinition
	{
		get;
		set;
	}

	public Sprite ItemSprite
	{
		get;
		set;
	}

	public string LocalizedItemName
	{
		get;
		set;
	}

	public bool CanExchange()
	{
		return true;
	}
}
