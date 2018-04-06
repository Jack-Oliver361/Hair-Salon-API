using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.ViewModel
{
    public class CustomerViewModel
    {

        public CustomerViewModel()
        {
        }

        public CustomerViewModel(Customer customer)
        {
            if (customer == null)
            {
                return;
            }

            CustomerID = customer.CustomerID;
            FirstName = customer.FirstName;
            LastName = customer.LastName;
            Email = customer.Email;
            Password = customer.Password;
            ConfirmPassword = customer.ConfirmPassword;
            Phone = customer.Phone;
            DOB = customer.DOB;
            Gender = customer.Gender;
           


        }

        public int CustomerID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get;set; }
        public string Phone { get; set; }
        public string DOB { get; set; }
        public string Gender { get; set; }
      
        
    }
}