// (c) Copyright HutongGames, LLC 2010-2021. All rights reserved.

using System.Diagnostics.CodeAnalysis;
using UnityEditor;
using UnityEngine;

namespace HutongGames.PlayMakerEditor
{
    internal class AboutWindow : BaseEditorWindow
    {
        // used to fit window to contents
        private static bool heightHasBeenSet;

        public override void Initialize()
        {
            // initial fixed size
            minSize = new Vector2(264, 292);
            maxSize = new Vector2(264, 292);

            // updated to fit contents in OnGUI
            heightHasBeenSet = false;
        }

        public override void InitWindowTitle()
        {
            SetTitle(Strings.AboutPlaymaker_Title);
        }

        [SuppressMessage("ReSharper", "StringLiteralTypo")]
        public override void DoGUI()
        {
            FsmEditorStyles.Init();

            GUILayout.BeginVertical();

            FsmEditorGUILayout.PlaymakerHeader(this);

            GUILayout.Label(FsmEditorSettings.ProductCopyright, EditorStyles.miniLabel);

            GUILayout.Label(!EditorApp.IsSourceCodeVersion
                ? string.Format(Strings.AboutPlaymaker_Version_Info, VersionInfo.GetAssemblyInformationalVersion())
                : "Source Code Version");

            if (VersionInfo.PlayMakerVersionInfo != "")
            {
                EditorGUILayout.HelpBox(VersionInfo.PlayMakerVersionInfo, MessageType.None);
            }

            // Some actions use render pipeline defines
            // E.g., Set Material Color uses _BaseColor instead of _Color

#if PLAYMAKER_URP
            GUILayout.Label("Render Pipeline: URP");
#elif PLAYMAKER_HDRP
            GUILayout.Label("Render Pipeline: HDRP");
#else
            GUILayout.Label("Render Pipeline: Built-in");
#endif

#if PLAYMAKER_TMPRO

            // Some actions use this define to support TextMeshPro features.

            GUILayout.Label("TextMeshPro support enabled.");
#endif

            EditorGUILayout.HelpBox(string.Format(Strings.AboutPlaymaker_Special_Thanks,
                "Erin Ko, Jean Fabre, DjayDino, Lane Fox, Stephen Scott Day, Kemal Amarasingham, Bruce Blumberg, " +
                "Steve Gargolinski, Lee Hepler, Bart Simon, Lucas Meijer, Joachim Ante, " +
                "Jaydee Alley, James Murchison, XiaoHang Zheng, Andrzej Łukasik, " +
                "Vanessa Wesley, Marek Ledvina, Bob Berkebile, MaDDoX, gamesonytablet, " +
                "Marc 'Dreamora' Schaerer, Eugenio 'Ryo567' Martínez, Steven 'Nightreaver' Barthen, " +
                "Damiangto, VisionaiR3D, 黄峻, Nilton Felicio, Andre Dantas Lima, " +
                "Ramprasad Madhavan, and the PlayMaker Community!\r\n"),
                MessageType.None);

            if (GUILayout.Button(Strings.AboutPlaymaker_Release_Notes))
            {
                EditorCommands.OpenWikiPage(WikiPages.ReleaseNotes);
            }

            if (GUILayout.Button(Strings.AboutPlaymaker_Hutong_Games_Link))
            {
                Application.OpenURL(FsmEditorSettings.ProductUrl);
            }

            GUILayout.Space(5);

            GUILayout.EndVertical();

            if (!heightHasBeenSet && Event.current.type == EventType.Repaint)
            {
                SetWindowHeightToFitContents();
            }
        }

        private void SetWindowHeightToFitContents()
        {
            var height = GUILayoutUtility.GetLastRect().height + 10f;

            position.Set(position.x, position.y, 264, height);

            minSize = new Vector2(264, height);
            maxSize = new Vector2(264, height + 1);

            heightHasBeenSet = true;
        }
    }
}
