using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClubPenguin.Core
{
    [Serializable]
    public struct DecorationLayoutData : IEquatable<DecorationLayoutData>
    {
        public struct ID : IEquatable<ID>
        {
            private string name;

            private string parentPath;

            private string cachedFullPath;

            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    cachedFullPath = "";
                    name = value ?? "";
                }
            }

            public string ParentPath
            {
                get
                {
                    return parentPath;
                }
                set
                {
                    cachedFullPath = "";
                    parentPath = value ?? "";
                }
            }

            public string GetFullPath()
            {
                //   if (cachedFullPath == "")
                //    {
                if (string.IsNullOrEmpty(ParentPath))
                {
                    cachedFullPath = Name;
                }
                else
                {
                    cachedFullPath = ParentPath + "/" + Name;
                }
                //     }
                return cachedFullPath;
            }

            public static ID FromFullPath(string path)
            {
                ID result = default(ID);
                int num = path.LastIndexOf("/");
                if (num != -1)
                {
                    result.parentPath = path.Substring(0, num);
                }
                result.name = path.Substring(num + 1);
                return result;
            }

            public bool Equals(ID other)
            {
                return string.Equals(name, other.name) && string.Equals(parentPath, other.parentPath);
            }

            public override bool Equals(object obj)
            {
                if (object.ReferenceEquals(null, obj))
                {
                    return false;
                }
                return obj is ID && Equals((ID)obj);
            }

            public override int GetHashCode()
            {
                return (((name != null) ? name.GetHashCode() : 0) * 397) ^ ((parentPath != null) ? parentPath.GetHashCode() : 0);
            }

            public static bool operator ==(ID left, ID right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(ID left, ID right)
            {
                return !left.Equals(right);
            }
        }

        public enum DefinitionType
        {
            Decoration,
            Structure
        }

        public ID Id;

        public DefinitionType Type;

        public int DefinitionId;

        public Vector3 Position;

        public Quaternion Rotation;

        public float UniformScale;

        private Dictionary<string, string> customProperties;

        public Dictionary<string, string> CustomProperties
        {
            get
            {
                if (customProperties == null)
                {
                    customProperties = new Dictionary<string, string>();
                }
                return customProperties;
            }
            set
            {
                customProperties = value;
            }
        }

        public bool Equals(DecorationLayoutData other)
        {
            return Id.Equals(other.Id) && Type == other.Type && DefinitionId == other.DefinitionId && Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && UniformScale.Equals(other.UniformScale);
        }

        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is DecorationLayoutData && Equals((DecorationLayoutData)obj);
        }

        public override int GetHashCode()
        {
            int hashCode = Id.GetHashCode();
            hashCode = ((hashCode * 397) ^ (int)Type);
            hashCode = ((hashCode * 397) ^ DefinitionId);
            hashCode = ((hashCode * 397) ^ Position.GetHashCode());
            hashCode = ((hashCode * 397) ^ Rotation.GetHashCode());
            hashCode = ((hashCode * 397) ^ UniformScale.GetHashCode());
            return (hashCode * 397) ^ ((customProperties != null) ? customProperties.GetHashCode() : 0);
        }

        public static bool operator ==(DecorationLayoutData left, DecorationLayoutData right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DecorationLayoutData left, DecorationLayoutData right)
        {
            return !left.Equals(right);
        }
    }
}