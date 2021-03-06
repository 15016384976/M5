﻿// <auto-generated />
using System;
using M5.API.USER.Framework.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace M5.API.USER.Migrations
{
    [DbContext(typeof(EFContext))]
    partial class EFContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.3-rtm-32065")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("M5.API.USER.Framework.Domain.Aggregates.Label", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<Guid>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Label");
                });

            modelBuilder.Entity("M5.API.USER.Framework.Domain.Aggregates.Property", b =>
                {
                    b.Property<Guid>("UserId");

                    b.Property<string>("Key")
                        .HasMaxLength(50);

                    b.Property<string>("Value")
                        .HasMaxLength(50);

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("UserId", "Key", "Value");

                    b.ToTable("Property");
                });

            modelBuilder.Entity("M5.API.USER.Framework.Domain.Aggregates.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("M5.API.USER.Framework.Domain.Aggregates.Label", b =>
                {
                    b.HasOne("M5.API.USER.Framework.Domain.Aggregates.User")
                        .WithMany("Labels")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("M5.API.USER.Framework.Domain.Aggregates.Property", b =>
                {
                    b.HasOne("M5.API.USER.Framework.Domain.Aggregates.User")
                        .WithMany("Properties")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
