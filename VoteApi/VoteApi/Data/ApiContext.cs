using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using VoteApi.Models;

namespace VoteApi.Data
{
    public class ApiContext : DbContext
    {
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Participant> Participants { get; set; }

        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>()
                .HasKey(nameof(Room.Id));
            modelBuilder.Entity<Room>()
                .Property(nameof(Room.Id))
                .ValueGeneratedOnAdd();
            modelBuilder.Entity<Room>()
                .HasMany(x => x.Participants)
                .WithOne()
                .HasForeignKey(x => x.RoomId)
                .IsRequired()
                .HasConstraintName("FK_Participant_Room");

            modelBuilder.Entity<Participant>()
                .HasKey(nameof(Participant.Id));
            modelBuilder.Entity<Participant>()
                .Property(nameof(Participant.Id))
                .ValueGeneratedOnAdd();
        }

    }
}
