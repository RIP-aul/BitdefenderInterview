using AvMock;
using AvMock.Interfaces;
using AvMock.Services;
using BitdefenderInterview.Controllers.Commons;
using Newtonsoft.Json;

namespace BitdefenderInterview
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
                options.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            SetDependencyInjection(builder);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }

        private static void SetDependencyInjection(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAntivirus, Antivirus>();
            builder.Services.AddSingleton<IAntivirusService, AntivirusService>();
            builder.Services.AddSingleton<IOnDemandScanService, OnDemandScanService>();
            builder.Services.AddSingleton<IAntivirusEventHandler, AntivirusEventHandler>();
        }
    }
}