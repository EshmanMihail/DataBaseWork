using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ModelsLibrary.Views;

namespace ModelsLibrary.Models;

public partial class HeatSchemeStorageContext : DbContext
{
    public HeatSchemeStorageContext()
    {
    }

    public HeatSchemeStorageContext(DbContextOptions<HeatSchemeStorageContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Enterprise> Enterprises { get; set; }

    public virtual DbSet<HeatConsumer> HeatConsumers { get; set; }

    public virtual DbSet<HeatNetwork> HeatNetworks { get; set; }

    public virtual DbSet<HeatPoint> HeatPoints { get; set; }

    public virtual DbSet<HeatWell> HeatWells { get; set; }

    public virtual DbSet<PipelineSection> PipelineSections { get; set; }

    public virtual DbSet<SteelPipe> SteelPipes { get; set; }

    public virtual DbSet<ViewHeatConsumer> ViewHeatConsumers { get; set; }

    public virtual DbSet<ViewHeatNetwork> ViewHeatNetworks { get; set; }

    public virtual DbSet<ViewHeatPoint> ViewHeatPoints { get; set; }

    public virtual DbSet<ViewPipelineSection> ViewPipelineSections { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=HeatSchemeStorage; Trusted_Connection=True; TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Enterprise>(entity =>
        {
            entity.HasKey(e => e.EnterpriseId).HasName("PK__Enterpri__52DEA5462ED0C064");

            entity.ToTable("Enterprise");

            entity.Property(e => e.EnterpriseId)
                .ValueGeneratedNever()
                .HasColumnName("EnterpriseID");
            entity.Property(e => e.EnterpriseName).HasMaxLength(50);
            entity.Property(e => e.ManagementOrganization).HasMaxLength(50);
        });

        modelBuilder.Entity<HeatConsumer>(entity =>
        {
            entity.HasKey(e => e.ConsumerId).HasName("PK__HeatCons__63BBE99A4A9E83E8");

            entity.ToTable("HeatConsumer");

            entity.Property(e => e.ConsumerId)
                .ValueGeneratedNever()
                .HasColumnName("ConsumerID");
            entity.Property(e => e.CalculatedPower).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ConsumerName).HasMaxLength(50);
            entity.Property(e => e.NetworkId).HasColumnName("NetworkID");

            entity.HasOne(d => d.Network).WithMany(p => p.HeatConsumers)
                .HasForeignKey(d => d.NetworkId)
                .HasConstraintName("FK__HeatConsu__Netwo__45F365D3");
        });

        modelBuilder.Entity<HeatNetwork>(entity =>
        {
            entity.HasKey(e => e.NetworkId).HasName("PK__HeatNetw__4DD57BEB08CF76F4");

            entity.ToTable("HeatNetwork");

            entity.Property(e => e.NetworkId)
                .ValueGeneratedNever()
                .HasColumnName("NetworkID");
            entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseID");
            entity.Property(e => e.NetworkName).HasMaxLength(50);
            entity.Property(e => e.NetworkType).HasMaxLength(20);

            entity.HasOne(d => d.Enterprise).WithMany(p => p.HeatNetworks)
                .HasForeignKey(d => d.EnterpriseId)
                .HasConstraintName("FK__HeatNetwo__Enter__4316F928");
        });

        modelBuilder.Entity<HeatPoint>(entity =>
        {
            entity.HasKey(e => e.PointId).HasName("PK__HeatPoin__40A9778192CC5EF2");

            entity.ToTable("HeatPoint");

            entity.Property(e => e.PointId)
                .ValueGeneratedNever()
                .HasColumnName("PointID");
            entity.Property(e => e.NetworkId).HasColumnName("NetworkID");
            entity.Property(e => e.PointName).HasMaxLength(50);

            entity.HasOne(d => d.Network).WithMany(p => p.HeatPoints)
                .HasForeignKey(d => d.NetworkId)
                .HasConstraintName("FK__HeatPoint__Netwo__440B1D61");
        });

        modelBuilder.Entity<HeatWell>(entity =>
        {
            entity.HasKey(e => e.WellId).HasName("PK__HeatWell__E955CC1C4EA60E2D");

            entity.ToTable("HeatWell");

            entity.Property(e => e.WellId)
                .ValueGeneratedNever()
                .HasColumnName("WellID");
            entity.Property(e => e.NetworkId).HasColumnName("NetworkID");
            entity.Property(e => e.WellName).HasMaxLength(50);

            entity.HasOne(d => d.Network).WithMany(p => p.HeatWells)
                .HasForeignKey(d => d.NetworkId)
                .HasConstraintName("FK__HeatWell__Networ__44FF419A");
        });

        modelBuilder.Entity<PipelineSection>(entity =>
        {
            entity.HasKey(e => e.SectionId).HasName("PK__Pipeline__80EF08922EA94606");

            entity.ToTable("PipelineSection");

            entity.Property(e => e.SectionId)
                .ValueGeneratedNever()
                .HasColumnName("SectionID");
            entity.Property(e => e.Diameter).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.PipelineLength).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Thickness).HasColumnType("decimal(18, 0)");

            entity.HasOne(d => d.EndNodeNumberNavigation).WithMany(p => p.PipelineSectionEndNodeNumberNavigations)
                .HasForeignKey(d => d.EndNodeNumber)
                .HasConstraintName("FK__PipelineS__EndNo__47DBAE45");

            entity.HasOne(d => d.StartNodeNumberNavigation).WithMany(p => p.PipelineSectionStartNodeNumberNavigations)
                .HasForeignKey(d => d.StartNodeNumber)
                .HasConstraintName("FK__PipelineS__Start__46E78A0C");
        });

        modelBuilder.Entity<SteelPipe>(entity =>
        {
            entity.HasKey(e => e.PipeId).HasName("PK__SteelPip__30B4C3BD5E09EAE6");

            entity.ToTable("SteelPipe");

            entity.Property(e => e.PipeId)
                .ValueGeneratedNever()
                .HasColumnName("PipeID");
            entity.Property(e => e.LinearInternalVolume).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.LinearWeight).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.OuterDiameter).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.Thickness).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<ViewHeatConsumer>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewHeatConsumers");

            entity.Property(e => e.CalculatedPower).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.ConsumerId).HasColumnName("ConsumerID");
            entity.Property(e => e.ConsumerName).HasMaxLength(50);
            entity.Property(e => e.NetworkName).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewHeatNetwork>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewHeatNetworks");

            entity.Property(e => e.EnterpriseName).HasMaxLength(50);
            entity.Property(e => e.NetworkId).HasColumnName("NetworkID");
            entity.Property(e => e.NetworkName).HasMaxLength(50);
            entity.Property(e => e.NetworkType).HasMaxLength(20);
        });

        modelBuilder.Entity<ViewHeatPoint>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewHeatPoints");

            entity.Property(e => e.NetworkName).HasMaxLength(50);
            entity.Property(e => e.PointId).HasColumnName("PointID");
            entity.Property(e => e.PointName).HasMaxLength(50);
        });

        modelBuilder.Entity<ViewPipelineSection>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("ViewPipelineSections");

            entity.Property(e => e.Diameter).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.EndNode).HasMaxLength(50);
            entity.Property(e => e.PipelineLength).HasColumnType("decimal(18, 0)");
            entity.Property(e => e.SectionId).HasColumnName("SectionID");
            entity.Property(e => e.StartNode).HasMaxLength(50);
            entity.Property(e => e.Thickness).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
