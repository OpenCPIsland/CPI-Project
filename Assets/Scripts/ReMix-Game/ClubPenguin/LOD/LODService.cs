using System;

namespace ClubPenguin.LOD
{
	public class LODService
	{
		public LODManager Manager
		{
			get;
			set;
		}

		public LODRequest Request(LODRequestData requestData, bool attemptSpawn = true)
		{
			if (Manager == null)
			{
				throw new ArgumentException("Called Request with a null entity", "manager");
			}
			return Manager.Request(requestData, attemptSpawn);
		}

		public void RemoveRequest(LODRequest request)
		{
			if (Manager == null)
			{
				throw new ArgumentException("Called RemoveRequest with a null entity", "manager");
			}
			Manager.RemoveRequest(request);
		}

		public void PauseRequest(LODRequest request)
		{
			if (Manager == null)
			{
				throw new ArgumentException("Called RemoveRequest with a null entity", "manager");
			}
			Manager.PauseRequest(request);
		}

		public void UnpauseRequest(LODRequest request)
		{
			if (Manager == null)
			{
				throw new ArgumentException("Called RemoveRequest with a null entity", "manager");
			}
			Manager.UnpauseRequest(request);
		}

		public void SetupComplete(string type)
		{
			Manager.SetupComplete(type);
		}
	}
}
