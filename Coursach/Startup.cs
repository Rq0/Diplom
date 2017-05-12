using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Coursach.Startup))]
namespace Coursach
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
