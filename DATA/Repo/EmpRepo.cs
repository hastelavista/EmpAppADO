
using System.Data;
using Microsoft.Data.SqlClient;
using DATA.Models;

namespace DATA.Repo
{
    public class EmpRepo : IEmpRepo
    {
        private readonly DbContext _db;
        public EmpRepo(DbContext db)
        {
            _db = db;
        }


        //for retrieval and display (read)
        public async Task<List<Experience>> GetExperiencesByEmployeeIdAsync(int id)
        {
            string query = "SELECT * FROM Experiences WHERE EmployeeID = @EmployeeID";
            var parameters = new[] { new SqlParameter("@EmployeeID", id) };
            var table = await _db.ExecuteDataTableAsync(query, parameters);
            return table.AsEnumerable().Select(row => new Experience
            {
                ExperienceID = Convert.ToInt32(row["ExperienceID"]),
                EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                Company = row["Company"].ToString(),
                Department = row["Department"].ToString(),
                Years = row["Years"] != DBNull.Value ? (int?)Convert.ToInt32(row["Years"]) : null,

            }).ToList();
        }
        public async Task<Employee> GetEmployeeByIdAsync(int id)
        {
            string query = "SELECT * FROM Employees WHERE EmployeeID = @EmployeeID";
            var parameters = new[] { new SqlParameter("@EmployeeID", id) };
            var table = await _db.ExecuteDataTableAsync(query, parameters);

            if (table.Rows.Count == 0) return null;

            var row = table.Rows[0];
            return new Employee
            {
                EmployeeID = Convert.ToInt32(row["EmployeeID"]),
                Name = row["Name"].ToString(),
                Age = Convert.ToInt32(row["Age"]),
                Gender = row["Gender"].ToString(),
                Contact = row["Contact"].ToString(),
                ImagePath = row["ImagePath"] != DBNull.Value ? row["ImagePath"].ToString() : null

            };
        }
        public async Task<DataTable> GeEmployeeList()
        {
            string query = @" SELECT  e.EmployeeID,  e.Age, e.Name, e.Gender, e.Contact, e.ImagePath,SUM(exp.Years) AS Years FROM [dbo].[Employees] AS e
                            INNER JOIN   dbo.Experiences AS exp   ON exp.EmployeeID = e.EmployeeID GROUP BY   e.EmployeeID, e.Age, e.Name, e.Gender, e.Contact, e.ImagePath";
            var table = await _db.ExecuteDataTableAsync(query);
            return table;

        }

        //for C,U,D
        public async Task<int> InsertNewEmpExp(Employee employee, List<Experience> experiences)
        {
            string empInsertQuery = @"INSERT INTO Employees (Name, Age, Gender, Contact, ImagePath)VALUES (@Name, @Age, @Gender, @Contact, @ImagePath);
                            SELECT SCOPE_IDENTITY();";

            SqlParameter[] empParams = new[]
            {
                new SqlParameter("@Name", employee.Name),
                new SqlParameter("@Age", employee.Age),
                new SqlParameter("@Gender", employee.Gender),
                new SqlParameter("@Contact", employee.Contact),
                new SqlParameter("@ImagePath", employee.ImagePath)
            };
            var result = await _db.ExecuteScalarAsync(empInsertQuery, empParams);
            int employeeId = Convert.ToInt32(result);

            foreach (var exp in experiences)
            {
                string expInsertQuery = @"INSERT INTO Experiences (EmployeeID, Company, Department, Years) 
                                            VALUES (@EmployeeID, @Company, @Department, @Years);";
                SqlParameter[] expParams = new[]
                {
                        new SqlParameter("@EmployeeID", employeeId),
                        new SqlParameter("@Company", exp.Company),
                        new SqlParameter("@Department", exp.Department),
                        new SqlParameter("@Years", exp.Years),
                };
                await _db.ExecuteNonQueryAsync(expInsertQuery, expParams);
            }

            return employeeId;
        }
        //upsert
        public async Task UpdateEmployeeWithExperiences(Employee employee, List<Experience> experiences)
        {
            //firstupdateemployee
            string updateEmployeeQuery = @"UPDATE Employees SET 
                                    Name = @Name, Age = @Age, Gender = @Gender, Contact = @Contact 
                                   WHERE EmployeeID = @EmployeeID";
            SqlParameter[] empParams =
            {
                        new SqlParameter("@EmployeeID", employee.EmployeeID),
                        new SqlParameter("@Name", employee.Name),
                        new SqlParameter("@Age", employee.Age),
                        new SqlParameter("@Gender", employee.Gender),
                        new SqlParameter("@Contact", employee.Contact),
                    };
            await _db.ExecuteNonQueryAsync(updateEmployeeQuery, empParams);

            //get all expids from  db of that emp
            string getExpIDsQuery = "SELECT ExperienceID FROM Experiences WHERE EmployeeID = @EmployeeID";

            var expTable = await _db.ExecuteDataTableAsync(getExpIDsQuery, new[]
            {
                new SqlParameter("@EmployeeID", employee.EmployeeID)
            });

            var existingIDs = expTable.AsEnumerable()
                .Select(row => Convert.ToInt32(row["ExperienceID"]))
                .ToList();

            var incomingIDs = experiences.Where(e => e.ExperienceID > 0).Select(e => e.ExperienceID).ToList();

            //delete removed exp
            var toDelete = existingIDs.Except(incomingIDs).ToList();
            foreach (var id in toDelete)
            {
                await _db.ExecuteNonQueryAsync("DELETE FROM Experiences WHERE ExperienceID = @ExperienceID", new[] {
                new SqlParameter("@ExperienceID", id)
            });
            }

            //insert or update
            foreach (var exp in experiences)
            {
                if (exp.ExperienceID > 0)
                {
                    // Update
                    string updateQuery = @"UPDATE Experiences SET Company=@Company, Department=@Department, Years=@Years
                                   WHERE ExperienceID=@ExperienceID";
                    SqlParameter[] updateParams = {
                new SqlParameter("@Company", exp.Company),
                new SqlParameter("@Department", exp.Department),
                new SqlParameter("@Years", exp.Years),
                new SqlParameter("@ExperienceID", exp.ExperienceID),
            };
                    await _db.ExecuteNonQueryAsync(updateQuery, updateParams);
                }
                else
                {
                    // Insert
                    string insertQuery = @"INSERT INTO Experiences (EmployeeID, Company, Department, Years)
                                   VALUES (@EmployeeID, @Company, @Department, @Years)";
                    SqlParameter[] insertParams = {
                new SqlParameter("@EmployeeID", employee.EmployeeID),
                new SqlParameter("@Company", exp.Company),
                new SqlParameter("@Department", exp.Department),
                new SqlParameter("@Years", exp.Years),
            };
                    await _db.ExecuteNonQueryAsync(insertQuery, insertParams);
                }
            }

        }
        public async Task DeleteEmpExpByID(int id)
        {
            SqlParameter[] expParams = new[]
            {
                new SqlParameter("@EmployeeID", id)
            };
            string delExpQuery = "DELETE FROM EXPERIENCES WHERE EmployeeID = @EmployeeID";
            await _db.ExecuteNonQueryAsync(delExpQuery, expParams);

            SqlParameter[] empParams = new[]
            {
                new SqlParameter("@EmployeeID", id)
            };
            string delEmpQuery = "DELETE FROM EMPLOYEES WHERE EmployeeID = @EmployeeID";
            await _db.ExecuteNonQueryAsync(delEmpQuery, empParams);
        }
    }
}
