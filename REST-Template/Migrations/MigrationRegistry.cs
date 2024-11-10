using Backend.migrations.core;

namespace Backend.migrations
{
    public class MigrationRegistry
    {
        public static void registerServices(IServiceCollection container)
        {
            // Migrations are executed in order from top to bottom
            container.AddTransient<MigrationController>();
            container.AddTransient<IMigration, AddUsersTableMigration>();
        }
    }
}
