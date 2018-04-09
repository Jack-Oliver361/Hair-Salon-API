using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.ViewModel
{
    public class ServiceViewModel
    {
        public ServiceViewModel()
        {
        }

        public ServiceViewModel(Service service)
        {
            if (service == null)
            {
                return;
            }

            ServiceID = service.ServiceID;
            Name = service.Name;
            Duration = service.Duration;
            Price = service.Price;

        }

        public int ServiceID { get; set; }
        public string Name { get; set; }
        public string Duration { get; set; }
        public string Price { get; set; }
        
    }
}