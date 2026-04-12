using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using EcommercePos.Persistence.Data;
using EcommercePos.Persistence.Seeding;
using EcommercePos.Api.Endpoints;
using EcommercePos.Api.Middleware;
using EcommercePos.Api.Services;
using EcommercePos.Application.DependencyInjection;
using EcommercePos.Shared.Common;
using EcommercePos.Persistence.Interceptors;
using EcommercePos.Api.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddScoped<AuditableEntityInterceptor>();
builder.Services.AddDbContext<ApplicationDbContext>((sp, options) =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.AddInterceptors(sp.GetRequiredService<AuditableEntityInterceptor>());
});

// Auto-register all Application handlers, validators, and services
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = System.Text.Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionHandler>();
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    context.Database.EnsureCreated();
}

app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapCategoryEndpoints();
app.MapBrandEndpoints();
app.MapBranchEndpoints();
app.MapProductEndpoints();
app.MapCustomerEndpoints();
app.MapSupplierEndpoints();
app.MapEmployeeEndpoints();
app.MapSaleEndpoints();
app.MapPurchaseEndpoints();
app.MapUnitEndpoints();
app.MapTaxRateEndpoints();
app.MapDeliveryZoneEndpoints();
app.MapColorEndpoints();
app.MapTagEndpoints();
app.MapPickupPointEndpoints();
app.MapWarehouseEndpoints();
app.MapRoleEndpoints();
app.MapPermissionEndpoints();
app.MapMenuEndpoints();
app.MapUserEndpoints();
app.MapShippingMethodEndpoints();
app.MapAttributeTypeEndpoints();
app.MapProductCollectionEndpoints();
app.MapExpenseCategoryEndpoints();
app.MapProductConditionEndpoints();
app.MapCustomerTierEndpoints();
app.MapPaymentStatusEndpoints();
app.MapOrderStatusEndpoints();
app.MapDiscountTypeEndpoints();
app.MapReturnStatusEndpoints();
app.MapShipmentStatusEndpoints();
app.MapPaymentMethodEndpoints();
app.MapCurrencyEndpoints();
app.MapWishlistTypeEndpoints();
app.MapStockMovementTypeEndpoints();
app.MapPosTransactionEndpoints();
app.MapPosReturnEndpoints();
app.MapCashShiftEndpoints();
app.MapPosCounterEndpoints();
app.MapPosTerminalEndpoints();
app.MapCashDrawerEventEndpoints();
app.MapDayEndSummaryEndpoints();
app.MapExpenseEndpoints();
app.MapCartEndpoints();

app.MapStockItemEndpoints();
app.MapStockMovementEndpoints();
app.MapInventoryAdjustmentEndpoints();
app.MapStockTransferEndpoints();
app.MapReorderRuleEndpoints();

app.MapOrderEndpoints();

app.MapCollectionEndpoints();
app.MapSpecificationEndpoints();

app.MapAuthEndpoints();

app.Run();
