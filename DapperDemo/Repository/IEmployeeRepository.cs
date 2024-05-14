using DapperDemo.Models;

namespace DapperDemo.Repository
{
    public interface IEmployeeRepository
    {
        Employee Find(int id);

        List<Employee> GetAll();

        Employee Add(Employee employee);

        Employee Update(Employee employee);

        void Remove(int id);
    }
}
