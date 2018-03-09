using System.ComponentModel.DataAnnotations;

namespace API.Models
{

    public class Service
    {
        [Key]
        public int ServiceID { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }


    
    }
}