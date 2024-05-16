using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IBonusRepository
    {
        List<Employee> GetEmployeeWithCompany(int id);

        Company GetCompanyWithEmployees(int id);

        List<Company> GetAllCompanyWithEmployees();

        void AddTestCompanyWithEmployees(Company objCompany);

        void AddTestCompanyWithEmployeesWithTransaction(Company objCompany);

        void RemoveRange(int[] companyId);

        List<Company> FilterCompanyByName(string name);
    }
}
