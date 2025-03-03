using DisneyMobile.CoreUnitySystems;
using UnityEngine.SceneManagement;

namespace BeanCounter
{
	public class mg_bc_GameController : BaseGameController
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
			if (SceneManager.GetActiveScene().name == "mg_bc_LoadScene")
			{
				SceneManager.LoadScene("mg_bc_GameScene");
			}
			return false;
		}

		public void OnRequiredResourceFailed(string msg)
		{
			Logger.LogFatal(this, "Required resource download failed with message : " + msg);
		}
	}
}
