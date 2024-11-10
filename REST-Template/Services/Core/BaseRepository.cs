using Dapper;
using MySqlConnector;

namespace Backend.Services.Core
{
    public class BaseRepository
    {
        private readonly string connectionString;
        public BaseRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        protected async Task<List<T>> read<T>(string query, object? parameters = null) where T : class
        {
            try
            {
                List<T> data = new List<T>();
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    data = (await connection.QueryAsync<T>(query, parameters)).ToList();
                    await connection.CloseAsync();
                }
                return data;

            }
            catch (Exception)
            {
                return new List<T>();
            }
        }

        protected async Task<T?> readOne<T>(string query, object? parameters = null) where T: class
        {
            try
            {
                T data;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    data = await connection.QueryFirstAsync<T>(query, parameters);
                    await connection.CloseAsync();
                }
                return data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        protected async Task write(string query, object? parameters = null)
        {
            using (MySqlConnection  connection = new MySqlConnection (connectionString))
            {
                await connection.OpenAsync();
                await connection.QueryAsync(query, parameters);
                await connection.CloseAsync();
            }
        }

        protected async Task<IdGet> insertWithId(string query, object? parameters = null)
        {
            query += "\n select LAST_INSERT_ID() as id;";
            try
            {
                IdGet data;
                using (MySqlConnection connection = new MySqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    data = await connection.QueryFirstAsync<IdGet>(query, parameters);
                    await connection.CloseAsync();
                }
                return data;
            }
            catch (Exception)
            {
                throw new Exception("something went wrong");
            }
        }
    }
}
