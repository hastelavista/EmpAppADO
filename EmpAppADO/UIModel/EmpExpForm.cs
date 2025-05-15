using DATA.Models;

namespace EmpAppADO.UIModel
{
    public class EmpExpForm
    {
        public Employee Employee { get; set; } = new Employee();
        public List<Experience> Experiences { get; set; } = new List<Experience>();

        //public string Employee { get; set; } = string.Empty;
        //public string Experiences { get; set; } = string.Empty;
        public IFormFile? ImageFile { get; set; }


    }
}
