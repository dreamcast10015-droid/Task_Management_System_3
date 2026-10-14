using Microsoft.EntityFrameworkCore;
using Task_Management_System.Data;

namespace Task_Management_System
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // --- «·≈÷«›… «·ÃœÌœ…: ≈⁄œ«œ«  «·‹ CORS ··”„«Õ ·‹ Angular »«·« ’«· »«·‹ API ---
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngularDev",
                    policy => policy.WithOrigins("http://localhost:4200") // »Ê—  «·‹ Angular «·«› —«÷Ì
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });
            // -------------------------------------------------------------------------

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // --- «·≈÷«›… «·ÃœÌœ…:  ›⁄Ì· «·‹ CORS ›Ì «·‹ Pipeline ---
            app.UseCors("AllowAngularDev");
            // -------------------------------------------------------

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}