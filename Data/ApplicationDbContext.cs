using Microsoft.EntityFrameworkCore;
using PolyTrack.API.Models;

namespace PolyTrack.API.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<SalesInquiry> SalesInquiries { get; set; }
    public DbSet<FeasibilityReview> FeasibilityReviews { get; set; }
    public DbSet<CostingQuote> CostingQuotes { get; set; }
    public DbSet<SalesOrder> SalesOrders { get; set; }
    public DbSet<ProductionPlan> ProductionPlans { get; set; }
    public DbSet<Procurement> Procurements { get; set; }
    public DbSet<ShopFloor> ShopFloorRecords { get; set; }
    public DbSet<FinalQC> FinalQCReports { get; set; }
    public DbSet<Packing> PackingRecords { get; set; }
    public DbSet<Dispatch> DispatchRecords { get; set; }
    public DbSet<WasteRecord> WasteRecords { get; set; }
}
