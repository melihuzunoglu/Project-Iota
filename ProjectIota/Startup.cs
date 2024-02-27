using ProjectIota.Data;
using Microsoft.EntityFrameworkCore;
using FastReport.Data;

namespace ProjectIota
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
            services.AddDbContext<DataContext>(x => x.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"))); //For PostgreSql
			services.AddControllersWithViews();

			FastReport.Utils.RegisteredObjects.AddConnection(typeof(PostgresDataConnection));

			services.AddControllers();
            
            services.AddFastReport();
            //FastReport.Utils.Config.WebMode = true;
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
