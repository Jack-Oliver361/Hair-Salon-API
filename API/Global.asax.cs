
namespace API
{
    using System.Data.Entity;
    using System.Web;
    using System.Web.Http;
    using Core;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using App_Start;

    public class WebApiApplication : HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);
            Database.SetInitializer(new Initializer());

            var formatters = GlobalConfiguration.Configuration.Formatters;
            var jsonFormatter = formatters.JsonFormatter;
            var settings = jsonFormatter.SerializerSettings;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            GlobalConfiguration.Configure(FilterConfig.Configure);
        }
    }
}
