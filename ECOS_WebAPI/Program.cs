

using ECOS_WebAPI.Agents;
using ECOS_WebAPI.Models;
using ECOS_WebAPI.Service;

namespace ECOS_WebAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<OpenRouterSettings>(
            builder.Configuration.GetSection("OpenRouter")
            );

            builder.Services.AddHttpClient<OpenRouterService>();
            builder.Services.AddScoped<ResearchAgent>();
            builder.Services.AddScoped<EvaluationAgent>();
            builder.Services.AddScoped<PipelineService>();

            //builder.Services.AddSingleton<OpenRouterService>(sp =>
            //{
            //    var config = sp.GetRequiredService<IConfiguration>();
            //    var apiKey = config["OpenRouter:ApiKey"];

            //    return new OpenRouterService(apiKey);
            //});


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
    }
}
