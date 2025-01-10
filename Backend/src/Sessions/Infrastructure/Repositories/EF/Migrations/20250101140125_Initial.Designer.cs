﻿// <auto-generated />
using System;
using Sessions.Infrastructure.Repositories.EF.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Sessions.Infrastructure.Repositories.EF.Migrations
{
    [DbContext(typeof(SessionDbContext))]
    [Migration("20250101140125_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Demo.Domain.Model.Bank.Bank", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Banks", (string)null);
                });

            modelBuilder.Entity("Demo.Domain.Model.Input.Input", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int?>("Attempt")
                        .HasColumnType("integer");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ErrorMessage")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.Property<DateTime?>("ProvidedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("RequestParams")
                        .HasColumnType("jsonb");

                    b.Property<string>("RequestType")
                        .HasColumnType("text");

                    b.Property<DateTime?>("RequestedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Value")
                        .HasMaxLength(512)
                        .HasColumnType("character varying(512)");

                    b.HasKey("Id");

                    b.ToTable("Inputs", (string)null);
                });

            modelBuilder.Entity("Demo.Domain.Model.Integration.Integration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("BankId")
                        .HasColumnType("uuid");

                    b.Property<string>("ClientDisplayName")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(256)
                        .HasColumnType("character varying(256)");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BankId");

                    b.ToTable("Integrations", (string)null);
                });

            modelBuilder.Entity("Demo.Domain.Model.Session", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsStarted")
                        .HasColumnType("boolean");

                    b.Property<string>("SelectedBankAccountId")
                        .HasColumnType("text");

                    b.Property<string>("SelectedBankId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("SelectedIntegrationId")
                        .HasMaxLength(128)
                        .HasColumnType("character varying(128)");

                    b.Property<string>("State")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("UpdatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.ToTable("Sessions", (string)null);
                });

            modelBuilder.Entity("Demo.Domain.Model.Input.Input", b =>
                {
                    b.OwnsOne("Demo.Domain.Model.Input.InputError", "Error", b1 =>
                        {
                            b1.Property<Guid>("InputId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Message")
                                .HasMaxLength(512)
                                .HasColumnType("character varying(512)");

                            b1.Property<string>("Type")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.HasKey("InputId");

                            b1.ToTable("Inputs");

                            b1.WithOwner()
                                .HasForeignKey("InputId");
                        });

                    b.Navigation("Error");
                });

            modelBuilder.Entity("Demo.Domain.Model.Integration.Integration", b =>
                {
                    b.HasOne("Demo.Domain.Model.Bank.Bank", "Bank")
                        .WithMany("Integrations")
                        .HasForeignKey("BankId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Bank");
                });

            modelBuilder.Entity("Demo.Domain.Model.Session", b =>
                {
                    b.OwnsOne("Demo.Domain.Model.BankAccounts.BankAccounts", "BankAccounts", b1 =>
                        {
                            b1.Property<Guid>("SessionId")
                                .HasColumnType("uuid");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");

                            b1.OwnsMany("Demo.Domain.Model.BankAccounts.BankAccount", "Accounts", b2 =>
                                {
                                    b2.Property<Guid>("BankAccountsSessionId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b2.Property<int>("Id"));

                                    b2.Property<string>("Number")
                                        .IsRequired()
                                        .HasMaxLength(30)
                                        .HasColumnType("character varying(30)");

                                    b2.HasKey("BankAccountsSessionId", "Id");

                                    b2.ToTable("BankAccounts", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("BankAccountsSessionId");
                                });

                            b1.Navigation("Accounts");
                        });

                    b.OwnsOne("Demo.Domain.Model.TransactionHistory", "TransactionHistory", b1 =>
                        {
                            b1.Property<Guid>("SessionId")
                                .HasColumnType("uuid");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");

                            b1.OwnsMany("Demo.Domain.Model.Transaction", "Transactions", b2 =>
                                {
                                    b2.Property<Guid>("TransactionHistorySessionId")
                                        .HasColumnType("uuid");

                                    b2.Property<int>("Id")
                                        .ValueGeneratedOnAdd()
                                        .HasColumnType("integer");

                                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b2.Property<int>("Id"));

                                    b2.Property<DateTime>("Date")
                                        .HasColumnType("timestamp with time zone");

                                    b2.Property<string>("Description")
                                        .IsRequired()
                                        .HasMaxLength(255)
                                        .HasColumnType("character varying(255)");

                                    b2.Property<int>("Type")
                                        .HasColumnType("integer");

                                    b2.HasKey("TransactionHistorySessionId", "Id");

                                    b2.ToTable("Transactions", (string)null);

                                    b2.WithOwner()
                                        .HasForeignKey("TransactionHistorySessionId");

                                    b2.OwnsOne("Demo.Domain.Model.Money", "Amount", b3 =>
                                        {
                                            b3.Property<Guid>("TransactionHistorySessionId")
                                                .HasColumnType("uuid");

                                            b3.Property<int>("TransactionId")
                                                .HasColumnType("integer");

                                            b3.Property<decimal>("Amount")
                                                .HasColumnType("numeric");

                                            b3.Property<int>("Currency")
                                                .HasColumnType("integer");

                                            b3.HasKey("TransactionHistorySessionId", "TransactionId");

                                            b3.ToTable("Transactions");

                                            b3.WithOwner()
                                                .HasForeignKey("TransactionHistorySessionId", "TransactionId");
                                        });

                                    b2.Navigation("Amount")
                                        .IsRequired();
                                });

                            b1.Navigation("Transactions");
                        });

                    b.OwnsOne("Demo.Domain.Model.User.User", "User", b1 =>
                        {
                            b1.Property<Guid>("SessionId")
                                .HasColumnType("uuid");

                            b1.Property<string>("Name")
                                .IsRequired()
                                .HasMaxLength(128)
                                .HasColumnType("character varying(128)");

                            b1.Property<string>("Nin")
                                .IsRequired()
                                .HasMaxLength(20)
                                .HasColumnType("character varying(20)");

                            b1.HasKey("SessionId");

                            b1.ToTable("Sessions");

                            b1.WithOwner()
                                .HasForeignKey("SessionId");
                        });

                    b.Navigation("BankAccounts")
                        .IsRequired();

                    b.Navigation("TransactionHistory")
                        .IsRequired();

                    b.Navigation("User")
                        .IsRequired();
                });

            modelBuilder.Entity("Demo.Domain.Model.Bank.Bank", b =>
                {
                    b.Navigation("Integrations");
                });
#pragma warning restore 612, 618
        }
    }
}
