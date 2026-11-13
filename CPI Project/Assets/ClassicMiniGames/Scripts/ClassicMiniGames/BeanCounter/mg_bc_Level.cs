using System.Collections.Generic;

namespace BeanCounter
{
	public class mg_bc_Level
	{
		public int Goal;

		public int MaxFlyingObjects;

		public float PowerupChance;

		public List<mg_bc_LevelObject> LevelObjects;

		public List<mg_bc_LevelObject> PowerUps;

		public int TotalObjectOdds = 0;

		public int TotalPowerUpOdds = 0;

		public int ThrownBagTypes = 0;

		public int WrongBagTypes = 0;

		public float SpawnDelay = 0f;

		public int DropAllowance = -1;
	}
}
