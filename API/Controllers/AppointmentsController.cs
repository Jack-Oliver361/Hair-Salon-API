using API.Core;
using API.Models;
using API.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Http;

namespace API.Controllers
{
    [RoutePrefix("api/appointments")]
    public class AppointmentsController : ApiController
    {


        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get()
        {
            using (var context = new HairSalonContext())
            {
                List<Appointment> Appointments = await context.Appointments.ToListAsync();
                List<AppointmentViewModel> appointmentView = new List<AppointmentViewModel>();
                foreach (Appointment appointment in Appointments)
                {
                    appointmentView.Add(new AppointmentViewModel (appointment));
                }
                

               
                return Ok(appointmentView);
            }
        }
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> Get(int id)
        {
            using (var context = new HairSalonContext())
            {
                Appointment Appointment = await context.Appointments.FirstOrDefaultAsync(x => x.AppointmentID == id);
                AppointmentViewModel appointmentView = new AppointmentViewModel();
                appointmentView = new AppointmentViewModel(Appointment);

                return Ok(appointmentView);
            }
        }

        [Authorize(Roles = "User")]
        [HttpGet]
        [Route("availableTimes")]
        public async Task<IHttpActionResult> GetTimes(string fullName)
        {
            string[] name = fullName.Split();

            List<string> availabletimes = new List<string>();
            List<string> bookedTimes = new List<string>();

            DateTime startHour = new DateTime(0);
            startHour = startHour.Add(new TimeSpan(9, 00, 0));

            while (!startHour.ToString("hh:mm tt").Equals("05:00 PM"))
            {

                availabletimes.Add(startHour.ToString("hh:mm tt"));
                startHour = startHour.Add(new TimeSpan(0, 15, 0));

            }

            using (var context = new HairSalonContext())
            {
                string first = name[0];
                string last = name[1];
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == first && e.LastName == last);
                if (employee == null)
                {
                    return NotFound();
                }


                

                

                List<ServiceProvided> servicedProvided = await context.ServiceProvided.Include(s => s.Service).Where(e => e.EmployeeID == employee.EmployeeID).ToListAsync();
                foreach (ServiceProvided sp in servicedProvided)
                {
                    string time = sp.Appointment.Time;
                    string duration = sp.Service.Duration;

                    
                        string[] numbers = Regex.Split(duration, @"\D+");
                        int dur = int.Parse(numbers[0]);
                        int id = availabletimes.IndexOf("0" + time);
                        for( int i = 0; i < dur / 15; i++)
                        {
                            availabletimes.RemoveAt(id);
                        }
                    }

                
                
                return Ok(availabletimes);
            }
        }



        [HttpPost]
        public async Task<IHttpActionResult> Post([FromBody] AppointmentViewModel appointment)
        {
            using (var context = new HairSalonContext())
            {
                var customer = await context.Customers.FirstOrDefaultAsync(b => b.CustomerID == appointment.CustomerID);
                if (customer == null)
                {
                    return NotFound();
                }

                var newAppointment = context.Appointments.Add(new Appointment
                {
                    CustomerID = customer.CustomerID,
                    Date = appointment.Date,
                    Time = appointment.Time
                });

                await context.SaveChangesAsync();
                return Ok(new AppointmentViewModel(newAppointment));
            }
        }


        [HttpDelete]
        public async Task<IHttpActionResult> Delete(int id)
        {
            using (var context = new HairSalonContext())
            {
                var appointment = await context.Appointments.FirstOrDefaultAsync(r => r.AppointmentID == id);
                if (appointment == null)
                {
                    return NotFound();
                }
                
                context.Appointments.Remove(appointment);
                await context.SaveChangesAsync();
            }
            return Ok();
      
    }
}
}
