using System.ComponentModel.DataAnnotations;

namespace API.Models
{

    public class Service
    {
        [Key]
        public int ServiceID { get; set; }
        [Display(Name = "Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service name is required")]
        public string Name { get; set; }
        [Display(Name = "Duration")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service duration is required")]
        public string Duration { get; set; }
        [Display(Name = "Price")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Service price is required")]
        public string Price { get; set; }


    
    }
}