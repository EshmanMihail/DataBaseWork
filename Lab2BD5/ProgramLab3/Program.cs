using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;
using InfoStruct.Services;
using Microsoft.AspNetCore.Http;
using InfoStruct.Midlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static void Main(string[] args)
    {
        const int cacheTimeSeconds = 258;

        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddRazorPages();

        builder.Services.AddDbContext<HeatSchemeStorageContext>();
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddControllersWithViews();
        builder.Services.AddSession();
        builder.Services.Configure<CookiePolicyOptions>(options =>
            options.CheckConsentNeeded = context => false
        );
        builder.Services.AddTransient<EnterpriceCacheData>();
        builder.Services.AddTransient<HeatConsumerCacheData>();
        builder.Services.AddTransient<HeatNetworkCacheData>();
        builder.Services.AddTransient<HeatPointCacheData>();
        builder.Services.AddTransient<HeatWellCacheData>();
        builder.Services.AddTransient<PipelineSectionCacheData>();
        builder.Services.AddTransient<SteelPipeCacheData>();

        builder.Services.AddTransient(x =>
            new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(cacheTimeSeconds),
            }
        );

        var app = builder.Build();
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseSession();
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();

        app.Map("/Enterprice", Middlewares.Tables.ShowEnterprices);

        int x = 2;
        app.Run(async (context) =>
        {
            x = x * 2;  //  2 * 2 = 4
            await context.Response.WriteAsync($"Result: {x}");
        });
        
        //MVC here!
        app.UseRouting();
       
        app.Run();
    }
}