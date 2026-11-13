using System;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public sealed class GenerateLightingEventBatch : MonoBehaviour
{
    [Serializable]
    public sealed class SpawnEntry
    {
        public GameObject prefab;
        public string parentName;
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale = Vector3.one;
    }

    [Serializable]
    public sealed class ObjectEntry
    {
        public GameObject target;
    }

    [Serializable]
    public sealed class EventGroup
    {
        public string name;
        public List<SpawnEntry> spawn = new List<SpawnEntry>();
        public List<ObjectEntry> delete = new List<ObjectEntry>();
        public List<ObjectEntry> enable = new List<ObjectEntry>();
        public List<ObjectEntry> disable = new List<ObjectEntry>();
        public List<ObjectEntry> giStatic = new List<ObjectEntry>();
        public List<ObjectEntry> removeStatic = new List<ObjectEntry>();
        [Tooltip("Objects to disable after this event group's lightmap bake completes.")]
        public List<ObjectEntry> disableAfterBake = new List<ObjectEntry>();
    }

    public List<EventGroup> groups = new List<EventGroup>();
}
