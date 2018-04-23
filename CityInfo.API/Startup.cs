using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CityInfo.API.Entities;
using CityInfo.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;

namespace CityInfo.API
{
    /// <summary>
    /// This is the basic class called after Program.
    /// To testing 
    /// </summary>
    public class Startup
    {
        public static IConfiguration Configuration { get; private set; }

        public Startup(IConfiguration configuration) {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc()
                .AddMvcOptions(o => o.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter())); //We can add new formatter for xml.
                                                                                                             //We can change the json namingstategy to be as it is in model. For example: standard format is first letter lover case, if we change it, first letter will be as it is in model class.
                                                                                                             //.AddJsonOptions(j => {
                                                                                                             //    if(j.SerializerSettings.ContractResolver != null) {
                                                                                                             //        var castedResolver = j.SerializerSettings.ContractResolver as DefaultContractResolver;
                                                                                                             //        castedResolver.NamingStrategy = null;
                                                                                                             //    }
                                                                                                             //});
            services.AddTransient<IMailService, LocalMailService>(); //dependency injection example with asp.net core mechanism insted of Ninject for example.
            var connectionString = Startup.Configuration["connectionStrings:cityInfoDBConnectionString"];
            services.AddDbContext<CityInfoContext>(o => o.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, CityInfoContext cityInfoContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            } else {
                app.UseExceptionHandler();
            }

            cityInfoContext.EnsureSeedDataForContext();

            app.UseStatusCodePages(); // I enabled it to see status code text sample pages in browser :)

            app.UseMvc();

        }
    }
}
