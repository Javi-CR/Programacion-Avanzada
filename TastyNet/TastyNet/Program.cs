using TastyNet.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true; 
    options.Cookie.IsEssential = true; 
});
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IHashService, HashService>();

// Configure authentication (Cookie example)
builder.Services.AddAuthentication("CookieAuth")
.AddCookie("CookieAuth", options =>
{
   options.LoginPath = "/Login/Login";
   options.AccessDeniedPath = "/Home/Index";
});


var app = builder.Build();


app.UseExceptionHandler("/Home/Error");
app.UseHsts();


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
