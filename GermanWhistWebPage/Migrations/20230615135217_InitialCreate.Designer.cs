﻿// <auto-generated />
using System;
using GermanWhistWebPage.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace GermanWhistWebPage.Migrations
{
    [DbContext(typeof(GameContext))]
    [Migration("20230615135217_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.7");

            modelBuilder.Entity("GermanWhistWebPage.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("CardStack")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("CurrentPlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("HandPlayer1")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("HandPlayer2")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int?>("NewHandCardIdPlayer1")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("NewHandCardIdPlayer2")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PlayedCardIdPlayer1")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PlayedCardIdPlayer2")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player1Id")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Player2Id")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PreviousPlayedCardIdPlayer1")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("PreviousPlayedCardIdPlayer2")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundScorePlayer1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("RoundScorePlayer2")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StartingPlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TargetScore")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalScorePlayer1")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TotalScorePlayer2")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrickStartPlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("TrickWiningPlayerPreviousRound")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TrumpSuit")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Player1Id");

                    b.HasIndex("Player2Id");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("GermanWhistWebPage.Models.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Players");
                });

            modelBuilder.Entity("GermanWhistWebPage.Models.Game", b =>
                {
                    b.HasOne("GermanWhistWebPage.Models.Player", "Player1")
                        .WithMany()
                        .HasForeignKey("Player1Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("GermanWhistWebPage.Models.Player", "Player2")
                        .WithMany()
                        .HasForeignKey("Player2Id")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Player1");

                    b.Navigation("Player2");
                });
#pragma warning restore 612, 618
        }
    }
}
