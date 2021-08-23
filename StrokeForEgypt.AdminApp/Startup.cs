using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Converters;
using StrokeForEgypt.AdminApp.Resources;
using StrokeForEgypt.AdminApp.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Repository;
using System;
using System.Globalization;
using System.Reflection;

namespace StrokeForEgypt.AdminApp
{
    public class Startup
    {
        private readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLocalization(options => options.ResourcesPath = "resources");
            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "ar");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddSingleton<CommonLocalizationService>();
            services.AddSingleton<EntityLocalizationService>();

            services.AddControllersWithViews()
                 .AddViewLocalization().AddDataAnnotationsLocalization(options =>
                 {
                     options.DataAnnotationLocalizerProvider = (type, factory) =>
                     {
                         AssemblyName assemblyName = new(typeof(EntityResources).GetTypeInfo().Assembly.FullName);
                         return factory.Create(nameof(EntityResources), assemblyName.Name);
                     };
                 })
                 .AddSessionStateTempDataProvider()
                 .AddRazorRuntimeCompilation()
                 .AddNewtonsoftJson(options =>
                 {
                     options.SerializerSettings.Converters.Add(new StringEnumConverter());
                 });

            services.AddControllersWithViews();

            services.AddSession(s => s.IdleTimeout = TimeSpan.FromMinutes(20));
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<BaseDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("dbConnection")));

            services.AddScoped<UnitOfWork>();

            services.AddAutoMapper(c => c.AddProfile<Mapping>(), typeof(Startup));

            AppMainData.DomainName = Configuration.GetValue<string>("DomainName");
            AppMainData.ApiServiceURL = Configuration.GetValue<string>("ApiServiceURL");

            services.AddCors(options =>
            {
                options.AddPolicy(MyAllowSpecificOrigins,
                                  builder =>
                                  {
                                      builder.AllowAnyOrigin();
                                      builder.AllowAnyMethod();
                                      builder.AllowAnyHeader();
                                  });
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
            app.UseSession();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(MyAllowSpecificOrigins);

            AppMainData.WebRootPath = env.WebRootPath;

            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Login}/{action=Index}/{id?}");
            });
        }
    }
}
