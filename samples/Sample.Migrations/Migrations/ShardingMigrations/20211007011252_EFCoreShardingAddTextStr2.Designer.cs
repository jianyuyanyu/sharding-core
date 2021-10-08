﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sample.Migrations.EFCores;

namespace Sample.Migrations.Migrations.ShardingMigrations
{
    [DbContext(typeof(DefaultShardingTableDbContext))]
    [Migration("20211007011252_EFCoreShardingAddTextStr2")]
    partial class EFCoreShardingAddTextStr2
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.10")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Sample.Migrations.EFCores.NoShardingTable", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("NoShardingTable");
                });

            modelBuilder.Entity("Sample.Migrations.EFCores.ShardingWithDateTime", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.HasKey("Id");

                    b.ToTable("ShardingWithDateTime");
                });

            modelBuilder.Entity("Sample.Migrations.EFCores.ShardingWithMod", b =>
                {
                    b.Property<string>("Id")
                        .HasMaxLength(128)
                        .IsUnicode(false)
                        .HasColumnType("varchar(128)");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)");

                    b.Property<string>("TextStr")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasDefaultValue("");

                    b.Property<string>("TextStr1")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasDefaultValue("123");

                    b.Property<string>("TextStr2")
                        .IsRequired()
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(128)
                        .HasColumnType("nvarchar(128)")
                        .HasDefaultValue("123");

                    b.HasKey("Id");

                    b.ToTable("ShardingWithMod");
                });
#pragma warning restore 612, 618
        }
    }
}