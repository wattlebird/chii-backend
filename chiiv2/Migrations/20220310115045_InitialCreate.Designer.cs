﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using chiiv2;

#nullable disable

namespace chiiv2.Migrations
{
    [DbContext(typeof(BangumiContext))]
    [Migration("20220310115045_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("chiiv2.CustomRank", b =>
                {
                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<int>("SciRank")
                        .HasColumnType("int");

                    b.HasKey("SubjectId");

                    b.ToTable("CustomRanks");
                });

            modelBuilder.Entity("chiiv2.Subject", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<int>("CollectCount")
                        .HasColumnType("int");

                    b.Property<int>("DoCount")
                        .HasColumnType("int");

                    b.Property<int>("DroppedCount")
                        .HasColumnType("int");

                    b.Property<int>("FavCount")
                        .HasColumnType("int");

                    b.Property<string>("Infobox")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("NSFW")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NameCN")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OnHoldCount")
                        .HasColumnType("int");

                    b.Property<int>("Platform")
                        .HasColumnType("int");

                    b.Property<int?>("Rank")
                        .HasColumnType("int");

                    b.Property<int>("RateCount")
                        .HasColumnType("int");

                    b.Property<string>("Summary")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Type")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WishCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("chiiv2.SubjectEntity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Alias")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedAlias")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedAlias");

                    b.HasIndex("SubjectId");

                    b.ToTable("SubjectEntities");
                });

            modelBuilder.Entity("chiiv2.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<double>("Confidence")
                        .HasColumnType("float");

                    b.Property<string>("Content")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NormalizedContent")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("SubjectId")
                        .HasColumnType("int");

                    b.Property<int>("UserCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId", "NormalizedContent")
                        .IsUnique()
                        .HasFilter("[NormalizedContent] IS NOT NULL");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("chiiv2.Timestamp", b =>
                {
                    b.Property<DateTime>("Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Date");

                    b.ToTable("Timestamps");
                });

            modelBuilder.Entity("chiiv2.CustomRank", b =>
                {
                    b.HasOne("chiiv2.Subject", "Subject")
                        .WithOne("ScientificRank")
                        .HasForeignKey("chiiv2.CustomRank", "SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("chiiv2.SubjectEntity", b =>
                {
                    b.HasOne("chiiv2.Subject", "Subject")
                        .WithMany("SubjectEntities")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("chiiv2.Tag", b =>
                {
                    b.HasOne("chiiv2.Subject", "Subject")
                        .WithMany("Tags")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("chiiv2.Subject", b =>
                {
                    b.Navigation("ScientificRank");

                    b.Navigation("SubjectEntities");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}
