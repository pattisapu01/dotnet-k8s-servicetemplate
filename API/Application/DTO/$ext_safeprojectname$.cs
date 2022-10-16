using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cloud.$ext_safeprojectname$.Infrastructure.Base;

namespace Cloud.$ext_safeprojectname$.API.Application.DTO
{
    public class $ext_safeprojectname$ : BaseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }
        public double Amount { get; set; }
        public bool IsActive { get; set; }        
    }
}
