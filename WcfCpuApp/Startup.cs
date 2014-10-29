using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(WcfCpuApp.Startup))]

namespace WcfCpuApp
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}