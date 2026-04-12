using Microsoft.AspNetCore.Mvc;
using EcommercePos.Application.Features.Report;
using EcommercePos.Api.Extensions;

namespace EcommercePos.Api.Endpoints;

public static class ReportEndpoints
{
    public static void MapReportEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/reports").WithTags("Reports");

        group.MapGet("/dashboard", async (
            GetDashboardReport.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(ct)).ToHttpResult())
        .WithName("GetDashboard")
        .WithSummary("Get dashboard overview");

        group.MapGet("/sales", async (
            [AsParameters] GetSalesReport.Query request,
            GetSalesReport.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToHttpResult())
        .WithName("GetSalesReport")
        .WithSummary("Get sales report");

        group.MapGet("/inventory", async (
            [AsParameters] GetInventoryReport.Query request,
            GetInventoryReport.Handler handler,
            CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToHttpResult())
        .WithName("GetInventoryReport")
        .WithSummary("Get inventory report");
    }
}