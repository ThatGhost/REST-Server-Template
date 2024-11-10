using Backend.migrations.core;

namespace Backend.migrations
{
    public class MigrationRegistry
    {
        public static void registerServices(IServiceCollection container)
        {
            container.AddTransient<MigrationController>();
            container.AddTransient<IMigration, AddUsersTableMigration>();
        }
    }
}
