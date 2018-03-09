using API.Core;
using API.Models;
using API.ViewModel;
using System.Collections.Generic;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    public class EmployeesController : ApiController
    {
        [Authorize(Roles = "User")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                List<Employee> employees = await context.Employees.ToListAsync();
                List<EmployeeViewModel> employeeView = new List<EmployeeViewModel>();
                foreach (Employee employee in employees)
                {
                    employeeView.Add(new EmployeeViewModel(employee));
                }



                return Ok(employeeView);
            }
        }

        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using (var context = new HairSalonContext())
            {
                var employee = await context.Employees.FirstOrDefaultAsync(r => r.EmployeeID == id);
                if (employee == null)
                {
                    return NotFound();
                }

                context.Employees.Remove(employee);
                await context.SaveChangesAsync();
            }
            return Ok();

        }
    }
}