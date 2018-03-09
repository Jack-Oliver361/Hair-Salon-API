using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.ViewModel
{
    public class ServiceProvidedViewModel
    {
        public ServiceProvidedViewModel()
        {
        }

        public ServiceProvidedViewModel(ServiceProvided serviceprovided)
        {
            if (serviceprovided == null)
            {
                return;
            }


            AppointmentID = serviceprovided.AppointmentID;
            NumberOfService = serviceprovided.NumberOfService;
            ServiceID = serviceprovided.ServiceID;
            EmployeeID = serviceprovided.EmployeeID;
            Appointment = new AppointmentViewModel(serviceprovided.Appointment);
            Service = new ServiceViewModel(serviceprovided.Service);
            Employee = new EmployeeViewModel(serviceprovided.Employee);

        }

        public int AppointmentID { get; set; }
        public int EmployeeID { get; set; }
        public int NumberOfService { get; set; }
        public int ServiceID { get; set; }
        public AppointmentViewModel Appointment { get; set; }
        public ServiceViewModel Service { get; set; }
        public EmployeeViewModel Employee { get; set;}
    }
}