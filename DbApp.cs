using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;

namespace ShineSyncControl
{
    public class DbApp : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceProperty> DeviceProperties { get; set; }
        public DbSet<Expression> Expressions { get; set; }
        public DbSet<Models.DB.Action> Actions { get; set; }
        public DbSet<Models.DB.Task> Tasks { get; set; }
        public DbSet<ActionTask> ActionTask { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public DbApp(DbContextOptions<DbApp> options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<Device>()
               .HasIndex(x => x.Token)
               .IsUnique();

            modelBuilder.Entity<Expression>()
                .HasOne(e => e.Device)
                .WithMany()
                .HasForeignKey(e => e.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Expression>()
                .HasOne(e => e.DeviceProperty)
                .WithMany()
                .HasForeignKey(e => e.DevicePropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DeviceProperty>()
               .HasIndex(x => new { x.DeviceId, x.PropertyName })
               .IsUnique();

            modelBuilder.Entity<Models.DB.Task>()
                .HasOne(t => t.Device)
                .WithMany()
                .HasForeignKey(t => t.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Models.DB.Task>()
                .HasOne(t => t.DeviceProperty)
                .WithMany()
                .HasForeignKey(t => t.DevicePropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActionTask>()
                .HasKey(x =>
                new
                {
                    x.ActionId,
                    x.WhenTrueTaskId,
                    x.WhenFalseTaskId
                });
            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.Action)                   // ActionTask has one Action
                .WithMany(a => a.ActionTasks)              // Action has many ActionTasks
                .HasForeignKey(at => at.ActionId)          // Foreign key in ActionTask
                .IsRequired();

            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.WhenTrueTask)             // ActionTask has one WhenTrueTask (Task)
                .WithMany()                               // No navigation property on Task side
                .HasForeignKey(at => at.WhenTrueTaskId)    // Foreign key in ActionTask
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.WhenFalseTask)            // ActionTask has one WhenFalseTask (Task)
                .WithMany()                               // No navigation property on Task side
                .HasForeignKey(at => at.WhenFalseTaskId)   // Foreign key in ActionTask
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDevice>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.DeviceId
                });

            modelBuilder.Entity<UserGroup>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.GroupId
                });

            base.OnModelCreating(modelBuilder);
        }
    }
}
