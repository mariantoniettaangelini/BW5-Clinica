using ApplicationDbContex.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Aggiungi servizi al contenitore.
builder.Services.AddControllersWithViews();

// Configura il DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configura l'autenticazione
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opt =>
    {
        opt.LoginPath = "/Home/Login";
    });




// Configura le policy di autorizzazione
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("VeterinarioPolicy", policy => policy.RequireRole("Vet"));
    options.AddPolicy("FarmacistaPolicy", policy => policy.RequireRole("Farmacista"));
});

// Configura il servizio di gestione delle autenticazioni
//builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Configura la pipeline HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Login}/{id?}");

app.Run();
