using DataAccess.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Data.Context
{
    public class MeasurementsContext : DbContext
    {
        public DbSet<MeasurementEntity> Measurements { get; set; }
        public DbSet<InvalidMesurementEntity> InvalidMesurements { get; set; }
        public DbSet<NotificationEntity> Notifications { get; set; }
        public DbSet<NotificationRuleEntity> NotificationRules { get; set; }
        public DbSet<SensorEntity> Sensors { get; set; }
        public DbSet<SensorTypeEntity> SensorTypes { get; set; }
        public DbSet<ValidationRuleEntity> ValidationRules { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=.\SQLEXPRESS;Database=Measurements;Trusted_Connection=True;");
        }

    }
}
