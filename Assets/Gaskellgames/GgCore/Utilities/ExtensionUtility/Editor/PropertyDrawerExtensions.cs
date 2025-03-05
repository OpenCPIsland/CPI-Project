#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code updated by Gaskellgames: https://github.com/Gaskellgames
    /// Original code created by ghysc: https://github.com/ghysc/SwitchAttribute
    /// </summary>
	
    public static class PropertyDrawerExtensions
    {
        private static Dictionary<Type, PropertyDrawer> customDrawers = new Dictionary<Type, PropertyDrawer>();

        /// <summary>
        /// Returns the custom property drawer for given property, if one exists.
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        public static PropertyDrawer Find(SerializedProperty property)
        {
            Type propertyType = property.GetPropertyType();
            if (propertyType == null) { return null; }
            
            // cache value for future checks
            if (!customDrawers.ContainsKey(propertyType))
            {
                customDrawers.Add(propertyType, Find(propertyType));
            }
            return customDrawers[propertyType];

        }
        
        /// <summary>
        /// Returns custom property drawer for type if one could be found, or null if 
        /// no custom property drawer could be found. Does not use cached values, so it's resource intensive.
        /// </summary>
        public static PropertyDrawer Find(Type propertyType)
        {
            foreach (Assembly assem in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type candidate in assem.GetTypes())
                {
                    FieldInfo typeField = typeof(CustomPropertyDrawer).GetField("m_Type", BindingFlags.NonPublic | BindingFlags.Instance);
                    FieldInfo childField = typeof(CustomPropertyDrawer).GetField("m_UseForChildren", BindingFlags.NonPublic | BindingFlags.Instance);
                    foreach (Attribute a in candidate.GetCustomAttributes(typeof(CustomPropertyDrawer)))
                    {
                        if (a.GetType().IsSubclassOf(typeof(CustomPropertyDrawer)) || a.GetType() == typeof(CustomPropertyDrawer))
                        {
                            CustomPropertyDrawer drawerAttribute = (CustomPropertyDrawer)a;
                            Type drawerType = (Type)typeField.GetValue(drawerAttribute);
                            if (drawerType == propertyType ||
                                ((bool)childField.GetValue(drawerAttribute) && propertyType.IsSubclassOf(drawerType)) ||
                                ((bool)childField.GetValue(drawerAttribute) && TypeExtensions.IsGenericSubclass(drawerType, propertyType)))
                            {
                                if (candidate.IsSubclassOf(typeof(PropertyDrawer)))
                                {
                                    return (PropertyDrawer)Activator.CreateInstance(candidate);
                                }
                            }
                        }
                    }
                }
            }
			
            return null;
        }

    } // class end
}

#endif