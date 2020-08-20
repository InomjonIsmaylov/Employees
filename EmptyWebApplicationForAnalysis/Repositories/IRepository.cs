using EmployeesApplication.Models;
using System.Collections.Generic;

namespace EmployeesApplication
{
    public interface IRepository
    {
        List<Employees> GetEmployeeList();
        void AddEmployee(Employees employee);
        void SaveChanges();
        Employees FindById(int? id);
        void Update(Employees employee);
        void Delete(Employees employees);
        void Delete(int? id);
        void Dispose();
    }
}
