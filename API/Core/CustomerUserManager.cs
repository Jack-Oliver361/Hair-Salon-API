using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace API.Core
{
    public class CustomerUserManager : UserManager<IdentityUser>
    {
        public CustomerUserManager() : base(new CustomerUserStore())
        {

        }
    }
}