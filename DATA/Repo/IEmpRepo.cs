
using System.Data;
using DATA.Models;


namespace DATA.Repo
{
    public interface IEmpRepo
    {
        Task<Employee> GetEmployeeByIdAsync(int id);
        Task<List<Experience>> GetExperiencesByEmployeeIdAsync(int id);
        Task<DataTable> GeEmployeeList();
        Task<int> InsertNewEmpExp(Employee employee, List<Experience> experiences);
        //update on same exp id
        Task UpdateEmployeeWithExperiences(Employee employee, List<Experience> experiences);
        Task DeleteEmpExpByID(int id);
    }
}
