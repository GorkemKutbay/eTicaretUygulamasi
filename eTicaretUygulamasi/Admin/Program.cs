using App.Data;
using eTicaretUygulamasi.Mvc.App.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();



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
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"] ?? "GIZLISIFRE_VEYA_BURAYA_CONFIGDEN_GELECEK_DEGER"))
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
            OnChallenge = context =>
            {
                context.HandleResponse();
                // eTicaret projesinin login sayfasına yönlendir (Portu vs ayarlayabilirsiniz)
                context.Response.Redirect("https://localhost:7084/Auth/Login"); // Port eklenebilir veya sadece / erişimi olabilir
                return Task.CompletedTask;
            },
            OnForbidden = context =>
            {
                context.Response.Redirect("/Home/Error");
                return Task.CompletedTask;
            }
        };
        options.MapInboundClaims = false;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("admin"));
    options.AddPolicy("Seller", policy => policy.RequireRole("seller"));
    options.AddPolicy("Buyer", policy => policy.RequireRole("buyer"));
});

var app = builder.Build();






if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
   
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseStatusCodePagesWithReExecute("/Error/{0}");
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();