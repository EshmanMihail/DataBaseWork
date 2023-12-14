using InfoStruct.MiddlewaresFolder;
using InfoStruct.Service;
using Microsoft.Extensions.Caching.Memory;
using ModelsLibrary.Models;

internal class Program//lab4 with idea
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
        app.UseRouting();
        app.UseAuthorization();
        app.MapRazorPages();
        app.UseSession();

        app.Map("/Enterprice", Middlewares.Tables.ShowEnterprice);
        app.Map("/HeatConsumer", Middlewares.Tables.ShowConsumer);
        app.Map("/HeatNetwork", Middlewares.Tables.ShowNetwork);
        app.Map("/HeatPoint", Middlewares.Tables.ShowPoint);
        app.Map("/HeatWell", Middlewares.Tables.ShowWell);
        app.Map("/Pipeline", Middlewares.Tables.ShowPipeline);

        app.Map("/info", Middlewares.InfoClass.ShowClientInfo);
        app.Map("/searchform1", Middlewares.Search.ShowForm1);
        app.Map("/searchform2", Middlewares.Search.ShowForm2);

        app.Run();
    }
}