using UnityEngine;

[CreateAssetMenu(menuName = "Definition/Event Fog")]
public class EventFog : ScriptableObject

{
    public bool FogEnabled = true;

        public Color Color = new Color32(7, 0, 36, byte.MaxValue);

        public float Density = 0.9f;

        public FogMode FogMode = FogMode.Linear;

        public float StartDistance = -65f;

        public float EndDistance = 75f;
    }
