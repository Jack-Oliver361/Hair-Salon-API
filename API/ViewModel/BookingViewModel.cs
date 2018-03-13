using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.ViewModel
{
    public class BookingViewModel
    {
        public string selectedService { get; set; }
        public string selectedDate { get; set; }
        public string selectedProvider { get; set; }
        public string selectedHour { get; set; }
        public string Email { get; set; }

    }
}