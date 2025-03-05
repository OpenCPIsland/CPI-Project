using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gaskellgames.FolderSystem.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>

    [System.Serializable]
    public class ProjectFolderIconLinks : IComparable<ProjectFolderIconLinks>
    {
        public string name;
        public Texture texture;
        public List<string> additionalLinks;

        /// <summary>
        /// Compares only the name of the ProjectFolderIconLinks!
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(ProjectFolderIconLinks other)
        {
            if (other == null)
            {
                return -1;
            }
            else
            {
                return name.Equals(other.name) ? 0 : string.CompareOrdinal(name, other.name);
            }
        }
        
    } // class end
}