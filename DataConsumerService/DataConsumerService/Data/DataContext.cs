using DataConsumerService.Model;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace DataConsumerService.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<FileData> FileData { get; set; }
    }
}
