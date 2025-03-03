using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

using System.Linq;

using UnityEngine;
using UnityEditor;

using Net.FabreJean.UnityEditor;
using MyUtils = Net.FabreJean.UnityEditor.Utils;

#pragma warning disable 618

namespace Net.FabreJean.PlayMaker.Ecosystem
{
	public class ProjectScanner
	{

		static ProjectScanner _instance;

		public static ProjectScanner instance
		{
			get { 
				if (_instance==null)
				{
					_instance = new ProjectScanner();
				}
				return _instance;
			}
		}

		//[MenuItem ("PlayMaker/Addons/Ecosystem/Scan Project",true)]
		static bool ScanProjectMenuValidation()
		{
			return ! instance.IsScanning;
		}

		//[MenuItem ("PlayMaker/Addons/Ecosystem/Scan Project")]
		public static void ScanProject()	
		{
			instance.LaunchScanningProcess(true);
		}

	 

		bool _isScanning;
		public bool IsScanning
		{
			get{ return _isScanning;}
		}

		bool _hasError;
		public bool hasError
		{
			get{ return _hasError;}
		}

		bool _isProjectScanned;
		public bool isProjectScanned
		{
			get{ return _isProjectScanned;}
		}

		int _foundAssetsCountInProject;
		public int foundAssetsCountInProject
		{
			get{ return _foundAssetsCountInProject;}
		}


		public int AssetsCount
		{
			get{
				return AssetsList.Count;
			}
		}

		public string GetJoinedSelectedAssets()
		{
			return string.Join(",",AssetsSelectedList.ToArray());
		}

		public SortedDictionary<string,AssetItem> AssetsList = new SortedDictionary<string, AssetItem>();

		public List<string> AssetsSelectedList = new List<string>();
		public string[] AssetsFoundList;
		public string[] AssetsNotFoundList;

		public bool OutputInConsole = true;


		bool _cancelFlag;

		HttpWrapper _wwwWrapper;

		string _editorPlayerPrefKey;

		/// <summary>
		/// Launch the scanning process.
		/// </summary>
		public void LaunchScanningProcess(bool ConsoleOutput,string EditorPlayerPrefKey = "")
		{

			OutputInConsole = ConsoleOutput;

			_editorPlayerPrefKey = EditorPlayerPrefKey;

			if (OutputInConsole) Debug.Log("Project Scanner: Downloading Assets Description");

			_hasError = false;
			_isScanning = true;
			AssetsList = new SortedDictionary<string, AssetItem>();

			_wwwWrapper = new HttpWrapper();

			WWWForm _form = new WWWForm();

			_form.AddField("UnityVersion",Application.unityVersion);
			_form.AddField("PlayMakerVersion",MyUtils.GetPlayMakerVersion());

			_wwwWrapper.GET
			(
				"http://www.fabrejean.net/projects/playmaker_ecosystem/assetsDescription"
				,
				_form
				,
				(WWW www) => 
				{
					if (!string.IsNullOrEmpty(www.error))
					{
						Debug.LogError("Project Scanner: Error downloading assets definition :"+www.error);

						_isScanning = false;
						_hasError = true;
					}else{
						EditorCoroutine.start(DoScanProject(www.text));
					}
				}
			);
	
		}

		public void CancelScanningProcess()
		{
			if (_editorPlayerPrefKey!=null)
			{
				if (!EditorPrefs.HasKey(_editorPlayerPrefKey))
				{
					EditorPrefs.SetString(_editorPlayerPrefKey,"");
				}
			}

			// cancel async processes;
			_cancelFlag = true;
			if (_wwwWrapper!=null)
			{
				_wwwWrapper.Cancel();
			}

			// reset variables
			_isProjectScanned = false;
			_isScanning = false;
			_foundAssetsCountInProject = 0;
			AssetsFoundList = new string[0];
			AssetsNotFoundList = new string[0];
			AssetsList = new SortedDictionary<string, AssetItem>();

			Debug.Log("Project Scanning operation cancelled");

		}


