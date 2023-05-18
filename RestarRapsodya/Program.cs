using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.EntityFrameworkCore;
using RestarRapsodya.Data;
using RestarRapsodya.Extension;
using RestarRapsodya.Models;
using RestarRapsodya.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<RestarRapsodyaContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerConnection") ?? throw new InvalidOperationException("Connection string 'SqlServerConnection' not found.")));

builder.Services.AddAuthentication("CookieAuth")
        .AddCookie("CookieAuth", config =>
        {
            config.Cookie.Name = "RestarRapsodya.Cookie";
            config.LoginPath = "/Usuario/Iniciar_Sesion";
        });

builder.Services.AddAuthorization(config =>
    {
        config.AddPolicy("AdminPolicy", policy =>
            policy.RequireRole("Administrador"));
        config.AddPolicy("ClientePolicy", policy =>
            policy.RequireRole("Cliente"));
    });

builder.Services.AddControllersWithViews();

var contextAssembly = new CustomAssemblyLoadContext();
contextAssembly.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "LibreriaPDF/libwkhtmltox.dll"));
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));

builder.Services.AddScoped<IEmailService,EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var dbContext = services.GetRequiredService<RestarRapsodyaContext>();
       // dbContext.Database.EnsureDeleted();
       // dbContext.Database.EnsureCreated();
        SeedData.Initialize(services);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error manipulando el seed data " + ex.Message);
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
