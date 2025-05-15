using DATA.Models;

namespace EmpAppADO.Models
{
    public class EmpExpFormView
    {
        public Employee Employee { get; set; } = new Employee();
        public List<Experience> Experiences { get; set; } = new List<Experience>();

    }
}
