using API.Core;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Collections.Generic;
using API.Models;
using API.ViewModel;

namespace API.Controllers
{
    public class ServicesProvidedController : ApiController
    {
    
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
