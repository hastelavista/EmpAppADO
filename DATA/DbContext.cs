using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace DATA
{
    public class DbContext
    {
        private readonly string _connectionString;
        public DbContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }
        public async Task<DataSet> ExecuteDataSetAsync(string query, SqlParameter[] parameters = null)

        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new (query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            using SqlDataAdapter adapter = new (cmd);
            DataSet dataset = new();
            await Task.Run(() => adapter.Fill(dataset));
            return dataset;
        }

        public async Task<DataTable> ExecuteDataTableAsync(string query, SqlParameter[] parameters = null)
        {
            var dataset = await ExecuteDataSetAsync(query, parameters);
            return dataset.Tables.Count > 0 ? dataset.Tables[0] : new DataTable();
        }

        public async Task<object> ExecuteScalarAsync(string query, SqlParameter[] parameters = null)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            await conn.OpenAsync();
            return await cmd.ExecuteScalarAsync();
        }

        public async Task<int> ExecuteNonQueryAsync(string query, SqlParameter[] parameters = null)
        {
            using SqlConnection conn = new(_connectionString);
            using SqlCommand cmd = new(query, conn);
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);
            await conn.OpenAsync();
            return await cmd.ExecuteNonQueryAsync();
        }


    }
}
