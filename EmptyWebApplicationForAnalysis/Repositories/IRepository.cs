using EmployeesApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmptyWebApplicationForAnalysis
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
