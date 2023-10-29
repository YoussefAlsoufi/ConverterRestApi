﻿// <auto-generated />
using ConverterRestApi.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ConverterRestApi.Migrations
{
    [DbContext(typeof(ConverterRestApiContext))]
    [Migration("20231029113352_cred")]
    partial class cred
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.23")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("ConverterRestApi.Model.DataConverter", b =>
                {
                    b.Property<string>("UnitName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UnitName");

                    b.ToTable("DataUnits");
                });

            modelBuilder.Entity("ConverterRestApi.Model.LengthConverter", b =>
                {
                    b.Property<string>("UnitName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UnitName");

                    b.ToTable("LengthUnits");
                });

            modelBuilder.Entity("ConverterRestApi.Model.TemperatureConverter", b =>
                {
                    b.Property<string>("UnitName")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UnitName");

                    b.ToTable("TempertureUnits");
                });
#pragma warning restore 612, 618
        }
    }
}
