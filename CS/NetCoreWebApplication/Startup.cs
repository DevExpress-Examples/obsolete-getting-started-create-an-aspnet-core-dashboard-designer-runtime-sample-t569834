using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.AspNetCore;

namespace NetCoreWebApplication {
    public class Startup {
        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }
        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services) {
            // Add a DashboardController class descendant with a specified dashboard storage 
            // and a connection string provider. 
            services
                .AddMvc()
                .AddDefaultDashboardController(configurator => {
                    configurator.SetDashboardStorage(new DashboardFileStorage("App_Data\\Dashboards"));
                    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(Configuration));
                });
            // Add the third-party (JQuery, Knockout, etc.) and DevExtreme libraries. 
            services.AddDevExpressControls(settings => settings.Resources = ResourcesType.ThirdParty | ResourcesType.DevExtreme);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            } else {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            // Register the DevExpress middleware.
            app.UseDevExpressControls();
            app.UseMvc(routes => {
                // Map dashboard routes.
                routes.MapDashboardRoute();
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
