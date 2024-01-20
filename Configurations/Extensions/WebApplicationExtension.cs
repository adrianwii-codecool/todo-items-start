using Microsoft.EntityFrameworkCore;
using TodoItems.Data;

namespace TodoItems.Configurations.Extensions
{
    public static class WebApplicationExtension
    {
        public static WebApplication InitializeDatabase(this WebApplication app)
        {
            // recreate & migrate the database on each run, for demo purposes
            using var scope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TodoContext>();
            dbContext.Database.EnsureDeleted();
            dbContext.Database.Migrate();

            return app;
        }
    }
}
