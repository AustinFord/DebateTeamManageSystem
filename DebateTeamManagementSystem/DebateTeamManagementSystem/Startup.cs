using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DebateTeamManagementSystem.Startup))]
namespace DebateTeamManagementSystem
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
