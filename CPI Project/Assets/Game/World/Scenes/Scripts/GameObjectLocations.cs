using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using RenderSettings = UnityEngine.RenderSettings;

public class GameObjectLocations : MonoBehaviour
{
	// Enable for baking and disable for runtime
	public GameObject Lighting;
	// Lightmap baking skybox
#if UNITY_EDITOR
    public Material LightmappingSkybox;
	public Material PiratePartySkyboxForBakingLightmaps;
#endif
	// Set these to static
	public GameObject BoxDimensionDecorations;
	public GameObject EventPirateParty2018_Beach_Prefab;
	// Medieval Dungeon
	public GameObject OrbTracker;
	public GameObject BigBadBoulder;
	public GameObject BattleTracker;
	public GameObject Door1;
	public GameObject Door2;
	public GameObject Door3;
	public GameObject Door4;
	public GameObject Door5;
	public GameObject Door6;
	// Ends Medieval Dungeon
	public GameObject Animated;
	public GameObject Animated2;
	public GameObject Animated3;
	public GameObject Animated4;
	public GameObject Animated5;
	public GameObject Animated6;
	public GameObject AnimatedDoor1;
	public GameObject AnimatedDoor2;
    // Town
    public GameObject FrontTrainDoorLeft;
	public GameObject FrontTrainDoorRight;
	public GameObject StudioDoorLeft;
	public GameObject StudioDoorRight;
	public GameObject ClothingDoorLeft;
	public GameObject ClothingDoorRight;
	// The default skybox
    public Material DayCubemap;
	public Material BoxDimensionCubemap;
	public Material DivingCubemap;
	public Material HerbertBaseCubemap;
	public Material MtBlizzardCubemap;
	public Material EventPiratePartySkybox;
	public Material MedievalDungeonSkybox;
	public GameObject GatewayFX;
	// Set the following to not static during lightmap baking but static during gameplay.
	public GameObject StaticObject1;
    public GameObject StaticObject2;
    public GameObject StaticObject3;
    public GameObject StaticObject4;
    public GameObject StaticObject5;
    public GameObject StaticObject6;
    public GameObject StaticObject7;
    public GameObject StaticObject8;
    public GameObject StaticObject9;
    public GameObject StaticObject10;
	public GameObject StaticObject11;
	public GameObject StaticObject12;
	public GameObject StaticObject13;
	public GameObject StaticObject14;
	public GameObject StaticObject15;

    public void ChangeSkybox(Material mat)
    {
        RenderSettings.skybox = mat;
    }
    
    public void ChangeSource(AmbientMode ambientMode)
    {
        RenderSettings.ambientMode = ambientMode;
    }
}
