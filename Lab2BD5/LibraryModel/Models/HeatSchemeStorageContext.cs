using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lab2BD5
{
    public partial class HeatSchemeStorageContext : DbContext
    {
        public virtual DbSet<Enterprise> Enterprise { get; set; }
        public virtual DbSet<HeatConsumer> HeatConsumer { get; set; }
        public virtual DbSet<HeatNetwork> HeatNetwork { get; set; }
        public virtual DbSet<HeatPoint> HeatPoint { get; set; }
        public virtual DbSet<HeatWell> HeatWell { get; set; }
        public virtual DbSet<PipelineSection> PipelineSection { get; set; }
        public virtual DbSet<SteelPipe> SteelPipe { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            #warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
            optionsBuilder.UseSqlServer(@"Server=.;Database=HeatSchemeStorage;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Enterprise>(entity =>
            {
                entity.Property(e => e.EnterpriseId)
                    .HasColumnName("EnterpriseID")
                    .ValueGeneratedNever();

                entity.Property(e => e.EnterpriseName).HasMaxLength(50);

                entity.Property(e => e.ManagementOrganization).HasMaxLength(50);
            });

            modelBuilder.Entity<HeatConsumer>(entity =>
            {
                entity.HasKey(e => e.ConsumerId)
                    .HasName("PK__HeatCons__63BBE99A4A9E83E8");

                entity.Property(e => e.ConsumerId)
                    .HasColumnName("ConsumerID")
                    .ValueGeneratedNever();

                entity.Property(e => e.CalculatedPower).HasColumnType("decimal");

                entity.Property(e => e.ConsumerName).HasMaxLength(50);

                entity.Property(e => e.NetworkId).HasColumnName("NetworkID");

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.HeatConsumer)
                    .HasForeignKey(d => d.NetworkId)
                    .HasConstraintName("FK__HeatConsu__Netwo__45F365D3");
            });

            modelBuilder.Entity<HeatNetwork>(entity =>
            {
                entity.HasKey(e => e.NetworkId)
                    .HasName("PK__HeatNetw__4DD57BEB08CF76F4");

                entity.Property(e => e.NetworkId)
                    .HasColumnName("NetworkID")
                    .ValueGeneratedNever();

                entity.Property(e => e.EnterpriseId).HasColumnName("EnterpriseID");

                entity.Property(e => e.NetworkName).HasMaxLength(50);

                entity.Property(e => e.NetworkType).HasMaxLength(20);

                entity.HasOne(d => d.Enterprise)
                    .WithMany(p => p.HeatNetwork)
                    .HasForeignKey(d => d.EnterpriseId)
                    .HasConstraintName("FK__HeatNetwo__Enter__4316F928");
            });

            modelBuilder.Entity<HeatPoint>(entity =>
            {
                entity.HasKey(e => e.PointId)
                    .HasName("PK__HeatPoin__40A9778192CC5EF2");

                entity.Property(e => e.PointId)
                    .HasColumnName("PointID")
                    .ValueGeneratedNever();

                entity.Property(e => e.NetworkId).HasColumnName("NetworkID");

                entity.Property(e => e.PointName).HasMaxLength(50);

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.HeatPoint)
                    .HasForeignKey(d => d.NetworkId)
                    .HasConstraintName("FK__HeatPoint__Netwo__440B1D61");
            });

            modelBuilder.Entity<HeatWell>(entity =>
            {
                entity.HasKey(e => e.WellId)
                    .HasName("PK__HeatWell__E955CC1C4EA60E2D");

                entity.Property(e => e.WellId)
                    .HasColumnName("WellID")
                    .ValueGeneratedNever();

                entity.Property(e => e.NetworkId).HasColumnName("NetworkID");

                entity.Property(e => e.WellName).HasMaxLength(50);

                entity.HasOne(d => d.Network)
                    .WithMany(p => p.HeatWell)
                    .HasForeignKey(d => d.NetworkId)
                    .HasConstraintName("FK__HeatWell__Networ__44FF419A");
            });

            modelBuilder.Entity<PipelineSection>(entity =>
            {
                entity.HasKey(e => e.SectionId)
                    .HasName("PK__Pipeline__80EF08922EA94606");

                entity.Property(e => e.SectionId)
                    .HasColumnName("SectionID")
                    .ValueGeneratedNever();

                entity.Property(e => e.Diameter).HasColumnType("decimal");

                entity.Property(e => e.LastRepairDate).HasColumnType("date");

                entity.Property(e => e.PipelineLength).HasColumnType("decimal");

                entity.Property(e => e.Thickness).HasColumnType("decimal");

                entity.HasOne(d => d.EndNodeNumberNavigation)
                    .WithMany(p => p.PipelineSectionEndNodeNumberNavigation)
                    .HasForeignKey(d => d.EndNodeNumber)
                    .HasConstraintName("FK__PipelineS__EndNo__47DBAE45");

                entity.HasOne(d => d.StartNodeNumberNavigation)
                    .WithMany(p => p.PipelineSectionStartNodeNumberNavigation)
                    .HasForeignKey(d => d.StartNodeNumber)
                    .HasConstraintName("FK__PipelineS__Start__46E78A0C");
            });

            modelBuilder.Entity<SteelPipe>(entity =>
            {
                entity.HasKey(e => e.PipeId)
                    .HasName("PK__SteelPip__30B4C3BD5E09EAE6");

                entity.Property(e => e.PipeId)
                    .HasColumnName("PipeID")
                    .ValueGeneratedNever();

                entity.Property(e => e.LinearInternalVolume).HasColumnType("decimal");

                entity.Property(e => e.LinearWeight).HasColumnType("decimal");

                entity.Property(e => e.OuterDiameter).HasColumnType("decimal");

                entity.Property(e => e.Thickness).HasColumnType("decimal");
            });
        }
    }
}