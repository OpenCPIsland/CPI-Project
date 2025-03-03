using System.Collections.Generic;

using UnityEngine;
using UnityEditor;

using Net.FabreJean.UnityEditor.MarkdownSharp;

namespace Net.FabreJean.UnityEditor
{
	#if UNITY_5
	[CustomEditor(typeof(DefaultAsset))]
	#else
	[CustomEditor(typeof(UnityEngine.Object))]
	#endif
	public class ObjectInspector : Editor
	{

		MarkdownGUI _markdownGui;
		Vector2 _scroll;
		void DrawMarkDownInspector()
		{
			GUI.enabled = true;
			if (_markdownGui == null)
			{
				_markdownGui = new MarkdownGUI();
				_markdownGui.ProcessSource
					(
						Utils.GetFileContents
							(
								AssetDatabase.GetAssetPath(target)
							)
					);
				_markdownGui.ProcessSource
					(
						Utils.GetFileContents
						(
						AssetDatabase.GetAssetPath(target)
						)
						);
			}

			_scroll = GUILayout.BeginScrollView(_scroll);
			
			if ( _markdownGui.OnGUILayout_MardkDownTextArea())
			{
				//Debug.Log("hello");
				Repaint();
			}

			GUILayout.EndScrollView();
		}

		#region Internal

		/// <summary>
		/// The is mark down file.
		/// </summary>
		bool isMarkDownFile;

		/// <summary>
		/// redirect to draw the parsed marked down file or the default inspector.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (isMarkDownFile)
			{
				DrawMarkDownInspector();
			}else{
				DrawDefaultInspector();
			}
		}

		/// <summary>
		/// Detect if we deal with an markdown file, because of its extension.
		/// </summary>
		protected virtual void OnEnable()
		{        
			string assetPath = AssetDatabase.GetAssetPath(target);
			if ((assetPath != null) && (assetPath.EndsWith(".md"))) {
				isMarkDownFile = true;
			}
		}	

		#endregion


	}
}

