using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Thing1.Startup))]
namespace Thing1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
