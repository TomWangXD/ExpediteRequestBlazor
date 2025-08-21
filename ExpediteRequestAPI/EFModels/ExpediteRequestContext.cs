using System;
using System.Collections.Generic;
using ExpediteRequestAPI.EFModels.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ExpediteRequestAPI.EFModels
{
    public partial class ExpediteRequestContext : DbContext
    {
        public ExpediteRequestContext()
        {
        }

        public ExpediteRequestContext(DbContextOptions<ExpediteRequestContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<Document> Documents { get; set; }
        public virtual DbSet<ExpediteReason> ExpediteReasons { get; set; }
        public virtual DbSet<ApproverStatusLookup> ApproverStatusLookups { get; set; }
        public virtual DbSet<ExpediteRequestsExtended> ExpediteRequestsExtended { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Approval>(entity =>
            {
                entity.ToTable("Approval");

                entity.Property(e => e.ApprovalStatus).HasMaxLength(50);

                entity.Property(e => e.ApprovalType).HasMaxLength(50);

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.DocumentId).HasColumnName("DocumentID");

                entity.Property(e => e.Remarks).HasColumnType("ntext");

                entity.Property(e => e.Signature).HasMaxLength(50);

                entity.Property(e => e.Title).HasMaxLength(50);
            });

            modelBuilder.Entity<ApproverStatusLookup>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Approver__3214EC0793AC92C2");

                entity.ToTable("ApproverStatusLookup");

                entity.HasIndex(e => e.StatusName, "UQ__Approver__05E7698AA20505E3").IsUnique();

                entity.Property(e => e.StatusName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ExpediteReason>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Expedite__3214EC0791659733");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Document>(entity =>
            {
                entity.ToTable("Document");

                entity.Property(e => e.CoNum)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Comments)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.Created).HasDefaultValueSql("(sysdatetime())");

                entity.Property(e => e.CreatedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CustomerName).HasMaxLength(60);

                entity.Property(e => e.DocumentNo).HasMaxLength(10);

                entity.Property(e => e.Gkey).HasDefaultValueSql("(newid())");

                entity.Property(e => e.Dpas).HasColumnName("DPAS");

                entity.Property(e => e.Ipn).HasMaxLength(30);

                entity.Property(e => e.IpnDescription).HasMaxLength(40);

                entity.Property(e => e.Job).HasMaxLength(20);

                entity.Property(e => e.ModifiedBy)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.NewShipType).HasMaxLength(10);

                entity.Property(e => e.PartialQuantity).HasColumnType("decimal(19, 8)");

                entity.Property(e => e.PlanCode).HasMaxLength(3);

                entity.Property(e => e.QtyOrdered).HasColumnType("decimal(19, 8)");

                entity.Property(e => e.Reason)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ShelfLifeRequirement)
                    .HasMaxLength(255)
                    .HasDefaultValueSql("('')");

                entity.Property(e => e.ShipSite).HasMaxLength(8);

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.Um)
                    .HasMaxLength(3)
                    .HasColumnName("UM");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
