using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Core
{
    public class CustomerUserStore : UserStore<IdentityUser>
    {
        public CustomerUserStore() : base(new HairSalonContext())
        {

        }
    }
}