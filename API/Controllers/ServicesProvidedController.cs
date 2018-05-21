using API.Core;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Collections.Generic;
using API.Models;
using API.ViewModel;
using System;

namespace API.Controllers
{
    [RoutePrefix("api/servicesProvided")]
    public class ServicesProvidedController : ApiController
    {

        [Authorize(Roles = "Administrator")]
        [Route("dayAppointment/{date}")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(string date)
        {
            date = date.Replace("-", @"/");
            using (var context = new HairSalonContext())
            {
                List<ServiceProvided> servicesprovided = await context.ServiceProvided.Include(e => e.Employee).Include(s => s.Service).ToListAsync();
                List<ServiceProvided> appointments = new List<ServiceProvided>();
                List<ServiceProvidedViewModel> serviceprovidedView = new List<ServiceProvidedViewModel>();
                foreach (ServiceProvided serviceprovided in servicesprovided)
                {
                    if (serviceprovided.Appointment.Date == date)
                        appointments.Add(serviceprovided);

                }

                List<ServiceProvided> AM = new List<ServiceProvided>();
                List<ServiceProvided> noon = new List<ServiceProvided>();
                List<ServiceProvided> PM = new List<ServiceProvided>();

                foreach (ServiceProvided appointment in appointments)
                {
                    if (appointment.Appointment.Time.Contains("12"))
                    {
                        noon.Add(appointment);
                    }else if (appointment.Appointment.Time.Contains("AM"))
                    {
                        AM.Add(appointment);

                    }else if (appointment.Appointment.Time.Contains("PM") && !appointment.Appointment.Time.Contains("12"))
                    {
                        PM.Add(appointment);

                    }
                       

                }
                AM.Sort((x, y) => string.Compare(x.Appointment.Time, y.Appointment.Time));
                noon.Sort((x, y) => string.Compare(x.Appointment.Time, y.Appointment.Time));
                PM.Sort((x, y) => string.Compare(x.Appointment.Time, y.Appointment.Time));

                foreach (ServiceProvided serviceprovided in AM)
                {
                    serviceprovidedView.Add(new ServiceProvidedViewModel(serviceprovided));
                }
                foreach (ServiceProvided serviceprovided in noon)
                {
                    serviceprovidedView.Add(new ServiceProvidedViewModel(serviceprovided));
                }
                foreach (ServiceProvided serviceprovided in PM)
                {
                    serviceprovidedView.Add(new ServiceProvidedViewModel(serviceprovided));
                }

               return Ok(serviceprovidedView);
            }
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                List<ServiceProvided> servicesprovided = await context.ServiceProvided.Include(e => e.Employee).Include(s => s.Service).ToListAsync();
                List<ServiceProvidedViewModel> serviceprovidedView = new List<ServiceProvidedViewModel>();
                foreach (ServiceProvided serviceprovided in servicesprovided)
                {
                    serviceprovidedView.Add(new ServiceProvidedViewModel(serviceprovided));
                }



                return Ok(serviceprovidedView);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("getservice/{id}/{id2}")]
        public async Task<IHttpActionResult> Get(int id, int id2)
        {
        
      
            using (var context = new HairSalonContext())
            {
               ServiceProvided serviceprovided = await context.ServiceProvided.Include(e => e.Employee).Include(s => s.Service).FirstOrDefaultAsync(x => x.AppointmentID == id && x.NumberOfService == id2);
               ServiceProvidedViewModel serviceprovidedView = new ServiceProvidedViewModel();
               serviceprovidedView = new ServiceProvidedViewModel(serviceprovided);
               return Ok(serviceprovidedView);
            }
        }


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        [Route("getAllservice/{id}")]
        public async Task<IHttpActionResult> Get(int id)
        {
            using (var context = new HairSalonContext())
            {
                List<ServiceProvided> servicesprovided = await context.ServiceProvided.Include(e => e.Employee).Include(s => s.Service).ToListAsync();
                List<ServiceProvidedViewModel> serviceprovidedView = new List<ServiceProvidedViewModel>();
                foreach (ServiceProvided serviceprovided in servicesprovided)
                {
                    if (serviceprovided.AppointmentID == id)
                        serviceprovidedView.Add(new ServiceProvidedViewModel(serviceprovided));
                        
                    

                }
                return Ok(serviceprovidedView);
            }
        }


        

    }

}
