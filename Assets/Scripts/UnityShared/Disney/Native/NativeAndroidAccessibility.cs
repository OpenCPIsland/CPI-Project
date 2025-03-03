#if UNITY_ANDROID
using System.Collections;
using Disney.MobileNetwork;
using UnityEngine;
#endif
namespace Disney.Native
{
	public class NativeAndroidAccessibility : NativeAccessibility
	{
#if UNITY_ANDROID
        private AndroidJavaClass JavaClass = null;

        public NativeAndroidAccessibility()
        {
            Initialize();
        }

        public void Initialize()
        {
            JavaClass = new AndroidJavaClass("com.disney.nativeaccessibility.NativeAccessibility");
        }

        public override int GetAccessibilityLevel()
        {
            return JavaClass.CallStatic<int>("GetAccessibilityLevel", new object[0]);
        }

        public override bool IsSwitchControlEnabled()
        {
            return JavaClass.CallStatic<bool>("IsSwitchControlEnabled", new object[0]);
        }

        public override bool IsVoiceOverEnabled()
        {
            return JavaClass.CallStatic<bool>("IsVoiceOverEnabled", new object[0]);
        }

        public override bool IsDisplayZoomEnabled()
        {
            return false;
        }

        public override void RemoveView(int aId)
        {
            JavaClass.CallStatic("RemoveView", aId);
        }

        public override void RenderText(int aId, Rect aRect, string aLabel)
        {
            JavaClass.CallStatic("RenderText", aId, (int)aRect.x, (int)aRect.y, (int)aRect.width, (int)aRect.height, aLabel);
        }

        public override void RenderButton(int aId, Rect aRect, string aLabel)
        {
            JavaClass.CallStatic("RenderButton", aId, (int)aRect.x, (int)aRect.y, (int)aRect.width, (int)aRect.height, aLabel);
        }

        public override void SelectElement(int aId)
        {
            JavaClass.CallStatic("SelectElement", aId);
        }

        public IEnumerator SelectElementDelay(int aId)
        {
            yield return new WaitForSeconds(1f);
            SelectElement(aId);
        }

        public override void HandleError(int aId)
        {
            StartCoroutine(SelectElementDelay(aId));
        }

        public override void ClearAllElements()
        {
            JavaClass.CallStatic("ClearAllElements");
        }

        public override void UpdateView(int aId, Rect aRect, string aLabel)
        {
            if (aLabel == null)
            {
                aLabel = "";
            }
            JavaClass.CallStatic("UpdateView", aId, (int)aRect.x, (int)aRect.y, (int)aRect.width, (int)aRect.height, aLabel);
        }

        public override void Speak(string aTextToSpeak)
        {
            Service.Get<AccessibilityManager>().Speak(aTextToSpeak, 0.7f);
        }
#endif
    }

}
