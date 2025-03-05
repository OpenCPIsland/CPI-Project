using UnityEditor;
using UnityEngine;

namespace ASoliman.Utils.ClipboardPlus
{
    /// <summary>
    /// Provides Unity context menu integration for the Clipboard Plus system, enabling quick access to copy and paste operations.
    /// </summary>
    public static class ClipboardContextMenu
    {
        /// <summary>
        /// Adds the selected component to the clipboard system.
        /// Appears as "Copy" under "Clipboard Plus" in the component's context menu.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clipboard Plus/Copy")]
        private static void CopyComponent(MenuCommand command)
        {
            var component = command.context as Component;
            if (component != null)
            {
                ClipboardManager.Instance.AddClip(component, ClipType.Component);
            }
        }
        
        /// <summary>
        /// Pastes the most recent compatible clip onto the selected component.
        /// Appears as "Quick Paste (Recent)" under "Clipboard Plus" in the component's context menu.
        /// Only pastes if a recent clip of the same component type exists.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clipboard Plus/Quick Paste (Recent)")]
        private static void PasteRecentComponent(MenuCommand command)
        {
            var targetComponent = command.context as Component;
            if (targetComponent != null)
            {
                var recentClip = ClipboardManager.Instance.GetRecentClipOfType(targetComponent.GetType());
                if (recentClip != null)
                {
                    ClipboardManager.Instance.PasteClip(recentClip, targetComponent);
                }
            }
        }

        /// <summary>
        /// Opens the Clipboard Plus window with the selected component as the paste target.
        /// Appears as "Paste (Select)" under "Clipboard Plus" in the component's context menu.
        /// Allows users to choose which clip to paste from the clipboard window.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clipboard Plus/Paste (Select)")]
        private static void PasteComponentValue(MenuCommand command)
        {
            var window = ClipboardEditorWindow.ShowWindow();
            window.SetPasteTarget(command.context as Component);
        }
    }
}