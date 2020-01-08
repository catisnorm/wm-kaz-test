using System;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WmKazTest.Core.Services;
using WmKazTest.Data;
using WmKazTest.Data.Interfaces;

namespace WmKazTest
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
            services.AddDbContext<ObservationDataContext>(builder =>
                builder.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ObservationDb")));
            services.AddControllers();

            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(asm => asm.FullName.StartsWith("WmKazTest", StringComparison.OrdinalIgnoreCase));
            services.AddAutoMapper(assemblies);

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<SequenceService>();
            services.AddScoped<ObservationService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IMapper autoMapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            autoMapper.ConfigurationProvider.AssertConfigurationIsValid();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}