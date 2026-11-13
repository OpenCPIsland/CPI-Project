using System;

namespace ClubPenguin.ObjectManipulation
{
	public class ObjectManipulationService
	{
		public Action<int, Action<bool>> ConfirmObjectRemoval;

		private StructurePlotManager structurePlotManager;

		public StructurePlotManager StructurePlotManager
		{
			get
			{
				return structurePlotManager;
			}
		}

		public ObjectManipulationService()
		{
			structurePlotManager = new StructurePlotManager();
		}
	}
}
