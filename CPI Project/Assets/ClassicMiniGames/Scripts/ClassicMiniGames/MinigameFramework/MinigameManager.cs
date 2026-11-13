using ClubPenguin.Classic;
using DisneyMobile.CoreUnitySystems;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;

namespace MinigameFramework
{
    public class MinigameManager : MonoBehaviour
    {
        private static MinigameManager m_instance = null;

        private Minigame m_activeMinigame;

        public static bool IsPaused
        {
            get
            {
                return !(Instance.m_activeMinigame != null) || Instance.m_activeMinigame.IsPaused;
            }
        }

        public int PlayerCoins
        {
            get;
            private set;
        }

        public static MinigameManager Instance
        {
            get
            {
                return m_instance;
            }
        }

        public MinigameManager()
        {
            PlayerCoins = 0;
            m_activeMinigame = null;
        }

        public void Awake()
        {
            if (m_instance == null)
            {
                UnityEngine.Object.DontDestroyOnLoad(this);
                m_instance = this;
            }
            else
            {
                DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "Attempted to create multiple MinigameManagers!");
            }
        }

        public void Shutdown()
        {
            m_instance = null;
            UnityEngine.Object.Destroy(base.gameObject);
        }

        public void ShowMinigame(EMinigameTypes _type)
        {
            string initialScene = MinigameFactory.GetInitialScene(_type);
            Debug.Log("Loading scene: " + initialScene);
            SceneManager.LoadScene(initialScene);
        }

        public void OnMiniGameLoaded(Minigame _minigame)
        {
            m_activeMinigame = _minigame;
            m_activeMinigame.MusicVolume = ClassicMiniGames.MainGameMusicVolume;
            m_activeMinigame.SFxVolume = ClassicMiniGames.MainGameSFXVolume;
            m_activeMinigame.ResumeGame();
        }

        public static Minigame GetActive()
        {
            Minigame minigame = null;
            if (m_instance != null)
            {
                minigame = m_instance.m_activeMinigame;
                if (minigame != null)
                {
                    minigame.MusicVolume = ClassicMiniGames.MainGameMusicVolume;
                    minigame.SFxVolume = ClassicMiniGames.MainGameSFXVolume;
                }
            }
            return minigame;
        }

        public static T GetActive<T>() where T : Minigame
        {
            T result = null;
            if (m_instance != null)
            {
                return m_instance.m_activeMinigame as T;
            }
            return result;
        }

        public void ExitMinigame()
        {
            BaseGameController.DestroyInstance();
            ClassicMiniGames.AddCoinsToAccount(Instance.PlayerCoins);
            Instance.PlayerCoins = 0;
            m_activeMinigame = null;
            SceneManager.LoadScene("ClassicMiniGames");
            Resources.UnloadUnusedAssets();
        }

