namespace API.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ServiceProvided
    {
        [Key, Column(Order = 1), ForeignKey ("Appointment")]
        public int AppointmentID { get; set; }
        [Key, Column(Order = 2)]
        public int NumberOfService { get; set; }
        [ForeignKey ("Service")]
        public int ServiceID { get; set; }
        [ForeignKey ("Employee")]
        public int EmployeeID { get; set; }

        public virtual Appointment Appointment{ get; set; }
        public virtual Service Service { get; set; }
        public virtual Employee Employee { get; set; }
        
    }
}