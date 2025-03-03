using System;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Tweaker.Core
{
	public class SearchOptions
	{
		public enum ScopeType
		{
			NonPublic,
			Public,
			All
		}

		public enum BindingType
		{
			Static,
			Instance,
			All
		}

		public WeakReference weakInstance;

		public Regex NameRegex
		{
			get;
			set;
		}

		public Regex AssemblyRegex
		{
			get;
			set;
		}

		public ScopeType Scope
		{
			get;
			set;
		}

		public BindingType Binding
		{
			get;
			set;
		}

		public SearchOptions(string nameRegex = null, string assemblyRegex = null, ScopeType scope = ScopeType.All, BindingType binding = BindingType.All, object instance = null)
		{
			NameRegex = ((nameRegex == null) ? null : new Regex(nameRegex));
			AssemblyRegex = ((assemblyRegex == null) ? null : new Regex(assemblyRegex));
			Scope = scope;
			Binding = binding;
			weakInstance = ((instance != null) ? new WeakReference(instance) : null);
		}

		public bool CheckMatch(string name, MethodInfo info)
		{
			return CheckMatch(name, info.ReflectedType.Assembly, info.IsPublic ? ScopeType.Public : ScopeType.NonPublic, (!info.IsStatic) ? BindingType.Instance : BindingType.Static);
		}

		public bool CheckMatch(string name, FieldInfo info)
		{
			return CheckMatch(name, info.ReflectedType.Assembly, info.IsPublic ? ScopeType.Public : ScopeType.NonPublic, (!info.IsStatic) ? BindingType.Instance : BindingType.Static);
		}

		public bool CheckMatch(ITweakerObject obj)
		{
			return CheckMatch(obj.Name, obj.Assembly, obj.IsPublic ? ScopeType.Public : ScopeType.NonPublic, (obj.WeakInstance != null) ? BindingType.Instance : BindingType.Static, obj.WeakInstance);
		}

		public bool CheckMatch(string name, Assembly assembly, ScopeType scope, BindingType binding, WeakReference weakInstance = null)
		{
			if (Scope != ScopeType.All && Scope != scope)
			{
				return false;
			}
			if (Binding != BindingType.All && Binding != binding)
			{
				return false;
			}
			if (NameRegex != null && name != null && !NameRegex.Match(name).Success)
			{
				return false;
			}
			if (AssemblyRegex != null && assembly != null && !AssemblyRegex.Match(assembly.FullName).Success)
			{
				return false;
			}
			if (this.weakInstance != null)
			{
				if (weakInstance == null)
				{
					return false;
				}
				object target = null;
				weakInstance.TryGetTarget(out target);
				object target2 = null;
				this.weakInstance.TryGetTarget(out target2);
				if (target2 != target)
				{
					return false;
				}
			}
			return true;
		}
	}
}
