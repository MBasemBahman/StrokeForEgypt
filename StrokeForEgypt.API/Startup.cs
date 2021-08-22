using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using StrokeForEgypt.API.Authorization;
using StrokeForEgypt.API.Helpers;
using StrokeForEgypt.API.Services;
using StrokeForEgypt.Common;
using StrokeForEgypt.DAL;
using StrokeForEgypt.Repository;
using System;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace StrokeForEgypt.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews()
                    .AddJsonOptions(options => options.JsonSerializerOptions.PropertyNamingPolicy = null);

            services.AddDbContext<BaseDBContext>(options => options.UseSqlServer(Configuration.GetConnectionString("dbConnection")));

            services.AddScoped<UnitOfWork>();

            services.AddAutoMapper(c => c.AddProfile<Mapping>(), typeof(Startup));

            AppMainData.DomainName = Configuration.GetValue<string>("DomainName");

            // configure strongly typed settings object
            services.Configure<AppSettings>(Configuration.GetSection("AppSettings"));

            // configure DI for application services
            services.AddScoped<IJwtUtils, JwtUtils>();
            services.AddScoped<IAccountService, AccountService>();

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("Account", new OpenApiInfo { Title = "Account", Version = "v1" });
                c.SwaggerDoc("MainData", new OpenApiInfo { Title = "Main Data", Version = "v1" });
                c.SwaggerDoc("Sponsor", new OpenApiInfo { Title = "Sponsor", Version = "v1" });
                c.SwaggerDoc("News", new OpenApiInfo { Title = "News", Version = "v1" });
                c.SwaggerDoc("Notification", new OpenApiInfo { Title = "Notification", Version = "v1" });
                c.SwaggerDoc("Event", new OpenApiInfo { Title = "Event", Version = "v1" });
                c.SwaggerDoc("Booking", new OpenApiInfo { Title = "Booking", Version = "v1" });

                string xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                string xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            #endregion

            #region Cors

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.SetIsOriginAllowed(origin => true)
                           .AllowAnyMethod()
                           .WithExposedHeaders("X-Status", "X-Pagination")
                           .AllowAnyHeader()
                           .AllowCredentials();
                });
            });

            #endregion

            #region Versioning

            services.AddApiVersioning(config =>
            {
                // Specify the default API Version
                config.DefaultApiVersion = new ApiVersion(1, 0);
                // If the client hasn't specified the API version in the request, use the default API version number 
                config.AssumeDefaultVersionWhenUnspecified = true;
                // Advertise the API versions supported for the particular endpoint
                config.ReportApiVersions = true;
            });

            #endregion

            #region Localization

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                CultureInfo[] supportedCultures = new[]
                {
                    new CultureInfo("en"),
                    new CultureInfo("ar")
                };
                options.DefaultRequestCulture = new RequestCulture(culture: "en", uiCulture: "en");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            services.AddSingleton<EntityLocalizationService>();

            #endregion

            #region Notification

            // Notification Service
            JToken jAppSettings = JToken.Parse(
                                 File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "appsettings.json")));
            string googleCredential = jAppSettings["GoogleCredential"].ToString();
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromJson(googleCredential)
            //});

            #endregion
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/Account/swagger.json", "Account");
                c.SwaggerEndpoint("/swagger/MainData/swagger.json", "Main Data");
                c.SwaggerEndpoint("/swagger/Sponsor/swagger.json", "Sponsor");
                c.SwaggerEndpoint("/swagger/News/swagger.json", "News");
                c.SwaggerEndpoint("/swagger/Notification/swagger.json", "Notification");
                c.SwaggerEndpoint("/swagger/Event/swagger.json", "Event");
                c.SwaggerEndpoint("/swagger/Booking/swagger.json", "Booking");

                c.RoutePrefix = "docs";
            });

            AppMainData.WebRootPath = env.ContentRootPath;

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseFileServer();
            app.UseCors();
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);

            // global error handler
            app.UseMiddleware<ErrorHandlerMiddleware>();

            // custom jwt auth middleware
            app.UseMiddleware<JwtMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
