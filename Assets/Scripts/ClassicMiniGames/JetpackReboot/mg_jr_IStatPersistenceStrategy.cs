namespace JetpackReboot
{
	public interface mg_jr_IStatPersistenceStrategy
	{
		void Save(mg_jr_PlayerStats _toSave);

		mg_jr_PlayerStats LoadOrCreate();
	}
}
