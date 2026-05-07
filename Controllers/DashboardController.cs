using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Data;

namespace PolyTrack.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DashboardController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("analytics")]
    public async Task<IActionResult> GetAnalytics()
    {
        // 1. Inquiries by Status (Pie Chart)
        var statusData = await _context.SalesInquiries
            .GroupBy(i => i.Status)
            .Select(g => new { name = g.Key ?? "New", value = g.Count() })
            .ToListAsync();

        // 2. Inquiries by Month (Last 6 months)
        var sixMonthsAgo = DateTime.Now.AddMonths(-5);
        var monthlyData = await _context.SalesInquiries
            .Where(i => i.InquiryDate >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
            .ToListAsync(); // Pull to memory for complex grouping if needed, or simplify

        var groupedMonthly = monthlyData
            .GroupBy(i => new { i.InquiryDate.Year, i.InquiryDate.Month })
            .Select(g => new 
            { 
                month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                count = g.Count(),
                sortKey = g.Key.Year * 100 + g.Key.Month
            })
            .OrderBy(x => x.sortKey)
            .ToList();

        // 3. Revenue by Month
        var revenueDataRaw = await _context.SalesOrders
            .Where(s => s.CreatedAt >= new DateTime(sixMonthsAgo.Year, sixMonthsAgo.Month, 1))
            .ToListAsync();

        var groupedRevenue = revenueDataRaw
            .GroupBy(s => new { s.CreatedAt.Year, s.CreatedAt.Month })
            .Select(g => new 
            { 
                month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMM"),
                revenue = g.Sum(s => s.TotalAmount),
                sortKey = g.Key.Year * 100 + g.Key.Month
            })
            .OrderBy(x => x.sortKey)
            .ToList();

        return Ok(new
        {
            statusDistribution = statusData,
            monthlyInquiries = groupedMonthly,
            monthlyRevenue = groupedRevenue
        });
    }
}
