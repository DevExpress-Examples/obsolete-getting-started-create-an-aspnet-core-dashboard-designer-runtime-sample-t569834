using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using DevExpress.AspNetCore;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreDashboardWebApplication {
    public class Startup { 
        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnvironment) {
            Configuration = configuration;
            FileProvider = hostingEnvironment.ContentRootFileProvider;
        }

        public IConfiguration Configuration { get; }
        public IFileProvider FileProvider { get; }

        public void ConfigureServices(IServiceCollection services) {
            // Add a DashboardController class descendant with a specified dashboard storage
            // and a connection string provider.
            services
                .AddMvc()
                .AddDefaultDashboardController(configurator => {
                    configurator.SetDashboardStorage(new DashboardFileStorage(FileProvider.GetFileInfo("App_Data/Dashboards").PhysicalPath));
                    configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(Configuration));
                });
            // Add the third-party (JQuery, Knockout, etc.) and DevExtreme libraries.
            services.AddDevExpressControls(settings => settings.Resources = ResourcesType.ThirdParty | ResourcesType.DevExtreme);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env) {
            if (env.IsDevelopment()) {
                app.UseDeveloperExceptionPage();
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
