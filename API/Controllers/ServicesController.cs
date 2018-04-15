using API.Core;
using API.Models;
using API.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    public class ServicesController : ApiController
    {
        [Authorize(Roles = "Administrator, User")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                List<Service> Services = await context.Services.ToListAsync();
                List<ServiceViewModel> serviceView = new List<ServiceViewModel>();
                foreach (Service service in Services)
                {
                   serviceView.Add(new ServiceViewModel(service));
                }



                return Ok(serviceView);
            }
        }
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IHttpActionResult> newService(Service service)
        {
            if (!ModelState.IsValid)
            {
                ServiceViewModel s = new ServiceViewModel(service);
                return BadRequest(ModelState);
            }
            else
            {
                using (var context = new HairSalonContext())
                {
                    var newService = context.Services.Add(new Service
                    {
                        Name = service.Name,
                        Duration = service.Duration,
                        Price = service.Price

                    });
                    await context.SaveChangesAsync();
                    return Ok(newService);
                }
            }

        }

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<IHttpActionResult> update(Service service)
        {
            if (!ModelState.IsValid)
            {
                ServiceViewModel s = new ServiceViewModel(service);
                return BadRequest(ModelState);
            }
            else
            {
                using (var context = new HairSalonContext())
                {
                    var entity = await context.Services.FirstOrDefaultAsync(s => s.ServiceID == service.ServiceID);
                    entity.ServiceID = service.ServiceID;
                    entity.Name = service.Name;
                    entity.Duration = service.Duration;
                    entity.Price = service.Price;
                    context.SaveChanges();
                    return Ok();
                }
            }

        }
    }
}
