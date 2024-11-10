namespace Backend.migrations.core
{
    public interface IMigration
    {
        public Task Up();
    }
}
