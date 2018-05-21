using API.Core;
using API.Models;
using API.ViewModel;
using Revalee.Client;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;

namespace API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
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

        [Authorize(Roles = "User, Administrator")]
        [HttpGet]
        public async Task<IHttpActionResult> GetTimes(string fullName, string date, string Service)
        {
            string[] name = fullName.Split();

            List<string> availabletimes = new List<string>();
            List<string> bookedTimes = new List<string>();

            DateTime startHour = new DateTime(0);
            startHour = startHour.Add(new TimeSpan(9, 00, 0));
            while (!startHour.ToString("hh:mm tt").Equals("01:00 PM"))
            {

                availabletimes.Add(startHour.ToString("hh:mm tt"));
                startHour = startHour.Add(new TimeSpan(0, 15, 0));

            }
            while (!startHour.ToString("%h:mm tt").Equals("5:15 PM"))
            {

                availabletimes.Add(startHour.ToString("h:mm tt"));
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
                        id = availabletimes.IndexOf(time);
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
                        if (id - i > -1)
                            availabletimes.RemoveAt(id - i);
                    }

                }



            }
            return Ok(availabletimes);
        }


        [HttpPost]
        [Authorize(Roles = "User, Administrator")]
        public async Task<IHttpActionResult> createBooking(BookingViewModel booking)
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

        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> SMSNotifi()
        {

            TimeSpan now = DateTime.Now.TimeOfDay;
            using (var context = new HairSalonContext())
            {
                List<Appointment> Appointments = await context.Appointments.Include(c => c.customer).ToListAsync();
                List<AppointmentViewModel> appointmentView = new List<AppointmentViewModel>();
                foreach (Appointment appointment in Appointments)
                {
                    DateTime localDate = DateTime.Now.Date.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
                    DateTime roundedDown = new DateTime(localDate.Year, localDate.Month, localDate.Day, localDate.Hour, localDate.Minute, 0).AddMinutes(-localDate.Minute % 15);
                    DateTime appointmentTime = DateTime.ParseExact(appointment.Date + " " + appointment.Time, "dd/MM/yyyy h:mm tt", CultureInfo.InvariantCulture);
                    if (roundedDown == appointmentTime.AddHours(-24))
                    {
                        String message = HttpUtility.UrlEncode("This is a reminder that a you have booked a hair appointment on " + appointment.Date + " at " + appointment.Time + ". We hope to see you there. If you would like to cancel please phone the hair salon at quote " + appointment.AppointmentID + ". Thank you");
                        string phoneNum = appointment.customer.Phone;
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
                return NotFound();

            }
        }
    }
}
