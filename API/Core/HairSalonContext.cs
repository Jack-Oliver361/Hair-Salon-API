
namespace API.Core
{
    using Microsoft.AspNet.Identity.EntityFramework;
    using Models;
    using System.Data.Entity;

    public class HairSalonContext : IdentityDbContext
    {


        public HairSalonContext()
            : base("HairSalonDatabase")
        {


        }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ServiceProvided> ServiceProvided { get; set; }
    }
}