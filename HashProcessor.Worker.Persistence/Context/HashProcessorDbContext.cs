using HashProcessor.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HashProcessor.Worker.Persistence.Context;

public class HashProcessorDbContext : DbContext
{
    public HashProcessorDbContext(DbContextOptions<HashProcessorDbContext> options) : base(options)
    {
    }

    public DbSet<Hash> Hashes { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
