using System;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace HutongGames.PlayMakerEditor
{
    /// <summary>
    /// Adds Playmaker defines to project
    /// Other tools can now use #if PLAYMAKER
    /// Package as source code so user can remove or modify
    /// </summary>
    [InitializeOnLoad]
    public class PlayMakerDefines
    {
        static PlayMakerDefines()
        {
            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER");

            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER_1_9");
            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER_1_9_8");
            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER_1_8_OR_NEWER");
            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER_1_8_5_OR_NEWER");
            DefinesHelper.AddSymbolToAllTargets("PLAYMAKER_1_9_OR_NEWER");
            
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_0");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_1");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_2");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_3");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_4");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_5");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_6");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_7");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_8");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_8_9");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_9_0");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_9_1");
            DefinesHelper.RemoveSymbolFromAllTargets("PLAYMAKER_1_9_6");

            UpdateTextMeshProDefines();
            UpdatePipelineDefines();
        }

        public static void AddScriptingDefineSymbolToAllTargets(string defineSymbol)
        {
            DefinesHelper.AddSymbolToAllTargets(defineSymbol);
        }

        public static void RemoveScriptingDefineSymbolFromAllTargets(string defineSymbol)
        {
            DefinesHelper.RemoveSymbolFromAllTargets(defineSymbol);
        }

        #region TextMeshPro

        private static void UpdateTextMeshProDefines()
        {
            if (TextMeshProIsPresent())
            {
                DefinesHelper.AddSymbol("PLAYMAKER_TMPRO");
            }
            else
            {
                DefinesHelper.RemoveSymbol("PLAYMAKER_TMPRO");
            }
        }

        private static bool TextMeshProIsPresent()
        {
            return PlayMakerEditorStartup.GetType("TMPro.TMP_Dropdown") != null;
        }

        #endregion

        #region Render Pipelines

        private enum PipelineType
        {
            Unsupported,
            BuiltInPipeline,
            UniversalPipeline,
            HDPipeline
        }

        private static void UpdatePipelineDefines()
        {
            var pipeline = GetPipeline();

            if (pipeline == PipelineType.UniversalPipeline)
            {
                DefinesHelper.AddSymbol("PLAYMAKER_URP");
            }
            else
            {
                DefinesHelper.RemoveSymbol("PLAYMAKER_URP");
            }
            if (pipeline == PipelineType.HDPipeline)
            {
                DefinesHelper.AddSymbol("PLAYMAKER_HDRP");
            }
            else
            {
                DefinesHelper.RemoveSymbol("PLAYMAKER_HDRP");
            }
        }

        /// <summary>
        /// Returns the type of renderpipeline that is currently running
        /// </summary>
        /// <returns></returns>
        private static PipelineType GetPipeline()
        {
#if UNITY_2019_1_OR_NEWER
        if (GraphicsSettings.renderPipelineAsset != null)
        {
            // SRP
            var srpType = GraphicsSettings.renderPipelineAsset.GetType().ToString();
            if (srpType.Contains("HDRenderPipelineAsset"))
            {
                return PipelineType.HDPipeline;
            }
            else if (srpType.Contains("UniversalRenderPipelineAsset") || srpType.Contains("LightweightRenderPipelineAsset"))
            {
                return PipelineType.UniversalPipeline;
            }
            else return PipelineType.Unsupported;
        }
#elif UNITY_2017_1_OR_NEWER
            if (GraphicsSettings.renderPipelineAsset != null)
            {
                // SRP not supported before 2019
                return PipelineType.Unsupported;
            }
#endif
            // no SRP
            return PipelineType.BuiltInPipeline;
        }

        #endregion
    }
}

