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
using Microsoft.AspNet.Identity;

namespace API.Controllers
{

    public class CustomersController : ApiController
    {
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                
                List<Customer> customers = await context.Customers.ToListAsync();
                List<CustomerViewModel> customerView = new List<CustomerViewModel>();
                foreach (Customer customer in customers)
                {
                   customerView.Add(new CustomerViewModel(customer));
                }



                return Ok(customerView);
            }
        }

        [Authorize(Roles = "User, Administrator")]
        [HttpPut]
        public async Task<IHttpActionResult> update(Customer customer)
        {
            if ((User.IsInRole("User") && customer.Email == User.Identity.GetUserId()) || User.IsInRole("Administrator"))
            {
                using (var context = new HairSalonContext())
                {

                    if (!ModelState.IsValid)
                    {
                        CustomerViewModel c = new CustomerViewModel(customer);
                        return BadRequest(ModelState);
                    }
                    else
                    {
                        var entity = await context.Customers.FirstOrDefaultAsync(c => c.CustomerID == customer.CustomerID);
                        entity.CustomerID = customer.CustomerID;
                        entity.FirstName = customer.FirstName;
                        entity.LastName = customer.LastName;
                        entity.Email = customer.Email;
                        entity.Password = customer.Password;
                        entity.ConfirmPassword = customer.ConfirmPassword;
                        entity.Phone = customer.Phone;
                        entity.DOB = customer.DOB;
                        entity.Gender = customer.Gender;

                        var store = new UserStore<IdentityUser>(new HairSalonContext());
                        var manager = new UserManager<IdentityUser>(store);
                        var currentUser = manager.FindByEmail(customer.Email);
                        currentUser.PasswordHash = new CustomerUserManager().PasswordHasher.HashPassword(customer.Password);
                        await manager.UpdateAsync(currentUser);
                        var userContext = store.Context;
                        userContext.SaveChanges();
                        context.SaveChanges();

                      



                        return Ok();
                    }
                }
            }else
            {
                return Unauthorized();
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            using (var context = new HairSalonContext())
            {
                return Ok(await context.Customers.FirstOrDefaultAsync(c => c.CustomerID == id));
            }
        }

        [Authorize(Roles = "User, Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> GetCustomer(string email)
        {
            if ((User.IsInRole("User") && email == User.Identity.GetUserId()) || User.IsInRole("Administrator")) {
                    using (var context = new HairSalonContext())
                    {
                        Customer customer = await context.Customers.FirstOrDefaultAsync(c => c.Email == email);
                        return Ok(customer);
                    }
                }else{
                return Unauthorized();
                }
        }


        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                CustomerViewModel c = new CustomerViewModel(customer);
                return BadRequest(ModelState);
            }
            else
            {
                using (var context = new HairSalonContext())
                {

                    if (await context.Customers.AnyAsync(c => c.Email == customer.Email))
                    {
                        ModelState.AddModelError("customer.Email", "Email is already registered");
                        return BadRequest(ModelState);
                    }

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
                    return Ok(newCustomer);

                }
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using (var context = new HairSalonContext())
            {
                var customer = await context.Customers.FirstOrDefaultAsync(r => r.CustomerID == id);
                List<Appointment> Appointments = await context.Appointments.Where(a => a.CustomerID == id).ToListAsync();
                if (customer == null)
                {
                    return NotFound();
                }
                var store = new UserStore<IdentityUser>(new HairSalonContext());
                var manager = new UserManager<IdentityUser>(store);
                var currentUser = manager.FindByEmail(customer.Email);
                manager.Delete(currentUser);
                var userContext = store.Context;
                userContext.SaveChanges();
                context.Customers.Remove(customer);
                foreach (var a in Appointments)
                {
                    context.Appointments.Remove(a);
                }
                await context.SaveChangesAsync();
            }
            return Ok();

        }
    }
}
