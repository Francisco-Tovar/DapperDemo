namespace DapperDemo.Models
{
    public class Company
    {        

        public int CompanyId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string State {  get; set; }
        public string PostalCode { get; set; }

        public List<Employee> Employees{ get;}

    }
}