		IEnumerator DoScanProject(string assetsDescription)
		{
			_isProjectScanned = false;
			_cancelFlag = false;
			_isScanning = true;
			_hasError = false;
			AssetsList = new SortedDictionary<string, AssetItem>();

			Hashtable _assets = (Hashtable)JSON.JsonDecode(assetsDescription);

			AssetsFoundList = new string[0];
			AssetsNotFoundList = new string[0];
			if (_assets == null)
			{
				Debug.LogError("Ecosystem Asset Description is invalid");
				_hasError = true;
				_isScanning = false;
				yield break;

			}

			yield return null;

			foreach(DictionaryEntry entry in _assets)
			{
				yield return null;

				Hashtable _assetDescription = (Hashtable)entry.Value;

				if (!_assetDescription.ContainsKey("Enabled") || ((int)_assetDescription["Enabled"]==1))
				{
					EditorCoroutine _findAssetCoroutine = EditorCoroutine.startManual(FindAsset(_assetDescription));
					while (_findAssetCoroutine.routine.MoveNext()) {
					
						yield return _findAssetCoroutine.routine.Current;
					}
				}
				yield return null;
			}

			// sort assets

			_foundAssetsCountInProject = AssetsFoundList.Length;

			_isScanning = false;


			if (!_cancelFlag)
			{
				_isProjectScanned = true;

				if (OutputInConsole) Debug.Log("Project Scanner scanned "+AssetsList.Count+" Assets descriptions");

				if (OutputInConsole) Debug.Log(GetScanSummary());
			}

			if (!string.IsNullOrEmpty(_editorPlayerPrefKey))
			{
				EditorPrefs.SetString(_editorPlayerPrefKey,GetScanJsonSummary());
				yield return null;
			}

			yield break;
		}

		IEnumerator FindAsset(Hashtable _definition)
		{

			// just for nice asynch effect
			for(int i=0;i<10;i++)
			{
				yield return null;
			}

			if (_cancelFlag) yield break;

			if (_definition==null)
			{
				Debug.LogWarning("FindAsset failed, details are missing");
				yield break;
			}

			string _name = (string)_definition["Name"];

			AssetItem _item = AssetItem.AssetItemFromHashTable(_definition);


			AssetsList[_name] = _item;

			yield return null;

			// get the scan methods
			ArrayList _scanMethods = (ArrayList)_definition["ScanMethods"];

			if (_scanMethods==null)
			{
				if (OutputInConsole) Debug.LogWarning("Scanning failed for "+_definition["Name"]+": missing 'ScanMethod' definitions" );
				yield break;
			}

			bool _found = false;

			foreach(Hashtable entry in _scanMethods)
			{
				if (entry.ContainsKey("FindByFile"))
				{
					_found= MyUtils.DoesFileExistsAssets((string)entry["FindByFile"]);

				}else if(entry.ContainsKey("FindByClass"))
				{
					_found = MyUtils.isClassDefined((string)entry["FindByClass"]);
				}else if(entry.ContainsKey("FindByNamespace"))
				{
					_found = MyUtils.isNamespaceDefined((string)entry["FindByNamespace"]);
				}

				if (_found)
				{
					// get the version
					_item.ProjectVersion = new VersionInfo(GetAssetVersion(_definition));
					if (OutputInConsole) Debug.Log(_definition["Name"]+" <color=green>found</color> in Project, version: "+_item.ProjectVersion);

					_item.FoundInProject = true;

					ArrayUtility.Add<string>(ref AssetsFoundList,_name);

					yield return null;
					continue;
				}else{
					ArrayUtility.Add<string>(ref AssetsNotFoundList,_name);
				}

				yield return null;
			}

			// append automatically to selected list
			// TODO: give the user some higher level settings like ignore all unfound toggle or something.
			// TODO: also remember user preference.
			if (_found)
			{
				AssetsSelectedList.Remove(_name);
				AssetsSelectedList.Add(_name);
				_item.SelectAllCategories();
			}
		
			if (OutputInConsole) Debug.Log(_definition["Name"]+" <color=red>not found</color> in Project");

			yield break;
		}

		string GetAssetVersion(Hashtable _definition)
		{
			string name = (string)_definition["Name"];

			if (name.Equals("PlayMaker"))
			{
				return MyUtils.GetPlayMakerVersion();
			}

			if (_definition.ContainsKey("VersionScanMethod"))
			{
				Hashtable _versionScanDetails = (Hashtable)_definition["VersionScanMethod"];
				if (_versionScanDetails.ContainsKey("FindInTextFile"))
				{
					try
					{
					Regex pattern = new Regex((string)_versionScanDetails["VersionRegexPattern"]);

					using (StreamReader inputReader = new StreamReader(Application.dataPath+_versionScanDetails["FindInTextFile"]))
					{
						while (!inputReader.EndOfStream)                     
						{
							try 
							{
								Match m = pattern.Match(inputReader.ReadLine());
								if (m.Success)
								{
									return m.Value;
								}
							}
							catch (FormatException) {}
							catch (OverflowException) {}
						}
					}
					}catch(Exception e)
					{
						Debug.LogError("Project Scanning error for version scanning of "+name+" :"+e.Message);
					}

				}else if (_versionScanDetails.ContainsKey("FindInVersionInfo"))
				{
					string _jsonText = File.ReadAllText(Application.dataPath+_versionScanDetails["FindInVersionInfo"]);
					return VersionInfo.VersionInfoFromJson(_jsonText).ToString();
				}
			}

			return "n/a";
		}

