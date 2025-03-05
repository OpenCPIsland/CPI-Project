#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Gaskellgames.EditorOnly
{
    /// <summary>
    /// Code created by Gaskellgames
    /// </summary>
    
    public abstract class GGEditorWindow_SplitView : GGEditorWindow
    {
        #region Variables
        
        private Rect cursorChangeRect;
        private float windowMinWidth = 150f;
        private bool resizeActive;
        private Vector2 leftPageScrollPos;
        private Vector2 rightPageScrollPos;
        
        protected float resizeBarPosition;
        protected new float pageWidth => Screen.width - (resizeBarPosition + shadowBuffer + singleLine);
        protected new float bannerWidth => pageWidth;
        protected new float bannerHeight => bannerWidth * banner.height / banner.width;
        protected int pageXMin => (int)(shadowBuffer + spacing);
        protected int pageYMin => (int)(shadowBuffer + spacing + (EditorWindowUtility.settings.showPackageBanners ? bannerHeight : 0));
        
        #endregion

        //----------------------------------------------------------------------------------------------------
        
        #region Virtual Functions [Implement in child]

        /// <summary>
        /// OnGUI method for the left hand pane
        /// </summary>
        protected abstract void OnLeftPageGUI();

        /// <summary>
        /// OnGUI method for the right hand pane
        /// </summary>
        protected abstract void OnRightPageGUI();
        
        #endregion

        //----------------------------------------------------------------------------------------------------

        #region Editor Loop

        protected override void OnBecameVisible()
        {
            base.OnBecameVisible();
            InitialiseSplitView();
        }

        protected override void OnFocus()
        {
            base.OnFocus();
            ValidateResizeBar();
            cursorChangeRect = new Rect(resizeBarPosition, toolbarHeight, shadowBuffer, Screen.height * 1.2f);
        }

        protected override void OnLostFocus()
        {
            base.OnLostFocus();
            ValidateResizeBar();
            cursorChangeRect = new Rect(resizeBarPosition, toolbarHeight, shadowBuffer, Screen.height * 1.2f);
        }

        protected override void OnPageGUI()
        {
            // draw menu, resize bar and the selected page ...
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal(GUILayout.Width(resizeBarPosition));
            GUILayout.BeginVertical();
            
            // ... draw left pane background & call OnLeftPageGUI
            EditorGUI.DrawRect(new Rect(0, toolbarHeight, resizeBarPosition, Screen.height - toolbarHeight), InspectorExtensions.backgroundNormalColor);
            EditorGUI.DrawRect(new Rect(0, toolbarHeight - 1, Screen.width, 1), InspectorExtensions.backgroundSeperatorColorDark);
            float scrollHeight = Screen.height - (toolbarHeight + scrollBarOffset + singleLine);
            leftPageScrollPos = EditorGUILayout.BeginScrollView(leftPageScrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(scrollHeight));
            OnLeftPageGUI();
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            
            // ... draw page background & resize bar
            EditorGUI.DrawRect(new Rect(resizeBarPosition, toolbarHeight, Screen.width - resizeBarPosition, Screen.height - toolbarHeight), InspectorExtensions.backgroundNormalColorDark);
            EditorGUI.DrawRect(new Rect(resizeBarPosition, toolbarHeight, Screen.width - resizeBarPosition, ShadowHeight), InspectorExtensions.backgroundShadowColor);
            ResizeBar();
            
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            
            // ... draw right pane background & call OnRightPageGUI draw (draws banner if one exists)
            rightPageScrollPos = EditorGUILayout.BeginScrollView(rightPageScrollPos, GUILayout.ExpandWidth(true), GUILayout.Height(scrollHeight));
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace(); // for some reason this recalculates the width correctly?
            GUILayout.EndHorizontal();
            DrawPageBanner();
            OnRightPageGUI();
            EditorGUILayout.EndScrollView();
            
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Left Page, Resize Bar & Right Page

        private void InitialiseSplitView()
        {
            ValidateResizeBar();
            cursorChangeRect = new Rect(resizeBarPosition, toolbarHeight, shadowBuffer, Screen.height * 1.2f);
        }

        private void ValidateResizeBar()
        {
            if (resizeBarPosition <= windowMinWidth)
            {
                resizeBarPosition = windowMinWidth;
                cursorChangeRect.Set(resizeBarPosition, toolbarHeight, cursorChangeRect.width, cursorChangeRect.height);
            }
            else if (Screen.width - windowMinWidth <= resizeBarPosition)
            {
                resizeBarPosition = Screen.width - windowMinWidth;
                cursorChangeRect.Set(resizeBarPosition, toolbarHeight, cursorChangeRect.width, cursorChangeRect.height);
            }
        }
        
        private void ResizeBar()
        {
            EditorGUI.DrawRect(cursorChangeRect, InspectorExtensions.backgroundShadowColor);
            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeHorizontal);

            // enable resize
            if (Event.current.type == EventType.MouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
            {
                resizeActive = true;
            }

            if (resizeActive)
            {
                // get resize bar position
                float mouseX = Mathf.Max(0, Event.current.mousePosition.x);
                if (mouseX < 0 + windowMinWidth)
                {
                    resizeBarPosition = windowMinWidth;
                }
                else if (Screen.width - windowMinWidth < mouseX)
                {
                    resizeBarPosition = Screen.width - windowMinWidth;
                }
                else
                {
                    resizeBarPosition = mouseX;
                }

                // set resize bar position
                cursorChangeRect.Set(resizeBarPosition, toolbarHeight, cursorChangeRect.width, cursorChangeRect.height);
                
                // Update visuals
                Repaint();
            }

            // disable resize
            if (Event.current.rawType == EventType.MouseUp)
            {
                resizeActive = false;
            }
        }

        #endregion
        
        //----------------------------------------------------------------------------------------------------

        #region Banner

        private void DrawPageBanner()
        {
            // draw banner if turned on in Gaskellgames settings
            if (!EditorWindowUtility.settings.showPackageBanners) { return; }
            if (!banner) { return; }
            
            Rect rect = GUILayoutUtility.GetRect(pageWidth, bannerHeight);
            rect.x += shadowBuffer;
            GUI.DrawTexture(rect, banner);
        }

        #endregion

    } // class end
}
#endif