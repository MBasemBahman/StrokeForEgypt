using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
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

            #region Swagger

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("MainData", new OpenApiInfo { Title = "Main Data", Version = "v1" });

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
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .WithExposedHeaders("X-Status", "X-Pagination")
                           .AllowAnyHeader();
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
                options.DefaultRequestCulture = new RequestCulture(culture: "ar", uiCulture: "ar");
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
                c.SwaggerEndpoint("/swagger/MainData/swagger.json", "Main Data API");

            });

            AppMainData.WebRootPath = env.ContentRootPath;

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStaticFiles();
            app.UseFileServer();
            app.UseCors();
            app.UseRequestLocalization(app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>().Value);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
