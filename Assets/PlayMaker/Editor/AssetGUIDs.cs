using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Collect Playmaker Asset GUIDs as static strings
    /// Used instead of asset paths (user can move files)
    /// Also can be used to check for installation problems
    /// </summary>
    public class AssetGUIDs
    {
        // Dll GUIDs - very important!
        // If these change you get missing script errors!

        public static string PlayMakerDll = "e743331561ef77147ae48cda9bcb8209";
        public static string PlayMakerEditorDll = "336aa50a81ce85b47b50a7b6adf85a76";
        public static string ConditionalExpressionDll = "d4efecccbe1d6134f99fa8da66d82942";
        public static string ConditionalExpressionEditorDll = "3588691a691f1074eb5388783b2d2f5d";
        public static string PlayMakerMetroDll = "fd7aabeb995f6a64aa68d02891fc2294";
        public static string PlayMakerWebGLDll = "9754d4abda502c6458053d5ed8e4fc5a";
        public static string PlayMakerWP8Dll = "de72a6d2da64d114d95e3c5a01cfaec5";

        // Install Packages
        // Used by installer to find package to import

        public static string LegacyNetworkingPackage = "9b52451efbd751f48ac368998c153aaf";
        public static string LegacyGUIPackage = "a4dfaee68dc865741bff8cd7207b91ab";
        public static string LegacyITweenPackage = "6d756ccf978926042887a51b4116f6a1";

        public static string PlayMakerUnitypackage1784 = "dd583cbbf618ba54983cdf396b28e49b";
        public static string PlayMakerUnitypackage180 = "f982487afa4f0444ea11e90a9d05b94e";
        public static string PlayMakerUnitypackage181 = "0921e97db908b2f4e8e407e68a2ed27c";
        public static string PlayMakerUnitypackage182 = "cd593cc3ded027746bf4658e85cb9fb9";
        public static string PlayMakerUnitypackage183 = "21698fae67461744189ec5c7a8eb143b";
        public static string PlayMakerUnitypackage184 = "a927a5681695a574386fab6afd5a1a00";
        public static string PlayMakerUnitypackage185 = "b4da689fd2d61134891c9fd284b0485a";
        public static string PlayMakerStudentUnitypackage185 = "4f5bb025ff7f7ae4ba2408a62b827893";
        public static string PlayMakerUnitypackage186 = "17874094cf1a41d429b9e0465bdf2494";
        public static string PlayMakerStudentUnitypackage186 = "2dadff40935957e41b795ea67eb1a3a4";
        public static string PlayMakerUnitypackage187 = "7272f6c05c0b7fc45bb29b684af0c64b";
        public static string PlayMakerStudentUnitypackage187 = "1e1e41d07575cdf419947b725706c8c0";
        public static string PlayMakerUnitypackage188 = "61b2528ec1d4eef4e91c88276af116d6";           
        public static string PlayMakerStudentUnitypackage188 = "91165102f4bbf954e94a545efb466352";   
        public static string PlayMakerUnitypackage189 = "d70e15867595c244c9b06c690e0074cd";           
        public static string PlayMakerStudentUnitypackage189 = "22e8745ba9787ac419ff0322c9c1340d";
        public static string PlayMakerUnitypackage190 = "cf739c5501a07794bb9fc605438c0eb9";           
        public static string PlayMakerStudentUnitypackage190 = "158721eab71f1714c9676bb47ac2a371";   
        public static string PlayMakerUnitypackage191 = "39bb10697963712438a72dd20e0435c9";           
        public static string PlayMakerStudentUnitypackage191 = "a0f1983f3632a6d4eb4779df06b35e65";   
        public static string PlayMakerUnitypackage192 = "8e350883416051c4185c056cafb17382";           
        public static string PlayMakerStudentUnitypackage192 = "5397507c2e0c8754aa77587c490e44b2";  
        public static string PlayMakerUnitypackage193 = "c694d774dfdb02a4fb3f67b851678d25";           
        public static string PlayMakerStudentUnitypackage193 = "3b43de08dc660944b98bfa42fe754f8b";   
        public static string PlayMakerUnitypackage194 = "2cd13ef21e3eb3347bfe52c186e2eec4";           
        public static string PlayMakerStudentUnitypackage194 = "43a21ae2508161d4fbf16a3acd72eee7";  
        public static string PlayMakerUnitypackage195 = "0fede577d30b6a840b9197c50d7bfd5b";           
        public static string PlayMakerStudentUnitypackage195 = "4fad501f687ad0143ac8b714ab036306"; 
        public static string PlayMakerUnitypackage196 = "368eae1f0540d1e479cef9f883154683";           
        public static string PlayMakerStudentUnitypackage196 = "0ae77ac19380be04f9f3a7c5193959af";
        public static string PlayMakerUnitypackage197 = "28b3543a4221f864bb27a40b79746b65";           
        public static string PlayMakerStudentUnitypackage197 = "e572e79e9b55e844aa09d14d150502c1";

        // Latest Install

        public static string PlayMakerUnitypackage198 = "bc59d05a08a00e24a944cfbc8bf2ce5f";           
        public static string PlayMakerStudentUnitypackage198 = "1097a284faadb8644930018aabdfee2d";


        public static string LatestInstall
        {
            get { return PlayMakerUnitypackage198; }
        }

        public static string LatestStudentInstall
        {
            get { return PlayMakerStudentUnitypackage198; }
        }

        public static bool IsStudentVersionInstall()
        {
            var fullVersion = AssetDatabase.GUIDToAssetPath(LatestInstall);
            if (!string.IsNullOrEmpty(fullVersion)) return false;
            var studentVersion = AssetDatabase.GUIDToAssetPath(LatestStudentInstall);
            return !string.IsNullOrEmpty(studentVersion);
        }

        public static string GetFullAssetPathToLatestInstall()
        {
            return GetFullAssetPath(LatestInstall);
        }

        public static string GetFullAssetPath(string assetGUID)
        {
            var path = AssetDatabase.GUIDToAssetPath(assetGUID);
            if (!string.IsNullOrEmpty(path))
            {
                // strip Assets from asset path since it's in dataPath
                path = Application.dataPath + path.Substring(6); 
            }
            return path;
        }
    }
}

