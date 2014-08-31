using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Cors;
using Owin;

namespace HubConnectionManager.SignalR.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var configuration = new HubConfiguration() {EnableDetailedErrors = true, EnableJSONP = true};
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR(configuration);
        }
    }
}
