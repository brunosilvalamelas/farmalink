using Backend.Entities;
using Microsoft.EntityFrameworkCore;

namespace Backend.Data;

/// <summary>
/// Database context for Entity Framework Core, managing database connections and operations for the application.
/// </summary>
public class DataContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the DataContext class with the specified options.
    /// </summary>
    /// <param name="options">The options to be used by the DbContext.</param>
    public DataContext(DbContextOptions<DataContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for managing Patient entities in the database.
    /// </summary>
    public DbSet<Patient> Patients { get; set; }

    /// <summary>
    /// DbSet for managing Medication entities in the database.
    /// </summary>
    public DbSet<Medication> Medications { get; set; }

    /// <summary>
    /// DbSet for managing Tutor entities in the database.
    /// </summary>
    public DbSet<Tutor> Tutors { get; set; }

    /// <summary>
    /// DbSet for managing User entities in the database.
    /// </summary>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// DbSet for managing Notifications entities in the database.
    /// </summary>
    public DbSet<Notification> Notifications { get; set; }
    
    public DbSet<IntakeInstruction> IntakeInstructions { get; set; }

    public DbSet<Routine> Routines { get; set; }

    


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().ToTable("Users");
        modelBuilder.Entity<Employee>().ToTable("Employees");
        modelBuilder.Entity<Patient>().ToTable("Patients");
        modelBuilder.Entity<Tutor>().ToTable("Tutors");
        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .HasConversion<string>();
    }
}
