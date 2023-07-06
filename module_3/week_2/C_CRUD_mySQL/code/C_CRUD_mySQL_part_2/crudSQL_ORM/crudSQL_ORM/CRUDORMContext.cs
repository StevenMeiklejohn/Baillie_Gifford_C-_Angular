using System;
using Microsoft.EntityFrameworkCore;

namespace crudSQL_ORM
{
    public class CRUDORMContext : DbContext
    {
        string _connectionString;
        public CRUDORMContext(string connectionString)
        {
            this._connectionString = connectionString;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(this._connectionString);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }
    }
}

