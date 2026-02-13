namespace eTicaretUygulamasi.Mvc.App.Data
{
    public class DbSeed
    {
        public static async Task SeedAsync(AppDbContext dbContext)
        {
            
            await dbContext.SaveChangesAsync();

        }
    }
}
