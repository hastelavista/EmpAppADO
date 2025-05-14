using DATA.Models;

namespace EmpAppADO.UIModel
{
    public class EmpExpForm
    {
        public Employee Employee { get; set; } = new Employee();
        public List<Experience> Experiences { get; set; } = new List<Experience>();

    }
}
