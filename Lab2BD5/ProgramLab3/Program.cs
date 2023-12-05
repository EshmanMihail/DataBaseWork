using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;
using InfoStruct;
using InfoStruct.Middlewares;

public class Program
{
    private static void Main(string[] args)
    {
        const int cacheTimeSeconds = 258;

        var builder = WebApplication.CreateBuilder(args);



        var app = builder.Build();

        int x = 2;
        app.Run(async (context) =>
        {
            x = x * 2;  //  2 * 2 = 4
            await context.Response.WriteAsync($"Result: {x}");
        });

        app.Run();
    }
}