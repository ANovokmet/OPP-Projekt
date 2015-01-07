using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
[assembly: OwinStartup(typeof(UpravljanjeCekanjem.Startup))]
namespace UpravljanjeCekanjem
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/HomeUser/LogIn")
                
            });
            app.MapSignalR();
            Global.refresh_dataset();
        }
    }
}
