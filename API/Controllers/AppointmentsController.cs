using API.Core;
using API.Models;
using API.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
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
                List<Appointment> Appointments = await context.Appointments.Include(c => c.customer).ToListAsync();
                List<AppointmentViewModel> appointmentView = new List<AppointmentViewModel>();
                foreach (Appointment appointment in Appointments)
                {
                    appointmentView.Add(new AppointmentViewModel(appointment));
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
        public async Task<IHttpActionResult> GetTimes(string fullName, string date, string Service)
        {
            string[] name = fullName.Split();

            List<string> availabletimes = new List<string>();
            List<string> bookedTimes = new List<string>();

            DateTime startHour = new DateTime(0);
            startHour = startHour.Add(new TimeSpan(9, 00, 0));

            while (!startHour.ToString("hh:mm tt").Equals("05:15 PM"))
            {

                availabletimes.Add(startHour.ToString("hh:mm tt"));
                startHour = startHour.Add(new TimeSpan(0, 15, 0));

            }
            using (var context = new HairSalonContext())
            {
                string first = name[0];
                string last = name[1];
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == first && e.LastName == last);
                var service = await context.Services.FirstOrDefaultAsync(s => s.Name == Service);
                if (employee == null || service == null)
                {
                    return NotFound();
                }
                List<ServiceProvided> servicedProvided = await context.ServiceProvided.Where(e => e.EmployeeID == employee.EmployeeID && e.Appointment.Date.Equals(date)).ToListAsync();

                foreach (ServiceProvided sp in servicedProvided)
                {
                    string time = sp.Appointment.Time;
                    string duration = sp.Service.Duration;
                    string[] numbers = Regex.Split(duration, @"\D+");
                    int dur = int.Parse(numbers[0]);
                    int id;
                    if (time.Length == 7)
                    {
                        id = availabletimes.IndexOf("0" + time);
                    }
                    else
                    {
                        id = availabletimes.IndexOf(time);
                    }
                    for (int i = 1; i < dur / 15; i++)
                    {
                        availabletimes.RemoveAt(id);
                    }
                    string chosenService = service.Duration;
                    string[] chosenServiceSplit = Regex.Split(chosenService, @"\D+");
                    int chosenDur = int.Parse(chosenServiceSplit[0]);
                    for (int i = 0; i < chosenDur / 15; i++)
                    {
                        if(id - i > -1)
                        availabletimes.RemoveAt(id - i);
                    }

                }



            }
            return Ok(availabletimes);
        }




        [HttpPost]
        [Route("create")]
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


        [Authorize(Roles = "User")]
        [HttpPost]
        [Route("createBooking")]
        public async Task<IHttpActionResult> Postappoint(BookingViewModel booking)
        {
            using (var context = new HairSalonContext())
            {

                string[] name = booking.selectedProvider.Split();
                string firstName = name[0];
                string lastName = name[1];
                var employee = await context.Employees.FirstOrDefaultAsync(e => e.FirstName == firstName && e.LastName == lastName);
                var customer = await context.Customers.FirstOrDefaultAsync(c => c.Email == booking.Email);
                var service = await context.Services.FirstOrDefaultAsync(s => s.Name == booking.selectedService);
                if (employee == null || customer == null || service == null)
                {
                    return NotFound();
                }

                var newAppointment = context.Appointments.Add(new Appointment
                {
                    CustomerID = customer.CustomerID,
                    Date = booking.selectedDate,
                    Time = booking.selectedHour
                });

                var newServiceProvided = context.ServiceProvided.Add(new ServiceProvided
                {
                    AppointmentID = newAppointment.AppointmentID,
                    NumberOfService = 1,
                    ServiceID = service.ServiceID,
                    EmployeeID = employee.EmployeeID
                });

                await context.SaveChangesAsync();
                String message = HttpUtility.UrlEncode("Thank you for booking an appointment on " + newAppointment.Date + " on " + newAppointment.Time + ". If you have any enquries about the appointment please quote on appointment ID: " + newAppointment.AppointmentID + " When you phone {phone number}. We look forward to seeing you.");
                string phoneNum = customer.Phone;
                using (var wb = new WebClient())
                {
                    byte[] response = wb.UploadValues("https://api.txtlocal.com/send/", new NameValueCollection()
                {
                {"apikey" , "5H7HsKubrWI-GAF5j1aDRdOvoKM11K0GtF1eZQEHS3"},
                {"numbers" , phoneNum},
                {"message" , message},
                {"sender" , "Hair Salon"}
                });
                    string result = System.Text.Encoding.UTF8.GetString(response);
                    return Ok();

                }
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
