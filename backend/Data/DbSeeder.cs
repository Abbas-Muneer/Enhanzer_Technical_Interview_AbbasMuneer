using Enhanzer.Api.Entities;

namespace Enhanzer.Api.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext dbContext)
    {
        if (!dbContext.Locations.Any())
        {
            dbContext.Locations.AddRange(
                new Location { Code = "LOC001", Name = "Warehouse A" },
                new Location { Code = "LOC002", Name = "Warehouse B" },
                new Location { Code = "LOC003", Name = "Main Store" }
            );
        }

        if (!dbContext.Items.Any())
        {
            dbContext.Items.AddRange(
                new ItemMaster { Name = "Mango" },
                new ItemMaster { Name = "Apple" },
                new ItemMaster { Name = "Banana" },
                new ItemMaster { Name = "Orange" },
                new ItemMaster { Name = "Grapes" },
                new ItemMaster { Name = "Kiwi" },
                new ItemMaster { Name = "Strawberry" }
            );
        }

        await dbContext.SaveChangesAsync();
    }
}
