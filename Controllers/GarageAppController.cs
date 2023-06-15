using GarageApp.DAL;
var builder = WebApplication.CreateBuilder(args);
var AllowOrigins = "AllowTheseOrigins";
builder.Services.AddCors(options =>
{
    options.AddPolicy(AllowOrigins, builder =>
    {
        // add the website that makes the requests and expects the responses here (comma delimites)
        builder.WithOrigins("http://localhost:4200")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IWork, Work>();

//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new() { Title = "TodoApi", Version = "v1" });
//});

var app = builder.Build();

if (builder.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();

    //app.UseSwagger();
    //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "TodoApi v1"));
}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

app.UseCors();

//app.UseAuthorization();

app.MapControllers();

app.Run();



/*using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace GarageApp
{
    public class GarageAppController
    {
        [EnableCors("AllowTheseOrigins")]
        [Route("[controller]")]
        // [ApiController]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            [HttpPost("/parkedcars/parkyourcar")]
            public string Hello()
            {
                string m = "Welcome to the PWC Data & Tech challenge.";
                return m;
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Run();
        }
    }
}*/