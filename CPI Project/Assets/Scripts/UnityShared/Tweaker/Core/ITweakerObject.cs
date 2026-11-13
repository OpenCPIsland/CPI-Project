using System;
using System.Reflection;

namespace Tweaker.Core
{
	public interface ITweakerObject
	{
		string Description
		{
			get;
		}

		string Name
		{
			get;
		}

		string ShortName
		{
			get;
		}

		bool IsPublic
		{
			get;
		}

		Assembly Assembly
		{
			get;
		}

		WeakReference WeakInstance
		{
			get;
		}

		object StrongInstance
		{
			get;
		}

		bool IsValid
		{
			get;
		}

		ICustomTweakerAttribute[] CustomAttributes
		{
			get;
		}

		TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute, ICustomTweakerAttribute;
	}
}
