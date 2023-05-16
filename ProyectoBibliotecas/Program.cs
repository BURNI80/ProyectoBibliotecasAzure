using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using ProyectoBibliotecas.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddTransient<ServiceApiBibliotecas>();

builder.Services.AddTransient<ServiceStorageBlobs>();
string azurekey = builder.Configuration.GetValue<string>("ConnectionStrings:BlobsConnection");
BlobServiceClient blobServiceClient = new BlobServiceClient(azurekey);
builder.Services.AddTransient<BlobServiceClient>(x => blobServiceClient);



builder.Services.AddSession(options => options.IdleTimeout = TimeSpan.FromMinutes(30));

builder.Services.AddAuthentication(options =>
{
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddCookie();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ADMIN", policy => policy.RequireRole("ADMIN","EDITOR"));
});

// Add services to the container.
builder.Services.AddControllersWithViews(
    options => options.EnableEndpointRouting = false);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    await next();

    if (context.Response.StatusCode == 404)
    {
        context.Request.Path = "/Managed/NotFound";
        await next();
    }
});

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseMvc(routes =>
{
    routes.MapRoute(
        name: "Default",
        template: "{controller=Home}/{action=LandingPage}/{id?}"
        );
});
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=LandingPage}/{id?}");

app.Run();
