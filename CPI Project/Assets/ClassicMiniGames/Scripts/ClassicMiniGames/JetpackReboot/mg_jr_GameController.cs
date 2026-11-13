using DisneyMobile.CoreUnitySystems;
using UnityEngine.SceneManagement;

namespace JetpackReboot
{
	public class mg_jr_GameController : BaseGameController
	{
		protected void Start()
		{
			Initialize();
		}

		protected void Initialize()
		{
			InitManager initManager = new InitManager(base.EventDispatcher);
			initManager.AddInitAction(new InitActionFileManager());
			InitActionResourceManager initActionResourceManager = new InitActionResourceManager();
			initActionResourceManager.FailedCallback = OnRequiredResourceFailed;
			initManager.AddInitAction(initActionResourceManager);
			initManager.EventDispatcher.AddListener<InitCompleteEvent>(OnInitComplete);
			initManager.Process(base.Configurator);
		}

		protected bool OnInitComplete(InitCompleteEvent evt)
		{
			if (SceneManager.GetActiveScene().name == "mg_jr_LoadScene")
			{
				SceneManager.LoadScene("mg_jr_GameScene");
			}
			return false;
		}

		public void OnRequiredResourceFailed(string msg)
		{
			Logger.LogFatal(this, "Required resource download failed with message : " + msg);
		}
	}
}
