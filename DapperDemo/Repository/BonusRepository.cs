using Dapper;
using DapperDemo.Models;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace DapperDemo.Repository
{
    public class BonusRepository : IBonusRepository
    {
        private IDbConnection db;

        public BonusRepository(IConfiguration configuration)
        {
            this.db = new SqlConnection(configuration.GetConnectionString("DefaultConnection"));
        }

        public void AddTestCompanyWithEmployees(Company objCompany)
        {
            var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);" +
                        "\r\nSELECT CAST(SCOPE_IDENTITY() as int);";

            var id = db.Query<int>(sql, objCompany).Single();

            objCompany.CompanyId = id;

            //foreach (var employee in objCompany.Employees)
            //{
            //    employee.CompanyId = id;
            var sql1 = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) " +
                    "VALUES(@Name, @Title, @Email, @Phone, @CompanyId);\r\n" +
                    "SELECT CAST(SCOPE_IDENTITY() as int);";

            //    db.Query<int>(sql1, employee).Single();                
            //}

            objCompany.Employees.Select(s => {
                s.CompanyId = id; return s;
            }).ToList();

            db.Execute(sql1, objCompany.Employees);

        }

        public void AddTestCompanyWithEmployeesWithTransaction(Company objCompany)
        {
            using (var transaction = new TransactionScope())
            {
                try
                {
                    var sql = "INSERT INTO Companies (Name, Address, City, State, PostalCode) VALUES(@Name, @Address, @City, @State, @PostalCode);" +
                        "\r\nSELECT CAST(SCOPE_IDENTITY() as int);";

                    var id = db.Query<int>(sql, objCompany).Single();

                    objCompany.CompanyId = id;

                    objCompany.Employees.Select(s => {
                        s.CompanyId = id; return s;
                    }).ToList();

                    var sqlEmp = "INSERT INTO Employees (Name, Title, Email, Phone, CompanyId) " +
                                    "VALUES(@Name, @Title, @Email, @Phone, @CompanyId);\r\n" +
                                    "SELECT CAST(SCOPE_IDENTITY() as int);";
                    db.Execute(sqlEmp, objCompany.Employees);
                    transaction.Complete();

                }
                catch (Exception)
                {

                    throw;
                }
            }            

        }

        public List<Company> GetAllCompanyWithEmployees()
        {
            var sql = "select c.*, e.* from Employees as e inner join Companies as c on e.CompanyId = c.CompanyId";

            var companyDic = new Dictionary<int, Company>();

            var company = db.Query<Company, Employee, Company>(sql, (c,e) => 
            {
                if (!companyDic.TryGetValue(c.CompanyId, out var currentCompany))
                {
                    currentCompany = c;
                    companyDic.Add(currentCompany.CompanyId, currentCompany);
                }
                currentCompany.Employees.Add(e);
                return currentCompany;
            }, splitOn: "EmployeeId");

            return company.Distinct().ToList();
        }

        public Company GetCompanyWithEmployees(int id)
        {
            var p = new
            {
                CompanyId = id 
            };

            var sql = "select *  from Companies where CompanyId = @CompanyId;" +
                "select *  from Employees where CompanyId = @CompanyId;";

            Company company;

            using (var lists = db.QueryMultiple(sql, p))
            {
                company = lists.Read<Company>().ToList().FirstOrDefault();
                company.Employees = lists.Read<Employee>().ToList();
            }
            return company;
        }

        public List<Employee> GetEmployeeWithCompany(int id)
        {
            var sql = "select e.*, c.* from Employees as e inner join Companies as c on e.CompanyId = c.CompanyId";

            if (id != 0) 
            {
                sql += " where e.CompanyId = @Id";
            }

            var employee = db.Query<Employee, Company, Employee>(sql, (e,c) => {
                e.Company = c;
                return e;
            }, new { id },splitOn: "CompanyId");

            return employee.ToList();
        }

        public void RemoveRange(int[] companyId)
        {
            db.Query("delete from Companies where CompanyId in @companyId", new {@companyId = companyId});
        }


        public List<Company> FilterCompanyByName(string name)
        {
            return db.Query<Company>("select * from Companies where Name like '%' + @name + '%' ", new {name} ).ToList();
        }
    }
}
