using System.Diagnostics;

using Amazon;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using Amazon.Runtime;

namespace AwsPauseExponentially
{
    public class Widget
    {
    }

    public class WidgetMiddleware
    {
        public WidgetMiddleware(
            RequestDelegate next,
            IWebHostEnvironment env,
            IEnumerable<Widget> widget)
        {
        }

        public async Task Invoke(HttpContext context)
        {
        }
    }

    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            Env = env;
        }

        private IConfiguration Configuration { get; }
        private IWebHostEnvironment Env { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            
            services.AddControllersWithViews();
            
            services.AddAWSService<IAmazonS3>(new AWSOptions 
            {
                Credentials = new AnonymousAWSCredentials(),
                Region = RegionEndpoint.USEast1
            });

            services.AddSingleton<Widget>((container) =>
            {
                var sw = Stopwatch.StartNew();
                var s3 = container.GetRequiredService<IAmazonS3>();
                sw.Stop();
                throw new ApplicationException("AWSSDK took " + sw.ElapsedMilliseconds.ToString() + "ms for existential reflection.");
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();

            app.UseMiddleware<WidgetMiddleware>();
            
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
