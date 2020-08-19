using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EmployeesApplication.Models
{
    /// <summary>
    /// View Model Class, for interactions between Controller and View
    /// </summary>
    public class EmployeesVM
    {
        public bool Succeeded { get; set; } = false;

        public int Added { get; set; } = 0;
        public bool Created { get; set; }
        public bool Edited { get; set; } = false;
        public bool Deleted { get; set; } = false;

    }
}