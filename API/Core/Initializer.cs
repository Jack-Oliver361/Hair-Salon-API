using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API.Core
{
    using System.Data.Entity;

    public class Initializer : MigrateDatabaseToLatestVersion<HairSalonContext, Configuration>
    {
    }
}