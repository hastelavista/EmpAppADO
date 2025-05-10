using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DATA.Models
{
    public class Experience
    {
        public int ExperienceID { get; set; }
        public int EmployeeID { get; set; }
        public string Company { get; set; }
        public string Department { get; set; }
        public int? Years { get; set; }

    }
}
