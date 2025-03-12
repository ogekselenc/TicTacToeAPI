﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TicTacToeAPI.Data;

#nullable disable

namespace TicTacToeAPI.Migrations
{
    [DbContext(typeof(TicTacToeDbContext))]
    [Migration("20250312203626_UpdateGameModel")]
    partial class UpdateGameModel
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.2");

            modelBuilder.Entity("TicTacToeAPI.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("BoardState")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CurrentPlayer")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsGameOver")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Player1")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Player2")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Size")
                        .HasColumnType("INTEGER");

                    b.Property<int>("WinLength")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Winner")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
