using CapaDatos;
using CapaNegocio;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
{
    // Puedes ajustar el buffer predeterminado si hay errores por tamaño
    options.JsonSerializerOptions.MaxDepth = 128; // Opcional
    options.JsonSerializerOptions.DefaultBufferSize = 1024 * 1024; // 1 MB, opcional

    // También puedes habilitar manejo de ciclos si es necesario
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
}); 

// 🟢 Agrega el servicio de autenticación con cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Index";      // Ruta al login
        options.LogoutPath = "/Acceso/CerrarSesion"; // Ruta al logout
    });


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tiempo de expiración de la sesión
    options.Cookie.HttpOnly = true; // Cookie solo accesible por HTTP
    options.Cookie.IsEssential = true; // Necesario para que la sesión funcione
});

Conexion.cn = builder.Configuration.GetConnectionString("Cadena");

var settings = builder.Configuration.GetSection("Paypal").Get<PaypalSettings>();
CN_Paypal paypal = new CN_Paypal(settings);


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Tienda}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
