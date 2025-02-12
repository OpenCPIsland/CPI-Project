using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using RenderSettings = UnityEngine.RenderSettings;

public class GameObjectLocations : MonoBehaviour
{
	// Boardwalk
	public GameObject LighthouseIce;
    public GameObject funHutSpeakerTowerA;
    public GameObject funHutSpeakerTowerB;
	public GameObject LighthouseUpperDoorLightOff;
	public GameObject LighthouseGlassWallsLightOff;
	// Lightmap baking skybox
    public Material LightmappingSkybox;
	public Material PiratePartySkyboxForBakingLightmaps;
	// Set these to static
	public GameObject HerbertBaseV2;
    public GameObject WorldArt;
	public GameObject World;
	public GameObject LighthouseDoor;
	public GameObject Sewer;
	public GameObject AdditiveWorldArt;
	public GameObject AdditionalDecorations;
	public GameObject Environment;
	public GameObject ClothtingDesignerCustomizerPlatform;
	public GameObject MedievalWorldArt;
	public GameObject RoofFix;
	public GameObject DragonCaveSkull;
	public GameObject BoxDimensionDecorations;
	public GameObject LevelDesign;
	public GameObject Maze_Base;
	public GameObject SecretDanceCaveEntrance;
	public GameObject SecretDanceCave;
	public GameObject EventPirateParty2018_Boardwalk_NotOnQuestDecor;
	public GameObject PiratePartyPrizeRuins;
	public GameObject EventPirateParty2018_Boardwalk_Prefab;
	public GameObject EventPirateParty2018_Beach_Prefab;
	// Town
	public GameObject FrontTrainDoorLeft;
	public GameObject FrontTrainDoorRight;
	public GameObject TrainBlue;
	public GameObject Interactables;
	// The default skybox
    public Material DayCubemap;
	public Material BoxDimensionCubemap;
	public Material DivingCubemap;
	public Material HerbertBaseCubemap;
	public Material MtBlizzardCubemap;
	public Material EventPiratePartySkybox;
	public Material MedievalDungeonSkybox;
	// Summer Splashdown
	public GameObject InflatableDolphinFBX;
	// Disable static on these meshes
	public GameObject DanceCaveSpotLights;
	public GameObject GatewayFX;
	public GameObject GatewayFX2;

    public void ChangeSkybox(Material mat)
    {
        RenderSettings.skybox = mat;
    }
    
    public void ChangeSource(AmbientMode ambientMode)
    {
        RenderSettings.ambientMode = ambientMode;
    }
}
