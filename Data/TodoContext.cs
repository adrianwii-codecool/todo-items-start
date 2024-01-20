using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Reflection.Emit;
using TodoItems.Models;

namespace TodoItems.Data
{
    public class TodoContext : DbContext
    {
        public DbSet<TodoItem> TodoItems { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // required by IdentityDbContext

            modelBuilder.Entity<TodoItem>().HasData(new List<TodoItem>
        {
            new TodoItem
            {
                Id = 1, Name = "Clean living room", IsComplete = false
            }
            });
        }
    }
}
