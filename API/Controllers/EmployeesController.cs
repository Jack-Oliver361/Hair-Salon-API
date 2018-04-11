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
        [Authorize(Roles = "Administrator")]
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

        [Authorize(Roles = "Administrator")]
        [HttpPut]
        public async Task<IHttpActionResult> update(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                EmployeeViewModel e = new EmployeeViewModel(employee);
                return BadRequest(ModelState);
            }
            else
            {
                using (var context = new HairSalonContext())
                {
                    var entity = await context.Employees.FirstOrDefaultAsync(e => e.EmployeeID == employee.EmployeeID);
                    entity.EmployeeID = employee.EmployeeID;
                    entity.FirstName = employee.FirstName;
                    entity.LastName = employee.LastName;
                    context.SaveChanges();
                    return Ok();
                }
            }

        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IHttpActionResult> newEmployee(Employee employee)
        {
            if (!ModelState.IsValid)
            {
                EmployeeViewModel e = new EmployeeViewModel(employee);
                return BadRequest(ModelState);
            }
            else
            {
                using (var context = new HairSalonContext())
                {
                    var newEmployee = context.Employees.Add(new Employee
                    {
                        FirstName = employee.FirstName,
                        LastName = employee.LastName

                    });
                    await context.SaveChangesAsync();
                    return Ok(newEmployee);
                }
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