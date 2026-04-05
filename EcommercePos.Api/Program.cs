using Microsoft.EntityFrameworkCore;
using EcommercePos.Persistence.Data;
using EcommercePos.Api.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
        policyBuilder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.UseCors("AllowAll");

// Core Entities
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

// Additional Entities
app.MapDeliveryZoneEndpoints();
app.MapColorEndpoints();
app.MapTagEndpoints();
app.MapPickupPointEndpoints();
app.MapWarehouseEndpoints();
app.MapRoleEndpoints();
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

app.Run();
