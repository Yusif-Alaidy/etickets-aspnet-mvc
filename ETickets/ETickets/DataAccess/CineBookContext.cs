using System;
using System.Collections.Generic;
using ETickets.Models;
using Microsoft.EntityFrameworkCore;

namespace ETickets.DataAccess;

public partial class CineBookContext : DbContext
{


    public CineBookContext(DbContextOptions<CineBookContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Actor> Actors { get; set; }

    public virtual DbSet<Category> Categories { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Actor>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Actors__3214EC071FA49555");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.ProfilePicture).HasMaxLength(255);

            entity.HasMany(d => d.Movies).WithMany(p => p.Actors)
                .UsingEntity<Dictionary<string, object>>(
                    "ActorMovie",
                    r => r.HasOne<Movie>().WithMany()
                        .HasForeignKey("MoviesId")
                        .HasConstraintName("FK_ActorMovies_Movies"),
                    l => l.HasOne<Actor>().WithMany()
                        .HasForeignKey("ActorsId")
                        .HasConstraintName("FK_ActorMovies_Actors"),
                    j =>
                    {
                        j.HasKey("ActorsId", "MoviesId");
                        j.ToTable("ActorMovies");
                    });
        });

        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Categori__3214EC07336104FE");

            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cinemas__3214EC0797A09E18");

            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.CinemaLogo).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Movies__3214EC079CCC2664");

            entity.Property(e => e.EndDate).HasColumnType("datetime");
            entity.Property(e => e.ImgUrl).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(200);
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.StartDate).HasColumnType("datetime");
            entity.Property(e => e.TrailerUrl).HasMaxLength(500);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}