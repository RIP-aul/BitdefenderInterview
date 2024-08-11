using AvMock;
using AvMock.Interfaces;
using BitdefenderInterview.Controllers.Commons;

namespace BitdefenderInterview
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            SetDependencyInjection(builder);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
            builder.Services.AddSingleton<IAntivirusEventHandler, AntivirusEventHandler>();
        }
    }
}