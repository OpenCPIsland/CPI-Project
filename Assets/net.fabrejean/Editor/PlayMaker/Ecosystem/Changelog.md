#Ecosystem Change log

###0.6.11
**Release:** 17/03/2021

**Fix:** fixed 2021 support

###0.6.10
**Release:** 17/03/2021

**Update:** made unity version tag optional ( can be toggled in settings menu)

###0.6.9
**Release:** 15/03/2021

**Update:** removed obsolete code, moved to Unity 2018

**Fix:** fixed 2020 support

###0.6.8
**Release:** 20/04/2020 

**Fix:** Added 2020 support


###0.6.7
**Release:** 03/04/2020 

**Fix:** Fixed 2019 WWW url weird bug where it doesn't keep the original url passed

###0.6.6
**Release:** 20/05/2019 

**Update:** Removed some updateable api to prevent mess in 2019 and up when api updater doesn't perform

###0.6.5
**Release:** 30/04/2019 

**Update:** Support for Unity 2019

###0.6.4
**Release:** 23/01/2019 

**Fixed:** moved JSON into namespace  Net.FabreJean.UnityEditor to avoid conflict with other publishers using JSON too.

###0.6.3
**Release:** 8/11/2018 

**Update:** Remove WWW obsolete warnings on 2018.3 and newer

###0.6.2
**Release:** 18/10/2018 

**New:** authoring tool updated for latest unity and playmaker versions and 2018 github rep


###0.6.1
**Release:** 28/03/2018 

**New:** Support for Unity 2018 

###0.6
**Release:** 26/02/2018 

**New:** Added new function to be able to auto search from another script. 
**New:** New Scripting define symbols for `ECOSYSTEM` and `ECOSYSTEM_0_6`. 

**Change:** string searchString = "";  TO static string searchString = ""; (to be able to access from another scrips).  
**Change:** private List<PlayMakerEcosystemFilters> searchFilters;  TO  private static List<PlayMakerEcosystemFilters> searchFilters;  (to be able to access from another scrips)   

**Update:** Scripting define symbols routine to mount and unmount updated ( taken from Playmaker Editor Utils)

###0.5.3
**Release:** 03/08/2017  

**Fix:** Addon menu item properly indexed to not end up first.

###0.5.2
**Release:** 01/08/2017  

**New:** Added verification protocol to accept server's certificates, trying to resolve empty return from queries

###0.5.1
**Release:** 24/07/2017  

**Update:** Fixed Support for Unity 2017.0 and 2017.1

**Fix**: Fixed bad compile flag for obsolete .title property of EditorWindow

###0.5 
**Release:** 24/07/2017  

