using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DATA.Models;
using Microsoft.Data.SqlClient;

namespace DATA.Repo
{
    public class UserRepo: IUserRepo
    {
        private readonly DbContext _db;
        public UserRepo(DbContext db)
        {
            _db = db;
        }

        #region EMPREPO
        public async Task<List<User>> GetAllUsers()
        {
            string query = "SELECT [Username],[Password], [IsAdmin] FROM [dbo].[Users]";
            var table = await _db.ExecuteDataTableAsync(query);

            var users = new List<User>();

            foreach (DataRow row in table.Rows)
            {
                users.Add(new User
                {
                    Username = row["Username"].ToString(),
                    Password = row["Password"].ToString(),
                    IsAdmin = Convert.ToBoolean(row["IsAdmin"])
                });
            }

            return users;

        }

        public async Task<User> GetUserByUsernameAndPassword(string username, string password)
        {
            string query = "SELECT [Username],[Password],[IsAdmin] FROM [dbo].[Users] WHERE Username = @username AND password = @password";
            var parameters = new[]
            {
                new SqlParameter("@username",  username ),
                new SqlParameter("@password", password )
            };
            var table = await _db.ExecuteDataTableAsync(query, parameters);

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new User
            {
                Username = row["Username"].ToString(),
                Password = row["Password"].ToString(),
                IsAdmin = Convert.ToBoolean(row["IsAdmin"])
            };

        }
        #endregion
    }
}
