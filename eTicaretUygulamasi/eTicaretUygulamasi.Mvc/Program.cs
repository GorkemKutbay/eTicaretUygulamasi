using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("Default");
    options.UseSqlServer(connectionString);


});




// Program.cs içinde, builder.Build() satırından önce ekle:

builder.Services.AddScoped<IDataRepository, DataRepository>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {

        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "eTicaretUygulamasi",
            ValidateAudience = true,
            ValidAudience = "eTicaretUygulamasi",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            RoleClaimType = System.Security.Claims.ClaimTypes.Role,

            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Cookies["access_token"];

                if (!string.IsNullOrEmpty(accessToken))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            },

            OnChallenge = async context =>
            {
                // varsayılan zorlama davranışını engelle
                context.HandleResponse();

                context.Response.Redirect("/Auth/Login");

                await Task.CompletedTask;
            },
            OnForbidden = async context =>
            {
                context.Response.Redirect("/");
                await Task.CompletedTask;
            }

        };

        options.MapInboundClaims = false;


    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("admin"));
    options.AddPolicy("SellerOnly", policy =>
        policy.RequireRole("seller"));
    options.AddPolicy("BuyerOnly", policy =>
        policy.RequireRole("buyer"));

    options.AddPolicy("AllRoles", policy =>
       policy.RequireRole("buyer", "seller", "admin"));  // tum roller

    options.AddPolicy("AdminOrSeller", policy =>
        policy.RequireRole("admin", "seller")); // admin ve seller
    options.AddPolicy("BuyerOrSeller", policy =>
       policy.RequireRole("buyer", "seller")); // buyer ve seller
    options.AddPolicy("BuyerOrAdmin", policy =>
       policy.RequireRole("buyer", "admin")); // buyer ve admin 
});


var app = builder.Build();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
using (var scope = app.Services.CreateScope())
{
    using (var dbcontext = scope.ServiceProvider.GetRequiredService<AppDbContext>())
    {
        await dbcontext.Database.EnsureCreatedAsync();

    }
}

app.Run();