        public Color GetPenguinColor()
        {
            try
            {
                object dataEntityCollection = GetServiceInstance("ClubPenguin.Core.CPDataEntityCollection");
                if (dataEntityCollection != null)
                {
                    object localPlayerHandle = GetInstancePropertyValue(dataEntityCollection, "LocalPlayerHandle");
                    if (IsNullHandle(localPlayerHandle))
                    {
                        localPlayerHandle = InvokeInstanceMethod(dataEntityCollection, "FindEntityByName", new object[1] { "LocalPlayer" });
                    }

                    if (!IsNullHandle(localPlayerHandle))
                    {
                        Type avatarDetailsType = FindTypeInDomain("ClubPenguin.AvatarDetailsData");
                        if (avatarDetailsType != null)
                        {
                            object avatarDetails = TryGetComponentGeneric(dataEntityCollection, localPlayerHandle, avatarDetailsType);
                            if (avatarDetails != null)
                            {
                                object bodyColorValue = GetInstancePropertyValue(avatarDetails, "BodyColor");
                                if (bodyColorValue is Color)
                                {
                                    return (Color)bodyColorValue;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception message)
            {
                DisneyMobile.CoreUnitySystems.Logger.LogWarning(this, "GetPenguinColor() reflection failed. Falling back to default. " + message);
            }

            return new Color(0f, 1f, 1f);
        }

        private static Type FindTypeInDomain(string fullName)
        {
            Type type = Type.GetType(fullName);
            if (type != null)
            {
                return type;
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                try
                {
                    type = assemblies[i].GetType(fullName);
                    if (type != null)
                    {
                        return type;
                    }
                }
                catch
                {
                }
            }
            return null;
        }

        private static object GetServiceInstance(string serviceInterfaceFullName)
        {
            Type serviceType = FindTypeInDomain("Disney.MobileNetwork.Service");
            if (serviceType == null)
            {
                return null;
            }

            Type targetType = FindTypeInDomain(serviceInterfaceFullName);
            if (targetType == null)
            {
                return null;
            }

            MethodInfo isSet = serviceType.GetMethod("IsSet", BindingFlags.Public | BindingFlags.Static);
            MethodInfo get = serviceType.GetMethod("Get", BindingFlags.Public | BindingFlags.Static);
            if (isSet == null || get == null)
            {
                return null;
            }

            try
            {
                bool flag = (bool)isSet.MakeGenericMethod(targetType).Invoke(null, null);
                if (!flag)
                {
                    return null;
                }
                return get.MakeGenericMethod(targetType).Invoke(null, null);
            }
            catch
            {
                return null;
            }
        }

        private static object GetInstancePropertyValue(object instance, string propertyName)
        {
            if (instance == null)
            {
                return null;
            }

            PropertyInfo property = instance.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
            {
                return null;
            }

            try
            {
                return property.GetValue(instance, null);
            }
            catch
            {
                return null;
            }
        }

        private static object InvokeInstanceMethod(object instance, string methodName, object[] args)
        {
            if (instance == null)
            {
                return null;
            }

            try
            {
                MethodInfo method = instance.GetType().GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                {
                    return null;
                }
                return method.Invoke(instance, args);
            }
            catch
            {
                return null;
            }
        }

        private static bool IsNullHandle(object handle)
        {
            if (handle == null)
            {
                return true;
            }

            try
            {
                PropertyInfo property = handle.GetType().GetProperty("IsNull", BindingFlags.Public | BindingFlags.Instance);
                if (property != null && property.PropertyType == typeof(bool))
                {
                    return (bool)property.GetValue(handle, null);
                }
            }
            catch
            {
            }

            return false;
        }

        private static object TryGetComponentGeneric(object dataEntityCollection, object handle, Type componentType)
        {
            if (dataEntityCollection == null || handle == null || componentType == null)
            {
                return null;
            }

            try
            {
                MethodInfo targetMethod = null;
                MethodInfo[] methods = dataEntityCollection.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
                for (int i = 0; i < methods.Length; i++)
                {
                    MethodInfo methodInfo = methods[i];
                    if (methodInfo.Name == "TryGetComponent" && methodInfo.IsGenericMethodDefinition)
                    {
                        ParameterInfo[] parameters = methodInfo.GetParameters();
                        if (parameters != null && parameters.Length == 2 && parameters[1].IsOut)
                        {
                            targetMethod = methodInfo;
                            break;
                        }
                    }
                }

                if (targetMethod == null)
                {
                    return null;
                }

                MethodInfo generic = targetMethod.MakeGenericMethod(componentType);
                object[] args = new object[2] { handle, null };
                object result = generic.Invoke(dataEntityCollection, args);
                if (result is bool && (bool)result)
                {
                    return args[1];
                }
            }
            catch
            {
            }

            return null;
        }

        public string GetPenguinName()
        {
            return "Penguin";
        }

        public void OnMinigameQuit()
        {
            m_activeMinigame.OnQuit();
        }

        public void OnMinigameEnded()
        {
            if (m_activeMinigame.CoinsEarned > 0)
            {
                PlayerCoins += m_activeMinigame.CoinsEarned;
                UIManager.Instance.OpenScreen("mg_ResultScreen", false, OnResultsClosed, null);
                m_activeMinigame.PauseGame();
            }
            else
            {
                ExitMinigame();
            }
        }

        private void OnResultsClosed(UIControlBase _screen)
        {
            ExitMinigame();
        }
    }
}
