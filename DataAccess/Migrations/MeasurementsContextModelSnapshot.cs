﻿// <auto-generated />
using System;
using DataAccess.Data.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DataAccess.Migrations
{
    [DbContext(typeof(MeasurementsContext))]
    partial class MeasurementsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.13")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DataAccess.Data.Entities.MeasurementEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Discriminator")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("SensorEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SensorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("SensorEntityId");

                    b.ToTable("Measurements");

                    b.HasDiscriminator<string>("Discriminator").HasValue("MeasurementEntity");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.NotificationEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("MeasurementId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NotificationMsg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<DateTime>("TimeStamp")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.NotificationRuleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("NotificationMsg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("NotificationType")
                        .HasColumnType("int");

                    b.Property<int>("RuleType")
                        .HasColumnType("int");

                    b.Property<Guid?>("SensorTypeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SensorTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("SensorTypeEntityId");

                    b.ToTable("NotificationRules");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.SensorEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<double?>("Latitude")
                        .HasColumnType("float");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<double?>("Longitude")
                        .HasColumnType("float");

                    b.Property<Guid>("SensorTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("SensorTypeId");

                    b.ToTable("Sensors");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.SensorTypeEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Unit")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SensorTypes");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.ValidationRuleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("ErrorMsg")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("RuleType")
                        .HasColumnType("int");

                    b.Property<Guid?>("SensorTypeEntityId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SensorTypeId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<double>("Value")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("SensorTypeEntityId");

                    b.ToTable("ValidationRules");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.InvalidMesurementEntity", b =>
                {
                    b.HasBaseType("DataAccess.Data.Entities.MeasurementEntity");

                    b.Property<string>("ErrorMessage")
                        .HasColumnType("nvarchar(max)");

                    b.HasDiscriminator().HasValue("InvalidMesurementEntity");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.MeasurementEntity", b =>
                {
                    b.HasOne("DataAccess.Data.Entities.SensorEntity", null)
                        .WithMany("Measurements")
                        .HasForeignKey("SensorEntityId");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.NotificationRuleEntity", b =>
                {
                    b.HasOne("DataAccess.Data.Entities.SensorTypeEntity", null)
                        .WithMany("NotificationRules")
                        .HasForeignKey("SensorTypeEntityId");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.SensorEntity", b =>
                {
                    b.HasOne("DataAccess.Data.Entities.SensorTypeEntity", "SensorType")
                        .WithMany()
                        .HasForeignKey("SensorTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("SensorType");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.ValidationRuleEntity", b =>
                {
                    b.HasOne("DataAccess.Data.Entities.SensorTypeEntity", null)
                        .WithMany("ValidationRules")
                        .HasForeignKey("SensorTypeEntityId");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.SensorEntity", b =>
                {
                    b.Navigation("Measurements");
                });

            modelBuilder.Entity("DataAccess.Data.Entities.SensorTypeEntity", b =>
                {
                    b.Navigation("NotificationRules");

                    b.Navigation("ValidationRules");
                });
#pragma warning restore 612, 618
        }
    }
}
