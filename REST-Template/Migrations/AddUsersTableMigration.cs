using Backend.migrations.core;
using Backend.Services.Core;

namespace Backend.migrations
{
    public class AddUsersTableMigration : BaseRepository, IMigration
    {
        public AddUsersTableMigration(IConfiguration configuration) : base(configuration)
        {
        }

        public async Task Up()
        {
            await write(@"
                create table if not exists users(
	                email varchar(128) not null unique,
	                password varchar(256) not null,
	                id varchar(52) NOT NULL,
                    language varchar(5) NOT NULL DEFAULT 'en_US',
                    acces varchar(255) NOT NULL DEFAULT ''
                );
            ");
        }
    }
}
