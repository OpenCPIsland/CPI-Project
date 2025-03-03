using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using System.IO;

#pragma warning disable 618

namespace Net.FabreJean.UnityEditor
{
	/// <summary>
	/// Ecosystem utils. Set of common methods and tools.
	/// </summary>
	public class EcosystemUtils {

		/// <summary>
		/// Parses an url query string like ?variable=value&anotherVariable
		/// </summary>
		/// <returns>The query string.</returns>
		/// <param name="query">Query.</param>
		public static Dictionary<string, string> ParseUrlQueryParameters(String query)
		{

			Dictionary<string,string> _params = new Dictionary<string,string>();
			
			if (String.IsNullOrEmpty(query))
			{
				return _params;
			}
			
			return new Uri(query).Query.TrimStart('?')
				.Split(new[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries)
					.Select(parameter => parameter.Split(new[] { '=' }, StringSplitOptions.RemoveEmptyEntries))
					.GroupBy(parts => parts[0],
					         parts => parts.Length > 2 ? string.Join("=", parts, 1, parts.Length - 1) : (parts.Length > 1 ? parts[1] : ""))
					.ToDictionary(grouping => grouping.Key,
					              grouping => WWW.UnEscapeURL(
																string.Join(",", grouping.ToArray())
															).Trim()
					              );


//			Dictionary<String, String> queryDict = new Dictionary<string, string>();
//			foreach (String token in query.TrimStart(new char[] { '?' }).Split(new char[] { '&', ';' }, StringSplitOptions.RemoveEmptyEntries))
//			{
//				string[] parts = token.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
//				if (parts.Length == 2)
//					queryDict[parts[0].Trim()] = WWW.UnEscapeURL(parts[1]).Trim();
//				else
//					queryDict[parts[0].Trim()] = "";
//			}
//			return queryDict;
		}

		
		public static string GetUrlQueryParameter(string url,string key)
		{
			Dictionary<string,string> _params = ParseUrlQueryParameters(url);
			
			if (_params.ContainsKey(key))
			{
				return _params[key];
			}
			
			return null;
		}

		public static string AddParameterToUrlQuery(string url,string key,string value)
		{
			
			if (String.IsNullOrEmpty(url))
			{
				return url;
			}
			
			string _result = url;
			if (! _result.Contains("?"))
			{
				_result += "?";
			}else{
				_result += "&";
			}
			
			_result += Uri.EscapeDataString(key)+"="+Uri.EscapeDataString(value);
			
			return _result;
		}

		/// <summary>
		/// Extracts the meta data from text. expect a json content encapsulated between EcoMetaStart and EcoMetaEnd
		/// </summary>
		/// <returns>The meta data from text.</returns>
		/// <param name="text">Text.</param>
		public static Hashtable ExtractEcoMetaDataFromText(string text)
		{
			
			// check for Meta data
			Match match = Regex.Match(text,@"(?<=EcoMetaStart)[^>]*(?=EcoMetaEnd)",RegexOptions.IgnoreCase);
			
			// Here we check the Match instance.
			if (match.Success)
			{
				//	Debug.Log("we have meta data :" + match.Value);
				return  (Hashtable)JSON.JsonDecode(match.Value);
			}

			return new Hashtable();
		}

		public string InsertEcoMetaDataToText(string originalText,Hashtable json)
		{
			//string jsonString = JSON.JsonEncode(json);

			//string ecoMetaContent = "EcoMetaStart\n"+jsonString+"\nEcoMetaEnd";

			string modifiedText = "";

			return modifiedText;
		}


		/// <summary>
		//	This makes it easy to create, name and place unique new ScriptableObject asset files.
		/// </summary>
		public static void CreateAsset<T> (string name="") where T : ScriptableObject
		{
			T asset = ScriptableObject.CreateInstance<T> ();
			
			string path = AssetDatabase.GetAssetPath (Selection.activeObject);
			if (path == "") 
			{
				path = "Assets";
			} 
			else if (Path.GetExtension (path) != "") 
			{
				path = path.Replace (Path.GetFileName (AssetDatabase.GetAssetPath (Selection.activeObject)), "");
			}
			
			string _name = string.IsNullOrEmpty(name)? "New " + typeof(T).ToString() : name ;
			
			string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath (path + "/" + _name + ".asset");
			
			AssetDatabase.CreateAsset (asset, assetPathAndName);
			
			AssetDatabase.SaveAssets ();
			AssetDatabase.Refresh();
			EditorUtility.FocusProjectWindow ();
			Selection.activeObject = asset;
		}


	}
}
