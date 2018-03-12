using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Core
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Validation;

    public class Configuration : DbMigrationsConfiguration<HairSalonContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
        protected override void Seed(HairSalonContext context)
        {
            context.Appointments.AddOrUpdate(new Appointment { CustomerID = 1, AppointmentID = 1, Date = "20/03/2018", Time = "9:00 AM" });
            context.Appointments.AddOrUpdate(new Appointment { CustomerID = 1, AppointmentID = 2, Date = "22/03/2018", Time = "12:30 PM" });
            context.Appointments.AddOrUpdate(new Appointment { CustomerID = 2, AppointmentID = 5, Date = "20/03/2018", Time = "12:30 PM" });

            context.Appointments.AddOrUpdate(new Appointment { CustomerID = 2, AppointmentID = 3, Date = "22/03/2018", Time = "2:00 PM" });
            context.Appointments.AddOrUpdate(new Appointment { CustomerID = 2, AppointmentID = 4, Date = "27/02/2018", Time = "11:30 AM" });


            context.Services.AddOrUpdate(new Service{ ServiceID = 1, Name = "Cut and blow dry", Duration = "30m", Price = "£44.99"});
            context.Services.AddOrUpdate(new Service { ServiceID = 2, Name = "Full head colour", Duration = "60m", Price = "£74.99" });

            context.Employees.AddOrUpdate(new Employee { EmployeeID = 1, FirstName = "Sian", LastName = "Tray"});
            context.Employees.AddOrUpdate(new Employee { EmployeeID = 2, FirstName = "Laura", LastName = "Walksa" });

            context.ServiceProvided.AddOrUpdate(new ServiceProvided { AppointmentID = 1, NumberOfService = 1, ServiceID = 1, EmployeeID = 1 });
            context.ServiceProvided.AddOrUpdate(new ServiceProvided { AppointmentID = 3, NumberOfService = 1, ServiceID = 2, EmployeeID = 1 });
            context.ServiceProvided.AddOrUpdate(new ServiceProvided { AppointmentID = 2, NumberOfService = 1, ServiceID = 2, EmployeeID = 2 });
            context.ServiceProvided.AddOrUpdate(new ServiceProvided { AppointmentID = 5, NumberOfService = 1, ServiceID = 2, EmployeeID = 2 });

            context.Customers.AddOrUpdate(new Customer
            {
                CustomerID = 1,
                FirstName = "Eva",
                LastName = "Jones",
                Email = "EvaJones123@gmail.com",
                Password = "qwerty123",
                ConfirmPassword = "qwerty123",
                Phone = "09876543211",
                DOB = "27/07/1997",
                Gender = "Female"
                
            });

            context.Customers.AddOrUpdate(new Customer
            {
                CustomerID = 2,
                FirstName = "Jake",
                LastName = "Shirley",
                Email = "JakeShirley345@gmail.com",
                Password = "fireInDaBooth",
                ConfirmPassword = "fireInDaBooth",
                Phone = "01234567890",
                DOB = "12/09/1995",
                Gender = "Male"
            });



            string adminRoleId;
            string userRoleId;
            if (!context.Roles.Any())
            {
                adminRoleId = context.Roles.Add(new IdentityRole("Administrator")).Id;
                userRoleId = context.Roles.Add(new IdentityRole("User")).Id;
            }
            else
            {
                adminRoleId = context.Roles.First(c => c.Name == "Administrator").Id;
                userRoleId = context.Roles.First(c => c.Name == "User").Id;
            }

            context.SaveChanges();

            if (!context.Users.Any())
            {
                var administrator = context.Users.Add(new IdentityUser("administrator") { Email = "admin@somesite.com", EmailConfirmed = true });
                administrator.Roles.Add(new IdentityUserRole { RoleId = adminRoleId });

                var standardUser = context.Users.Add(new IdentityUser("jonpreece") { Email = "jon@somesite.com", EmailConfirmed = true });
                standardUser.Roles.Add(new IdentityUserRole { RoleId = userRoleId });

                context.SaveChanges();

                var store = new CustomerUserStore();
                store.SetPasswordHashAsync(administrator, new CustomerUserManager().PasswordHasher.HashPassword("administrator123"));
                store.SetPasswordHashAsync(standardUser, new CustomerUserManager().PasswordHasher.HashPassword("user123"));
            }

            context.SaveChanges();
        }
    }
}