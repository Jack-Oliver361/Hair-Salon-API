using API.Core;
using System.Threading.Tasks;
using System.Web.Http;
using System.Data.Entity;
using System.Collections.Generic;
using API.Models;
using API.ViewModel;
using System.Net;
using System.Net.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace API.Controllers
{
    [RoutePrefix("api/customers")]
    public class CustomersController : ApiController
    {
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                List<Customer> customers = await context.Customers.Include(a => a.Appointments).ToListAsync();
                List<CustomerViewModel> customerView = new List<CustomerViewModel>();
                foreach (Customer customer in customers)
                {
                   customerView.Add(new CustomerViewModel(customer));
                }



                return Ok(customerView);
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            using (var context = new HairSalonContext())
            {
                return Ok(await context.Customers.Include(x => x.Appointments).FirstOrDefaultAsync(c => c.CustomerID == id));
            }
        }
        // POST api/Account/Register
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(CustomerViewModel customer)
        {
            using (var context = new HairSalonContext())
            {

                if (await context.Customers.AnyAsync(c => c.Email == customer.Email))
                    return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Email address already registered"));

                var newCustomer = context.Customers.Add(new Customer
                {
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Password = customer.Password,
                    ConfirmPassword = customer.ConfirmPassword,
                    DOB = customer.DOB,
                    Phone = customer.Phone,
                    Gender = customer.Gender

                });

                string userRoleId = context.Roles.First(c => c.Name == "User").Id;
                var newUser = context.Users.Add(new IdentityUser(customer.Email) { Email = customer.Email, EmailConfirmed = true });
                newUser.Roles.Add(new IdentityUserRole { RoleId = userRoleId });

            

                var store = new CustomerUserStore();
                await store.SetPasswordHashAsync(newUser, new CustomerUserManager().PasswordHasher.HashPassword(customer.Password));
            
             
                await context.SaveChangesAsync();
                return Ok();
                
            }
        }
    }
}
