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
        [Authorize(Roles = "User")]
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
    }
}
