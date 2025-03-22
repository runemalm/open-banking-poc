﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sessions.Infrastructure.Persistence.EfCore.Context;

#nullable disable

namespace Sessions.Infrastructure.Persistence.EfCore.Migrations
{
    [DbContext(typeof(SessionDbContext))]
    partial class SessionDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.1");

            modelBuilder.Entity("OpenDDD.Infrastructure.TransactionalOutbox.OutboxEntry", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("EventName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("EventType")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Payload")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ProcessedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("OutboxEntries");
                });

            modelBuilder.Entity("Sessions.Domain.Model.Bank.Bank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Banks", (string)null);
                });

            modelBuilder.Entity("Sessions.Domain.Model.Input.Input", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int?>("Attempt")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("ProvidedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("RequestParams")
                        .HasColumnType("TEXT");

                    b.Property<string>("RequestType")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("RequestedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasMaxLength(512)
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Inputs", (string)null);
                });

            modelBuilder.Entity("Sessions.Domain.Model.Integration.Integration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("BankId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ClientDisplayName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("Integrations", (string)null);
                });

            modelBuilder.Entity("Sessions.Domain.Model.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("BankAccounts")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsStarted")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SelectedBankAccountId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SelectedBankId")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("SelectedIntegrationId")
                        .HasMaxLength(128)
                        .HasColumnType("TEXT");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TransactionHistory")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Sessions", (string)null);
                });

            modelBuilder.Entity("Sessions.Domain.Model.Input.Input", b =>
                {
                    b.OwnsOne("Sessions.Domain.Model.Input.InputError", "Error", b1 =>
                        {
                            b1.Property<Guid>("InputId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Message")
                                .HasMaxLength(512)
                                .HasColumnType("TEXT");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasColumnType("TEXT");

                            b1.HasKey("InputId");

                            b1.ToTable("Inputs");

                            b1.WithOwner()
                                .HasForeignKey("InputId");
                        });

                    b.Navigation("Error");
                });

            modelBuilder.Entity("Sessions.Domain.Model.Integration.Integration", b =>
                {
                    b.HasOne("Sessions.Domain.Model.Bank.Bank", "Bank")
                        .WithMany("Integrations")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");
                });

            modelBuilder.Entity("Sessions.Domain.Model.Session", b =>
                {
                    b.OwnsOne("Sessions.Domain.Model.User.User", "User", b1 =>
                        {
                            b1.Property<Guid>("SessionId")
                                .HasColumnType("TEXT");

                            b1.Property<string>("Name")
                                .HasMaxLength(128)
                                .HasColumnType("TEXT");

                            b1.Property<string>("Nin")
                                .HasMaxLength(20)
                                .HasColumnType("TEXT");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");
                        });

                    b.Navigation("User");
                });

            modelBuilder.Entity("Sessions.Domain.Model.Bank.Bank", b =>
                {
                    b.Navigation("Integrations");
                });
#pragma warning restore 612, 618
        }
    }
}
