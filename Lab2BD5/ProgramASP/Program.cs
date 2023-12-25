
using InfoStruct.Service;
using Microsoft.Extensions.Caching.Memory;
using ModelsLibrary.Models;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<HeatSchemeStorageContext>();
        builder.Services.AddControllersWithViews(); // Add MVC to DI.
        builder.Services.AddSession();
        builder.Services.AddResponseCaching();

        var app = builder.Build();

        app.UseHttpsRedirection();
        app.UseSession();
        app.UseResponseCaching();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapControllerRoute(
            name: "Enterprise",
            pattern: "{controller=Enterprise}/{action=ShowTable}/{id?}");

        app.MapControllerRoute(
            name: "HeatNetwork",
            pattern: "{controller=HeatNetwork}/{action=ShowTable}/{id?}");

        app.MapControllerRoute(
            name: "HeatConsumer",
            pattern: "{controller=HeatConsumer}/{action=ShowTable}/{id?}");

        app.MapControllerRoute(
            name: "HeatWell",
            pattern: "{controller=HeatWell}/{action=ShowTable}/{id?}");
        app.MapControllerRoute(
            name: "PipelineSection",
            pattern: "{controller=PipelineSection}/{action=ShowTable}/{id?}");
        app.MapControllerRoute(
            name: "SteelPipe",
            pattern: "{controller=SteelPipe}/{action=ShowTable}/{id?}");

        app.Run();
    }
}