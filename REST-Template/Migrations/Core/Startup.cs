

namespace BreadAPI.migrations.core
{
    public class Startup : IHostedService
    {
        private readonly MigrationController _controller;
        public Startup(MigrationController controller)
        {
            _controller = controller;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _controller.StartMigration();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            return;
        }
    }
}
