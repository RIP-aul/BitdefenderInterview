using AvMock;
using AvMock.Interfaces;
using AvMock.Services;
using BitdefenderInterview.Commons;
using BitdefenderInterview.Commons.Interfaces;
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

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            SetDependencyInjection(builder);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();
            app.UseHttpsRedirection();
            app.MapControllers();
            app.Run();
        }

        private static void SetDependencyInjection(WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton<IAntivirus, Antivirus>();
            builder.Services.AddSingleton<IAntivirusService, AntivirusService>();
            builder.Services.AddSingleton<IOnDemandScanService, OnDemandScanService>();
            builder.Services.AddSingleton<IRealTimeScanService, RealTimeScanService>();
            builder.Services.AddSingleton<IAntivirusEventHandler, AntivirusEventHandler>();
        }
    }
}