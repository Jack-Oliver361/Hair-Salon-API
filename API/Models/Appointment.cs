

namespace API.Models
{
    public class Appointment
    {
        public int AppointmentID { get; set;}
        public string Date { get; set;}
        public string Time { get; set; }
        public int CustomerID { get; set; }

    }
}