using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using WBSA.CurrencyExchangeApp.Data.Entities;

namespace WBSA.CurrencyExchangeApp.Data
{
    public class CurrencyExchangeDbContext:DbContext
    {
        
        public CurrencyExchangeDbContext(DbContextOptions<CurrencyExchangeDbContext> dbContextOptions)
            :base(dbContextOptions) 
        {
            try
            {
                var databaseCreator=Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databaseCreator != null) {
                    if (!databaseCreator.CanConnect()) databaseCreator.Create();

                    if(!databaseCreator.HasTables()) databaseCreator.CreateTables();
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }

        public DbSet<CurrencyExchangeHistory> CurrencyExchangeHistory { get; set; }

    }
}
