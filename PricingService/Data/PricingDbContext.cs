using Microsoft.EntityFrameworkCore;
using PricingService.Domain;

namespace PricingService.Data;

public class PricingDbContext : DbContext
{
    public PricingDbContext(DbContextOptions<PricingDbContext> options) : base(options) { }

    public DbSet<TariffEntity> TariffEntities => Set<TariffEntity>();
    public DbSet<BasePriceRuleEntity> BasePriceRules => Set<BasePriceRuleEntity>();
    public DbSet<DiscountMarkupRuleEntity> DiscountMarkupRules => Set<DiscountMarkupRuleEntity>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<TariffEntity>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Code).IsRequired();
            e.HasMany(t => t.BasePriceRules).WithOne().HasForeignKey(r => r.TariffId);
            e.HasMany(t => t.DiscountMarkupRules).WithOne().HasForeignKey(r => r.TariffId);
        });

        SeedData(mb);
    }

    private static void SeedData(ModelBuilder mb)
    {
        mb.Entity<TariffEntity>().HasData(
            new TariffEntity { Id = 1, Code = "TRI" },
            new TariffEntity { Id = 2, Code = "HSI" },
            new TariffEntity { Id = 3, Code = "FAI" },
            new TariffEntity { Id = 4, Code = "CAR" }
        );

        // TRI - Travel
        mb.Entity<BasePriceRuleEntity>().HasData(
            new BasePriceRuleEntity { Id = 1, TariffId = 1, CoverCode = "C1", Condition = null, Expression = "(NUM_OF_ADULTS) * (DESTINATION == \"EUR\" ? 26.00M : 34.00M)" },
            new BasePriceRuleEntity { Id = 2, TariffId = 1, CoverCode = "C2", Condition = null, Expression = "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 26.00M" },
            new BasePriceRuleEntity { Id = 3, TariffId = 1, CoverCode = "C3", Condition = null, Expression = "(NUM_OF_ADULTS + NUM_OF_CHILDREN) * 10.00M" }
        );
        mb.Entity<DiscountMarkupRuleEntity>().HasData(
            new DiscountMarkupRuleEntity { Id = 1, TariffId = 1, Condition = "DESTINATION == \"WORLD\"", Factor = 1.5M }
        );

        // HSI - House
        mb.Entity<BasePriceRuleEntity>().HasData(
            new BasePriceRuleEntity { Id = 4, TariffId = 2, CoverCode = "C1", Condition = "TYP == \"APT\"", Expression = "AREA * 1.25M" },
            new BasePriceRuleEntity { Id = 5, TariffId = 2, CoverCode = "C1", Condition = "TYP == \"HOUSE\"", Expression = "AREA * 1.50M" },
            new BasePriceRuleEntity { Id = 6, TariffId = 2, CoverCode = "C2", Condition = "TYP == \"APT\"", Expression = "AREA * 0.25M" },
            new BasePriceRuleEntity { Id = 7, TariffId = 2, CoverCode = "C2", Condition = "TYP == \"HOUSE\"", Expression = "AREA * 0.45M" },
            new BasePriceRuleEntity { Id = 8, TariffId = 2, CoverCode = "C3", Condition = null, Expression = "30M" },
            new BasePriceRuleEntity { Id = 9, TariffId = 2, CoverCode = "C4", Condition = null, Expression = "50M" }
        );
        mb.Entity<DiscountMarkupRuleEntity>().HasData(
            new DiscountMarkupRuleEntity { Id = 2, TariffId = 2, Condition = "FLOOD == \"YES\"", Factor = 1.50M },
            new DiscountMarkupRuleEntity { Id = 3, TariffId = 2, Condition = "NUM_OF_CLAIM > 1", Factor = 1.25M }
        );

        // FAI - Farm
        mb.Entity<BasePriceRuleEntity>().HasData(
            new BasePriceRuleEntity { Id = 10, TariffId = 3, CoverCode = "C1", Condition = null, Expression = "10M" },
            new BasePriceRuleEntity { Id = 11, TariffId = 3, CoverCode = "C2", Condition = null, Expression = "20M" },
            new BasePriceRuleEntity { Id = 12, TariffId = 3, CoverCode = "C3", Condition = null, Expression = "30M" },
            new BasePriceRuleEntity { Id = 13, TariffId = 3, CoverCode = "C4", Condition = null, Expression = "40M" }
        );
        mb.Entity<DiscountMarkupRuleEntity>().HasData(
            new DiscountMarkupRuleEntity { Id = 4, TariffId = 3, Condition = "FLOOD == \"YES\"", Factor = 1.50M },
            new DiscountMarkupRuleEntity { Id = 5, TariffId = 3, Condition = "NUM_OF_CLAIM > 2", Factor = 2.00M }
        );

        // CAR
        mb.Entity<BasePriceRuleEntity>().HasData(
            new BasePriceRuleEntity { Id = 14, TariffId = 4, CoverCode = "C1", Condition = null, Expression = "100M" }
        );
        mb.Entity<DiscountMarkupRuleEntity>().HasData(
            new DiscountMarkupRuleEntity { Id = 6, TariffId = 4, Condition = "NUM_OF_CLAIM > 2", Factor = 1.50M }
        );
    }
}

public class TariffEntity
{
    public int Id { get; set; }
    public string Code { get; set; } = "";
    public List<BasePriceRuleEntity> BasePriceRules { get; set; } = new();
    public List<DiscountMarkupRuleEntity> DiscountMarkupRules { get; set; } = new();
}

public class BasePriceRuleEntity
{
    public int Id { get; set; }
    public int TariffId { get; set; }
    public string CoverCode { get; set; } = "";
    public string? Condition { get; set; }
    public string Expression { get; set; } = "";
}

public class DiscountMarkupRuleEntity
{
    public int Id { get; set; }
    public int TariffId { get; set; }
    public string Condition { get; set; } = "";
    public decimal Factor { get; set; }
}
