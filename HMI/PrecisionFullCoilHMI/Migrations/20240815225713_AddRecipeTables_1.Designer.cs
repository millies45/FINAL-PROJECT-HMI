﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PrecisionFullCoilHMI.Data;

#nullable disable

namespace PrecisionFullCoilHMI.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20240815225713_AddRecipeTables_1")]
    partial class AddRecipeTables_1
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.8")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PrecisionFullCoilHMI.Models.Job", b =>
                {
                    b.Property<short>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<short>("Id"));

                    b.Property<short>("Bead")
                        .HasColumnType("smallint");

                    b.Property<short>("CleatEdges")
                        .HasColumnType("smallint");

                    b.Property<short>("Cleats")
                        .HasColumnType("smallint");

                    b.Property<short>("Connector")
                        .HasColumnType("smallint");

                    b.Property<short>("DuctType")
                        .HasColumnType("smallint");

                    b.Property<short>("Gauge")
                        .HasColumnType("smallint");

                    b.Property<short>("HoleDie")
                        .HasColumnType("smallint");

                    b.Property<short>("HoleSize")
                        .HasColumnType("smallint");

                    b.Property<short>("Insulation")
                        .HasColumnType("smallint");

                    b.Property<short>("JobNumber")
                        .HasColumnType("smallint");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<short>("Lock")
                        .HasColumnType("smallint");

                    b.Property<short>("PinSpacing")
                        .HasColumnType("smallint");

                    b.Property<short>("Quantity")
                        .HasColumnType("smallint");

                    b.Property<int>("RecipeId")
                        .HasColumnType("integer");

                    b.Property<short>("Sealant")
                        .HasColumnType("smallint");

                    b.Property<short>("SideA")
                        .HasColumnType("smallint");

                    b.Property<short>("SideAHoles")
                        .HasColumnType("smallint");

                    b.Property<short>("SideB")
                        .HasColumnType("smallint");

                    b.Property<short>("SideBHoles")
                        .HasColumnType("smallint");

                    b.HasKey("Id");

                    b.HasIndex("RecipeId");

                    b.ToTable("Jobs");
                });

            modelBuilder.Entity("PrecisionFullCoilHMI.Models.Recipe", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("LastUpdateDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<short>("NumberOfJobs")
                        .HasColumnType("smallint");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Recipes");
                });

            modelBuilder.Entity("PrecisionFullCoilHMI.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Definetion")
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NodeId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("Subscribe")
                        .HasColumnType("boolean");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("PrecisionFullCoilHMI.Models.Job", b =>
                {
                    b.HasOne("PrecisionFullCoilHMI.Models.Recipe", "Recipe")
                        .WithMany("Jobs")
                        .HasForeignKey("RecipeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Recipe");
                });

            modelBuilder.Entity("PrecisionFullCoilHMI.Models.Recipe", b =>
                {
                    b.Navigation("Jobs");
                });
#pragma warning restore 612, 618
        }
    }
}
