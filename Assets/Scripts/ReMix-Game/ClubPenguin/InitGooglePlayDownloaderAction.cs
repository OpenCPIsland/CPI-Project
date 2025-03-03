using DevonLocalization.Core;
using Disney.LaunchPadFramework;
using Disney.LaunchPadFramework.Utility;
using Disney.MobileNetwork;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ClubPenguin
{
	[RequireComponent(typeof(InitConfiguratorAction))]
	[RequireComponent(typeof(InitCoreServicesAction))]
	public class InitGooglePlayDownloaderAction : InitActionComponent
	{
		public enum AndroidPermission
		{
			READ_EXTERNAL_STORAGE
		}

		private const string READ_EXTERNAL_STORAGE = "READ_EXTERNAL_STORAGE";

		private const string PERMISSION_GRANTED = "PERMISSION_GRANTED";

		private const string PERMISSION_DENIED = "PERMISSION_DENIED";

		private const string PERMISSION_NEVER = "PERMISSION_NEVER";

		public GameObject PermissionPrompt;

		public Text BodyText;

		public Text ButtonText;

		public Button PermissionButton;

		private string expansionPath = "";

		private string mainExpansionPath;

		private bool isDownloadingOBB = false;

		private Language language;

		private bool promptShowing = true;

		private bool permissionGranted = false;

		private bool permissionNever = false;

		private AndroidJavaClass permissionGranterClass;

		private AndroidJavaObject activity;

		private bool initialized;

		public static Action<bool> PermissionRequestCallback;

		public override bool HasSecondPass
		{
			get
			{
				return false;
			}
		}

		public override bool HasCompletedPass
		{
			get
			{
				return false;
			}
		}

		public override IEnumerator PerformFirstPass()
		{
			yield break;
		}

		private IEnumerator waitForOBB()
		{
			while (isDownloadingOBB && !hasOBB())
			{
				yield return new WaitForSeconds(0.5f);
			}
			isDownloadingOBB = false;
		}

		private bool hasExternalStorage()
		{
			return expansionPath != null;
		}

		private bool hasOBB()
		{
			mainExpansionPath = GooglePlayDownloader.GetMainOBBPath(expansionPath);
			return mainExpansionPath != null;
		}

		private bool useOBB()
		{
			Configurator configurator = Service.Get<Configurator>();
			if (configurator.IsSystemEnabled("SKU"))
			{
				IDictionary<string, object> dictionaryForSystem = configurator.GetDictionaryForSystem("SKU");
				IDictionary<string, object> dictionary = dictionaryForSystem["values"] as IDictionary<string, object>;
				IDictionary<string, object> sKUDictionaryFromSystemDictionary = ConfigurationHelper.GetSKUDictionaryFromSystemDictionary(dictionary["Android"].AsDic(), "Android");
				object value;
				if (sKUDictionaryFromSystemDictionary.TryGetValue("UseAPKExpansionFiles", out value))
				{
					return (bool)value;
				}
			}
			else
			{
				Log.LogError(this, "ApplicationConfig.txt did not contian SKU system.");
			}
			return false;
		}

		private IEnumerator waitForPermission()
		{
			promptShowing = true;
			showStepOneMessage();
			while (!permissionGranted)
			{
				if (promptShowing)
				{
					yield return new WaitForSeconds(0.5f);
				}
				else if (permissionNever)
				{
					promptShowing = true;
					showStepFiveMessage();
				}
				else
				{
					promptShowing = true;
					showStepThreeMessage();
				}
			}
		}

		private void showPrompt(string bodyText, bool showButton = true)
		{
			PermissionButton.onClick.AddListener(showPermissionPrompt);
			BodyText.text = bodyText;
			string text = "";
			Language language = this.language;
			text = ((language != Language.es_LA) ? "OK" : "ACEPTAR");
			ButtonText.text = text;
			PermissionButton.gameObject.SetActive(showButton);
			PermissionPrompt.SetActive(true);
		}

		private void showStepOneMessage()
		{
			promptShowing = true;
			string text = "";
			switch (language)
			{
			case Language.pt_BR:
				text = "Para aproveitar o visual, os sons e a ação da Ilha do Club Penguin, você precisará permitir o acesso ao armazenamento do dispositivo";
				break;
			case Language.fr_FR:
				text = "Pour profiter des paysages, bruits et activités de l'Île de Club Penguin, tu dois autoriser l'accès au stockage de l'appareil";
				break;
			case Language.es_LA:
				text = "Para disfrutar de las vistas, sonidos y acción de la Isla de Club Penguin, necesitarás permitir el acceso al almacenamiento del dispositivo";
				break;
			default:
				text = "To enjoy the sights, sounds, and action of Club Penguin Island, you’ll need to allow device storage access";
				break;
			}
			showPrompt(text);
		}

		private void showStepThreeMessage()
		{
			promptShowing = true;
			string text = "";
			switch (language)
			{
			case Language.pt_BR:
				text = "Opa! Para jogar Ilha do Club Penguin, é preciso ter uma permissão de armazenamento. Permita o acesso e repita.";
				break;
			case Language.fr_FR:
				text = "Oups ! L'accès au stockage est requis pour jouer à l'Île de Club Penguin. Autorise l'accès puis réessaie";
				break;
			case Language.es_LA:
				text = "¡Uy! Se requiere permiso de almacenamiento para jugar a la Isla de Club Penguin. Por favor, autoriza el acceso y vuelve a intentarlo";
				break;
			default:
				text = "Oops! Storage permission is required to play Club Penguin Island. Please allow access and try again";
				break;
			}
			showPrompt(text);
		}

		private void showStepFiveMessage()
		{
			promptShowing = true;
			string text = "";
			switch (language)
			{
			case Language.pt_BR:
				text = "Opa! Para jogar Ilha do Club Penguin, é preciso ter uma permissão de armazenamento. Para dar a permissão para o aplicativo, acesse as CONFIGURAÇÕES do dispositivo.";
				break;
			case Language.fr_FR:
				text = "Oups ! L'accès au stockage est requis pour jouer à l'Île de Club Penguin. Pour autoriser l'accès, rends-toi dans les PARAMÈTRES de ton appareil";
				break;
			case Language.es_LA:
				text = "¡Uy! Se requiere permiso de almacenamiento para jugar a la Isla de Club Penguin. Para dar autorización a la app, ve a los AJUSTES de tu dispositivo";
				break;
			default:
				text = "Oops! Storage permission is required to play Club Penguin Island. To give the app permission, go to your device SETTINGS";
				break;
			}
			showPrompt(text, false);
		}

		private void showPermissionPrompt()
		{
			PermissionButton.onClick.RemoveListener(showPermissionPrompt);
			PermissionPrompt.SetActive(false);
			if (!permissionNever)
			{
				GrantPermission(AndroidPermission.READ_EXTERNAL_STORAGE);
			}
		}

		private void initializePermissionGranter()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			activity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			permissionGranterClass = new AndroidJavaClass("com.cpisland.unityplugins.PermissionGranter");
			initialized = true;
		}

		private void GrantPermission(AndroidPermission permission)
		{
			if (!initialized)
			{
				initializePermissionGranter();
			}
			permissionGranterClass.CallStatic("grantPermission", activity, (int)permission);
		}

		private void permissionRequestCallbackInternal(string message)
		{
			permissionGranted = (message == "PERMISSION_GRANTED");
			permissionNever = (message == "PERMISSION_NEVER");
			promptShowing = false;
		}
	}
}
