namespace API.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }
        [Display(Name = "First Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "First name is required")]
        public string FirstName { get; set; }
        [Display(Name = "Last Name")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Last name is required")]
        public string LastName { get; set; }

    }
}