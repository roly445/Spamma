﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spamma.Api.Web.Infrastructure.Database;

#nullable disable

namespace Spamma.Api.Web.Migrations
{
    [DbContext(typeof(SpammaDataContext))]
    [Migration("20240221010944_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.2");

            modelBuilder.Entity("Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate.Email", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("SentDate")
                        .HasColumnType("TEXT");

                    b.Property<string>("Subject")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Email", (string)null);
                });

            modelBuilder.Entity("Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate.Email", b =>
                {
                    b.OwnsMany("Spamma.Api.Web.Infrastructure.Domain.EmailAggregate.Aggregate.EmailAddress", "EmailAddresses", b1 =>
                        {
                            b1.Property<Guid>("Id")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Address")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.Property<int>("EmailAddressType")
                                .HasColumnType("INTEGER");

                            b1.Property<Guid>("EmailId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("Id");

                            b1.HasIndex("EmailId");

                            b1.ToTable("EmailAddress", (string)null);

                            b1.WithOwner()
                                .HasForeignKey("EmailId");
                        });

                    b.Navigation("EmailAddresses");
                });
#pragma warning restore 612, 618
        }
    }
}
