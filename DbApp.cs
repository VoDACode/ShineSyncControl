using Microsoft.EntityFrameworkCore;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Tools;

namespace ShineSyncControl
{
    public class DbApp : DbContext
    {
        public DbSet<UserRoles> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Device> Devices { get; set; }
        public DbSet<DeviceProperty> DeviceProperties { get; set; }
        public DbSet<DeviceGroup> DeviceGroups { get; set; }
        public DbSet<Expression> Expressions { get; set; }
        public DbSet<ActionModel> Actions { get; set; }
        public DbSet<TaskModel> Tasks { get; set; }
        public DbSet<ActionTask> ActionTask { get; set; }
        public DbSet<UserDevice> UserDevices { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }

        public DbApp(DbContextOptions<DbApp> options) : base(options)
        {
            DbInit.Initialize(this);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRoles>()
                .HasKey(p => p.Role);

            modelBuilder.Entity<User>()
                .HasOne(p => p.RoleEntity)
                .WithMany()
                .HasForeignKey(p => p.Role)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasIndex(x => x.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasMany(p => p.Expressions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Actions)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<User>()
                .HasMany(p => p.Tasks)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Device>()
               .HasKey(x => x.Id);

            modelBuilder.Entity<Device>()
               .HasIndex(x => x.Token)
               .IsUnique();

            modelBuilder.Entity<DeviceGroup>()
               .HasKey(x => x.Id);

            modelBuilder.Entity<DeviceGroup>()
                .HasOne(dg => dg.Owner)
                .WithMany()
                .HasForeignKey(dg => dg.OwnerId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DeviceGroup>()
                .HasMany(dg => dg.Devices)
                .WithMany(dg => dg.Groups);

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
               .HasKey(x => x.Id);
            modelBuilder.Entity<DeviceProperty>()
                .HasIndex(x => new
                {
                     x.Name,
                     x.DeviceId
                })
                .IsUnique();

            modelBuilder.Entity<ActionModel>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.Device)
                .WithMany()
                .HasForeignKey(t => t.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<TaskModel>()
                .HasOne(t => t.DeviceProperty)
                .WithMany()
                .HasForeignKey(t => t.DevicePropertyId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.Action)
                .WithMany(a => a.ActionTasks)
                .HasForeignKey(at => at.ActionId)
                .IsRequired();

            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.WhenTrueTask)
                .WithMany()
                .HasForeignKey(at => at.WhenTrueTaskId)
                .IsRequired()
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ActionTask>()
                .HasOne(at => at.WhenFalseTask)
                .WithMany()
                .HasForeignKey(at => at.WhenFalseTaskId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDevice>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.DeviceId
                });
            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.User)
                .WithMany()
                .HasForeignKey(ud => ud.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserDevice>()
                .HasOne(ud => ud.Device)
                .WithMany()
                .HasForeignKey(ud => ud.DeviceId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserGroup>()
                .HasKey(x => new
                {
                    x.UserId,
                    x.GroupId
                });

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.User)
                .WithMany()
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserGroup>()
                .HasOne(ug => ug.Group)
                .WithMany()
                .HasForeignKey(ug => ug.GroupId)
                .OnDelete(DeleteBehavior.NoAction);

            base.OnModelCreating(modelBuilder);
        }
    }
}
