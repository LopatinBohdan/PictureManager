using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace PictureManager.Entities;

public partial class ImageDbContext : DbContext
{
    public ImageDbContext()
    {
    }

    public ImageDbContext(DbContextOptions<ImageDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Picture> Pictures { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ImageDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Picture>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Pictures__3214EC07195F71CA");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Data).HasMaxLength(1);
            entity.Property(e => e.Path).HasMaxLength(1);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