**Known Issue:** Unity 2017.0 and 2017.1 have the www class redirect url [broken](https://issuetracker.unity3d.com/issues/www-dot-url-truncates-redirected-url-if-it-should-have-contained-spaces), so the Ecosystem browser doesn't work on these two versions. Implemented defines check to warn and prevent using the Browser in these versions

**Update:** Compatibility with Unity 2017, Unity 2017 WWW Class broke the .url parameter by changing it to reflect redirections, so the url set when creating the WWW Class is not necessary the one when done.  
So now a new unique local Id for each browser requests is maintained on the client side, passed to the server which will pass it on to the redirection url, when a www instance is done, it's the uid parameter that is used to reference the content and not the url.

**New:** `EcosystemUtils` url query parameters methods to add and get parameters  

###0.4.9 alpha
**Removed:** https://snipt.net/ support removed, snipt got deprecated, service is unavailable 

###0.4.8 alpha 
**Improvement**: new overlay UI technic for mouse overing while letting list width adjust properly   

**Fix**: UI switch for project scanner integration is improved  
**Fix**: Unity 4.7 compatibility  
**Fix**: First search error in table listing

###0.4.7 alpha 
**Fix:** Fixed MarkDown Custom Inspector on Unity 5  
**Fix:** Fixed Unity 4.7 Compatibility  

**New:** `WebImageManager` to help caching online images (for asset thumbnails, etc ) Implemented in Ecosystem browser project scanner listing

 

###0.4.6 alpha  
**New:** ScriptingSymbol setup to enable/disable beta features from the settings menu.  
**New:** ProjectScanner now automatically fired when starting Ecosystem  
**New:** Integration of Project scanner results and selection in the search  
**New:** Simple UI changes to validate internal process   

###0.4.5
**ReleaseDate:** 16/09/2015  

**Fix:** utils for getting relative asset path from absolute path, would not work on some projects paths  
**Fix:** loading of meta data in *LoadItemMetaData()* to take in account package text file definition  

**New:** pingMenu meta data for assets so that clicking on an item in the ecosystem list, will launch a menu  
**New:** copy paste search field  

###0.4.4
**ReleaseDate:** 09/09/2015 
 
**Fix:** MarkDown api compatibility with Unity 5.2  

**change:** Menu to access the ecosystem browser is now *PlayMaker/Addons/Ecosystem/Ecosystem Browser*  

**New:** VersionInfo now takes into account potential appendix to a version like uTomate *1.5.0 LE* edition  
**New:** Project Scanner tool, early version, used for bug reporting mainly. define **PLAYMAKER\_ECOSYSTEM\_BETA** in PLayer settings 'scripting define symbols' to enable the feature   
**New:** Ecosystem Icon on EditorWindow tab for Unity 5.x version  
**New:** Update button on installed assets, for easier process ( rather than "delete", then "get").

**Improvement:** Updated api to avoid Unity 5 updater process for `Resources.LoadAssetAtPath` to`AssetDatabase.LoadAssetAtPath`  

**Note:** Authoring and publishing features in development but ignored for now, so many utils and tools not in used yet for this release  

###0.4.3  
**Improvement:** Support for flexible dependancies injection with plain urls. The url, if not a unityPackage should have a _assetFilePath_ query parameter defining the file path within the Project (i.e Assets/xxx/y.cs), unescaped. if not defined, 

###0.4.2
**New:** [Snipt](https://snipt.net/) online repository initiative for simple scripts and easier publishing for contributors  

**Improvement:** Renamed "github" UI button "Preview" to be agnostic to the source itself.  
**Improvement:** Disclaimer page now features github and snipt links to inform people where content is pooled from 

**Fix:** *Server side*: Unity version tag for items, now content from Unity 5 online rep are tagged "Unity 5" properly

###0.4.1

**New:** Added "Github" button to open an item on Github. Very handy to reference and access content source.
**Fix:** Support for Unity 5 context. Warning some content will be obsolete nonnettes, especially physics based samples

###0.4.0
**Note:** **_Breaking Build_**. The "net.fabrejean" or the "jf_plugins" folder **MUST** be deleted first

**New:** MetaData download for documentation   
**New:** Youtube Link in Documentation when defined in metaData   
**New:** Youtube link on the disclaimer page  
**New:** Soft Integration of PlayMaker version check, to not throw error if PlayMaker not installed  
**New:** Visual feedback if PlayMaker is not installed, with button to open the asset store   
**New:** Version Type to match Unity and PlayMaker version info.  
**New:** Interactive versioning with Build being representative of the number of compilation.   
This is based on [InControl](https://github.com/pbhogan/InControl) by Patrick Hogan   
**New:** ChangeLog based on MarkDown. Ain't that funky, hyperlinks in regular Unity GUI!!! never seen that before :)  
**New:** Documentation of items using a png image, using PlayMaker documentation tool for actions and up to the discretion of publishers for other content  
( like screenshot of the component inspector, or even the game view or anything)   
 Dark and Light skin of Unity are supported with explicit path convention,   
 so the file path for doc images must be respected or it will not be found.   
**New:** IsDebugOn static public bool for surrounding classes to follow debug convention   
**New:** UI Update prompter and menus, currently only redirecting to the wiki page.

**Fix:** Fixed Editor Prefs, and improve prefs handling   
**Fix:** Fixed search errors and interface being locked when error is shown in interface.  
**Fix:** Fixed Server side search filtering when no filter is selected  
**Fix:** Fixed Guiskin for better alignment   
 
**Improvement:** Improved code base, make use of namespace to avoid potential conflict   
**Improvement:** started moving item based code into its own class to free up the browser class itself, make sense and will scale better  
**Improvement:** Dedicated [Trello board](https://trello.com/b/U0AH0SHy/ecosystem) to keep track of this overwhelming project :)   
**Improvement:** code is now using a repository for security   
**Improvement:** remove discrete bar glitch when recompiling   
**Improvement:** MarkDownGUI allow for custom guiskin and style   
**Improvement:** Ecosystem disclaimer uses MarkDown now   
**Improvement:** made the license text in small to gain space   


###0.3.0
**New:**	Package Item   


###0.2.0
**New:**	Template and Sample  
**New:**	Filtering of items

**Improvement:** custom search bar, to match the UI skin   
**Improvement:** better handling of debug logs with a proper settings to turn it on and off


###0.1.0
**Note:**	Initial Public Release 