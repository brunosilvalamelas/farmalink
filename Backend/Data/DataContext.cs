using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    public DbSet<Patient> Patients { get; set; }
    public DbSet<Medication> Medications { get; set; }
}