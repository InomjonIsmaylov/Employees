using EmployeesApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace EmployeesApplication.Repositories
{
    public class Repository : IRepository, IDisposable
    {
        private EmployeesEntities _db;

        public Repository(EmployeesEntities db) => _db = db;

        public List<Employees> GetEmployeeList() => _db.Employees.ToList();
        
        public void AddEmployee(Employees employee) => _db.Employees.Add(employee);

        public Employees FindById(int? id) => _db.Employees.Find(id.Value);

        public void Update(Employees employee) => _db.Entry(employee).State = EntityState.Modified;

        public void Delete(Employees employees) => _db.Employees.Remove(employees);

        public void Delete(int? id) => Delete(FindById(id));

        public int SaveChanges() => _db.SaveChanges();

        public void Dispose() => Dispose(true);

        protected void Dispose(bool disposing)
        {
            if (disposing)
                if (_db != null)
                {
                    _db.Dispose();
                    _db = null;
                }
        }
    }
}