		public string GetScanSummary()
		{
			if (AssetsList ==null || !isProjectScanned)
			{
				return "Please scan project first";
			}

			if (AssetsCount ==0)
			{
				return "No Known Assets detected in Project";
			}

			string _result = "Project scanning result:";
			_result += "\n"+SystemInfo.operatingSystem;
			_result += "\nUnity "+Application.unityVersion+" "+(Application.HasProLicense()?"Pro":"") +" targeting:"+Application.platform.ToString();

			foreach( KeyValuePair<string,AssetItem> _entry in AssetsList)
			{
				AssetItem _item = _entry.Value;

				if (_item.FoundInProject)
				{
					_result += "\n"+_entry.Key+" : "+_item.ProjectVersion.ToString();
				}
			}

			return _result;
		}

		public string GetScanJsonSummary()
		{
			if (AssetsList ==null || !isProjectScanned)
			{
				return "{Error:\"Please scan Projector first\"}";
			}
			
			if (AssetsCount ==0)
			{
				return "{FoundAssets:0}";
			}
			
			Hashtable _result = new Hashtable();
			_result["OperatingSystem"] = SystemInfo.operatingSystem;

			_result["UnityVersion"] = Application.unityVersion;
			_result["HasProLicense"] = Application.HasProLicense()?"true":"false";
			_result["platform"] = Application.platform.ToString();

			
			foreach( KeyValuePair<string,AssetItem> _entry in AssetsList)
			{
				AssetItem _item = _entry.Value;
				
				if (_item.FoundInProject)
				{
					_result[_entry.Key] = _item.ProjectVersion.ToString();
				}
			}
			
			return JSON.JsonEncode(_result);
		}

	}

	/// <summary>
	/// ProjectScanner Asset item.
	/// </summary>
	public class AssetItem
	{
		public string Name= "n/a";
		public string PublisherName= "n/a";
		public VersionInfo ProjectVersion = new VersionInfo();
		public VersionInfo LatestVersion = new VersionInfo();
		public string Type = "Asset";
		public string Url="";
		public string ThumbnailUrl = "";
		public int AssetStoreId = 0;

		public bool HasEcosystemContent = false;
		public bool FoundInProject = false;

		public string[] EcosystemCategories = new string[0];

		public string[] EcosystemSelectedCategories = new string[0];

		public static AssetItem AssetItemFromHashTable(Hashtable details)
		{
			AssetItem _item =  new AssetItem();
			if (details == null)
			{
				return _item;
			}

			if (details.ContainsKey("Type")) _item.Type = (string)details["Type"];
			if (details.ContainsKey("Name")) _item.Name = (string)details["Name"];
			if (details.ContainsKey("Publisher")) _item.PublisherName = (string)details["Publisher"];
			if (details.ContainsKey("Url")) _item.Url = (string)details["Url"];
			if (details.ContainsKey("ThumbnailUrl")) _item.ThumbnailUrl = (string)details["ThumbnailUrl"];

			if (details.ContainsKey("AssetStoreId"))
			{
				_item.AssetStoreId = (int)details["AssetStoreId"];
			}
			if (details.ContainsKey("Version")) _item.LatestVersion = new VersionInfo((string)details["Version"]);


			if (details.ContainsKey("Ecosystem")) 
			{
				_item.HasEcosystemContent = true;

				Hashtable _ecosystemDetails = (Hashtable)details["Ecosystem"];

				if (_ecosystemDetails.ContainsKey("Categories"))
				{

					ArrayList _filters = (ArrayList)_ecosystemDetails["Categories"];
					_item.EcosystemCategories = (String[]) _filters.ToArray( typeof( string ) );

				}
			}

			return _item;
		}

		public static AssetItem AssetItemFromJson(String jsonString)
		{
			AssetItem _item =  new AssetItem();
			if (string.IsNullOrEmpty(jsonString))
			{
				return _item;
			}
			
			Hashtable _details = (Hashtable)JSON.JsonDecode(jsonString);

			return AssetItemFromHashTable(_details);
		}


		public void SelectAllCategories()
		{
			EcosystemSelectedCategories = new string[EcosystemCategories.Length];
			EcosystemCategories.CopyTo(EcosystemSelectedCategories,0);
		
		}

		public void UnSelectAllCategories()
		{
			EcosystemSelectedCategories = new string[0];
		}

		public void ToggleCategory(string Category)
		{
			if (EcosystemSelectedCategories.Contains(Category))
			{
				ArrayUtility.Remove(ref EcosystemSelectedCategories,Category);
			}else if (EcosystemCategories.Contains(Category))
			{
				ArrayUtility.Add(ref EcosystemSelectedCategories,Category);
			}
		}

		public int GetFilterSelectionState()
		{
			if (EcosystemSelectedCategories.Length == 0) return 0;

			if (EcosystemSelectedCategories.Length == EcosystemCategories.Length) return 2;

			if (EcosystemSelectedCategories.Length != EcosystemCategories.Length) return 1;

			return 0;
		}

	}
}
