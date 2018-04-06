using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.ViewModel
{
    public class AppointmentViewModel
    {
        public AppointmentViewModel()
        {
        }

        public AppointmentViewModel(Appointment appointment)
        {
            if(appointment == null)
            {
                return;
            }

            AppointmentID = appointment.AppointmentID;
            CustomerID = appointment.CustomerID;
            Date = appointment.Date;
            Time = appointment.Time;
            cFirstName = appointment.customer.FirstName;
            cLastName = appointment.customer.LastName;
            

        }

        public int AppointmentID { get; set; }
        public int CustomerID { get; set; }
        public string Date { get; set; }
        public string Time { get; set; }
        public string cFirstName { get; set; }
        public string cLastName { get; set; }
        

    }
}