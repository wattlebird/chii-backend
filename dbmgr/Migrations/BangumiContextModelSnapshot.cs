﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using dbmgr;

namespace dbmgr.Migrations
{
    [DbContext(typeof(BangumiContext))]
    partial class BangumiContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.7")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("dbmgr.CustomRank", b =>
                {
                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("SciRank")
                        .HasColumnType("integer");

                    b.HasKey("SubjectId");

                    b.ToTable("CustomRanks");
                });

            modelBuilder.Entity("dbmgr.Subject", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("integer");

                    b.Property<DateTime?>("Date")
                        .HasColumnType("timestamp without time zone");

                    b.Property<int>("Favnum")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(500)");

                    b.Property<string>("NameCN")
                        .HasColumnType("varchar(500)");

                    b.Property<int?>("Rank")
                        .HasColumnType("integer");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("varchar(10)");

                    b.Property<int>("Votenum")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("dbmgr.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<double>("Confidence")
                        .HasColumnType("double precision");

                    b.Property<string>("Content")
                        .HasColumnType("varchar(500)");

                    b.Property<int>("SubjectId")
                        .HasColumnType("integer");

                    b.Property<int>("TagCount")
                        .HasColumnType("integer");

                    b.Property<int>("UserCount")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("SubjectId", "Content")
                        .IsUnique();

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("dbmgr.CustomRank", b =>
                {
                    b.HasOne("dbmgr.Subject", "Subject")
                        .WithOne("ScientificRank")
                        .HasForeignKey("dbmgr.CustomRank", "SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("dbmgr.Tag", b =>
                {
                    b.HasOne("dbmgr.Subject", "Subject")
                        .WithMany("Tags")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Subject");
                });

            modelBuilder.Entity("dbmgr.Subject", b =>
                {
                    b.Navigation("ScientificRank");

                    b.Navigation("Tags");
                });
#pragma warning restore 612, 618
        }
    }
}