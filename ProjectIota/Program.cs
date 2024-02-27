using Microsoft.AspNetCore.Hosting;
using ProjectIota;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession();


//Newtonsoft
builder.Services.AddControllers().AddNewtonsoftJson();

//Redis
builder.Services.AddStackExchangeRedisCache(redisOptions =>
{
	redisOptions.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
}
);

//Syncfusion
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NAaF1cWWhIfEx1RHxQdld5ZFRHallYTnNWUj0eQnxTdEFjW39ZcHZQRWRUUUJxXQ==");

//FastReport
var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseFastReport();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseSession();
app.Run();
