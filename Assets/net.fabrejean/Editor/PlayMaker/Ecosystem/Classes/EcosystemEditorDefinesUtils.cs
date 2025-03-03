using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Net.FabreJean.UnityEditor
{
	[InitializeOnLoad]
	class EcosystemEditorDefinesUtils
	{
		static EcosystemEditorDefinesUtils ()
		{
			
			#if ! ECOSYSTEM 
			Debug.Log("Setting Up Ecosystem Scripting define symbol 'ECOSYSTEM'"); 
			Utils.MountScriptingDefineSymbolToAllTargets ("ECOSYSTEM");
			#endif


			#if ! ECOSYSTEM_0_6
			Debug.Log("Setting Up Ecosystem Scripting define symbol 'ECOSYSTEM_0_6'"); 
			Utils.MountScriptingDefineSymbolToAllTargets("ECOSYSTEM_0_6");
			#endif

			#if ! ECOSYSTEM_0_6_OR_NEWER
			Debug.Log("Setting Up Ecosystem Scripting define symbol 'ECOSYSTEM_0_6_OR_NEWER'"); 
			Utils.MountScriptingDefineSymbolToAllTargets("ECOSYSTEM_0_6_OR_NEWER");
			#endif
		}
	}
}