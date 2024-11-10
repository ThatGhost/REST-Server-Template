using Backend.Services.Core;
using System.Linq;

namespace Backend.migrations.core
{
    public class MigrationController: BaseRepository
    {
        private readonly IMigration[] _migrations;
        private readonly IHostApplicationLifetime _lifetime;

        public MigrationController(
            IConfiguration config,
            IEnumerable<IMigration> migrations,
            IHostApplicationLifetime lifetime) : base(config)
        {
            if (migrations == null) throw new Exception();
            _migrations = migrations.ToArray();
            _lifetime = lifetime;
        }

        public async Task StartMigration()
        {
            await initializeDatabase();
            await applyMigrations();
        }

        private async Task initializeDatabase()
        {
            await write(@"
                CREATE TABLE IF NOT EXISTS migrations(
	                name varchar(128) not null,
	                state varchar(64) not null,
	                date datetime not null
                );
            ");
        }

        private async Task applyMigrations()
        {
            List<MigrationData> migrationsInDb = await read<MigrationData>("Select name, state, date FROM migrations");

            // add migration that are failed or have not been executed
            List<IMigration> nonAddedMigrations = _migrations.Where(migration =>
            {
                MigrationData? migrationData = migrationsInDb.Find(db => db.name == migration.GetType().Name);
                if (migrationData == null) return true;
                if (migrationData.state == "failed") return true;

                return false;
            }).ToList();

            // do all migrations
            foreach (var migration in nonAddedMigrations)
            {
                try
                {
                    await migration.Up();
                    await write("delete from migrations where name = @name", new { name = migration.GetType().Name });
                    await write("REPLACE INTO migrations (name, state, date) VALUES (@name, @state, @date)", new
                    {
                        name = migration.GetType().Name,
                        state = "succes",
                        date = DateTime.Now,
                    });
                    
                }
                catch (Exception e)
                {
                    await write("REPLACE INTO migrations (name, state, date) VALUES (@name, @state, @date)", new
                    {
                        name = migration.GetType().Name,
                        state = "failed",
                        date = DateTime.Now,
                    });
                    _lifetime.StopApplication();
                    throw new Exception(e.Message);
                }
            }
        }

        private class MigrationData
        {
            public required string name { get; set; }
            public required string state { get; set; }
            public required string date { get; set; }
        }
    }
}
