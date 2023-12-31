using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DentalCore.Data;

public class DentalCoreDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<AppDbContext>();
        optionBuilder.UseSqlServer(
            @"Server=NorthernRival\MSSQLSERVER,1433;Database=DentalCoreDb;Trusted_Connection=True;Encrypt=Optional");
        
        return new AppDbContext(optionBuilder.Options);
    }
}