using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using ShineSyncControl.Models.DB;
using ShineSyncControl.Models.Requests;
using System;
using System.Security.Cryptography;

namespace ShineSyncControl.Tools
{
    public static class DbInit
    {
        private static bool _isInitialized = false;

        public static void Initialize(DbApp context)
        {
            if (_isInitialized)
            {
                return;
            }
            context.Database.EnsureCreated();
            _isInitialized = true;

            if (!context.Roles.Any())
            {
                context.Roles.AddRange(new List<UserRoles>
                {
                    new UserRoles(UserRoles.User),
                    new UserRoles(UserRoles.Registrar),
                    new UserRoles(UserRoles.Admin)
                });
            }

            InitializeTableWithData<User, User>(
                dbContext: context,
                dataKey: "users",
                getEntityDbSet: (db) => db.Users,
                createNewEntity: (record, index) =>
                {
                    record.Password = PasswordHasher.Hash(record.Password);
                    return record;
                },
                validate: (existingEntity, entity) => existingEntity.Email == entity.Email
            );

           InitializeTableWithData<Device, RegisterDeviceRequest>(
                dbContext: context,
                dataKey: "devices",
                getEntityDbSet: (db) => db.Devices,
                createNewEntity: (record, index) =>
                {
                    var device = new Device()
                    {
                        Id = $"{Guid.NewGuid()}-{DateTime.UtcNow.Ticks}",
                        ActivatedAt = DateTime.UtcNow,
                        IsActive = true,
                        OwnerId = 1,
                        RegisteredAt = DateTime.UtcNow,
                        Type = record.Type
                    };

                    var token = new byte[512];
                    using (var rng = RandomNumberGenerator.Create())
                    {
                        rng.GetBytes(token);
                    }
                    device.Token = Convert.ToBase64String(token).Replace("\\", "").Replace("/", "").Replace("=", "");

                    return device;
                },
                validate: (existingEntity, entity) => false,
                entityAdded: (dbDevice, jsonDevice) =>
                {
                    foreach(var prop in jsonDevice.Properties)
                    {
                        var property = new DeviceProperty()
                        {
                            Device = dbDevice,
                            DeviceId = dbDevice.Id,
                            IsReadOnly = prop.IsReadOnly,
                            Name = prop.PropertyName,
                            PropertyUnit = prop.PropertyUnit,
                            Type = prop.Type
                        };

                        context.DeviceProperties.Add(property);
                    }

                    context.SaveChanges();
                }
            );
        }

        /// <summary>
        /// Initializes a database table with data based on provided configuration.
        /// </summary>
        /// <typeparam name="TableEntity">The type of the entity representing the database table.</typeparam>
        /// <typeparam name="EntityType">The type of the entity from the provided data.</typeparam>
        /// <param name="dbContext">The database context instance.</param>
        /// <param name="dataKey">The key to retrieve data from the configuration.</param>
        /// <param name="getEntityDbSet">A function to get the DbSet for the specified entity.</param>
        /// <param name="createNewEntity">A function that creates a new entity based on the provided data.</param>
        /// <param name="validate">A function to validate data against existing entities.</param>
        /// <exception cref="InvalidDataException">Thrown when invalid or duplicate data is encountered.</exception>
        private static void InitializeTableWithData<TableEntity, EntityType>(
            DbApp dbContext,
            string dataKey,
            Func<DbApp, DbSet<TableEntity>> getEntityDbSet,
            Func<EntityType, int, TableEntity> createNewEntity,
            Func<TableEntity, EntityType, bool> validate,
            Action<TableEntity, EntityType>? entityAdded = default
        ) where TableEntity : class
          where EntityType : class
        {
            var table = getEntityDbSet(dbContext);
            if (table.Any())
            {
                return;
            }

            var fileContent = File.ReadAllText(Path.Combine("src", "dbDefaultData", $"{dataKey}.json"));
            JArray dataConfig = JArray.Parse(fileContent);

            int i = 0;

            foreach (var dataItem in dataConfig)
            {
                var entity = dataItem.ToObject<EntityType>() ?? throw new InvalidDataException($"Invalid data found: {dataItem}");
                if (table.AsEnumerable().Any(existingEntity => validate.Invoke(existingEntity, entity)))
                {
                    Console.WriteLine($"Invalid data found in 'dbDefaultData{Path.DirectorySeparatorChar}{dataKey}.json'. Value: '{entity}' is duplicated.");
                    continue;
                }

                var dbEntity = table.Add(createNewEntity.Invoke(entity, ++i)).Entity;

                if (entityAdded is not null)
                {
                    dbContext.SaveChanges();
                    entityAdded?.Invoke(dbEntity, entity);
                }
            }

            dbContext.SaveChanges();
        }

        private static void CopyFile(string src, string dist, bool overwrite = false)
        {
            string[] parts = dist.Split(Path.DirectorySeparatorChar);

            string path = parts[0];

            for (int i = 1; i < parts.Length - 1; i++)
            {
                var part = Path.Combine(path, parts[i]);
                if (!Directory.Exists(part))
                {
                    Directory.CreateDirectory(part);
                }
                path = part;
            }
            File.Copy(src, Path.Combine(path, parts.Last()), overwrite);
        }
    }
}
