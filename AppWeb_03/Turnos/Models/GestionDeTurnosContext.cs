using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Turnos.Models;

public partial class GestionDeTurnosContext : DbContext
{
    public GestionDeTurnosContext()
    {
    }

    public GestionDeTurnosContext(DbContextOptions<GestionDeTurnosContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cliente> Clientes { get; set; }

    public virtual DbSet<Obrasocial> Obrasocials { get; set; }

    public virtual DbSet<Profesionale> Profesionales { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(entity =>
        {
            entity.HasKey(e => e.IdClientes).HasName("PK__CLIENTES__731F3B2EDED4B79F");

            entity.ToTable("CLIENTES");

            entity.HasIndex(e => e.CodigoCliente, "UQ__CLIENTES__FC43D39CF357E75E").IsUnique();

            entity.Property(e => e.IdClientes).HasColumnName("Id_Clientes");
            entity.Property(e => e.ApellidoCliente)
                .HasMaxLength(30)
                .HasColumnName("Apellido_Cliente");
            entity.Property(e => e.CodigoCliente)
                .HasMaxLength(25)
                .HasColumnName("Codigo_Cliente");
            entity.Property(e => e.EmailCliente)
                .HasMaxLength(40)
                .HasColumnName("Email_Cliente");
            entity.Property(e => e.IdObraSocialCliente).HasColumnName("Id_Obra_Social_Cliente");
            entity.Property(e => e.NombreCliente)
                .HasMaxLength(30)
                .HasColumnName("Nombre_Cliente");

            entity.HasOne(d => d.IdObraSocialClienteNavigation).WithMany(p => p.Clientes)
                .HasForeignKey(d => d.IdObraSocialCliente)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__CLIENTES__Id_Obr__534D60F1");
        });

        modelBuilder.Entity<Obrasocial>(entity =>
        {
            entity.HasKey(e => e.IdObraSocial).HasName("PK__OBRASOCI__D61119AF8BF5E9B8");

            entity.ToTable("OBRASOCIAL");

            entity.Property(e => e.IdObraSocial).HasColumnName("Id_Obra_Social");
            entity.Property(e => e.CodigoObraSocial)
                .HasMaxLength(25)
                .HasColumnName("Codigo_Obra_Social");
            entity.Property(e => e.NombreObraSocial)
                .HasMaxLength(25)
                .HasColumnName("Nombre_Obra_social");
            entity.Property(e => e.PrecioObraSocial)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("Precio_ObraSocial");
        });

        modelBuilder.Entity<Profesionale>(entity =>
        {
            entity.HasKey(e => e.IdProfesional).HasName("PK__PROFESIO__9E04403FD615C47D");

            entity.ToTable("PROFESIONALES");

            entity.HasIndex(e => e.CodigoProfesional, "UQ__PROFESIO__90105E18B7158DD1").IsUnique();

            entity.Property(e => e.IdProfesional).HasColumnName("Id_Profesional");
            entity.Property(e => e.ApellidoProfesional)
                .HasMaxLength(30)
                .HasColumnName("Apellido_Profesional");
            entity.Property(e => e.CodigoProfesional)
                .HasMaxLength(20)
                .HasColumnName("Codigo_Profesional");
            entity.Property(e => e.FechaDeRecibimiento)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_De_recibimiento");
            entity.Property(e => e.NombreProfesional)
                .HasMaxLength(30)
                .HasColumnName("Nombre_Profesional");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.HasKey(e => e.IdTurno).HasName("PK__TURNO__5CF9003F1BCFF4D6");

            entity.ToTable("TURNO");

            entity.HasIndex(e => e.CodigoTurno, "UQ__TURNO__287442B8DB88FBEE").IsUnique();

            entity.HasIndex(e => e.FechaTurno, "UQ__TURNO__86AAB37D854248BF").IsUnique();

            entity.Property(e => e.IdTurno).HasColumnName("Id_Turno");
            entity.Property(e => e.CodigoTurno)
                .HasMaxLength(20)
                .HasColumnName("Codigo_Turno");
            entity.Property(e => e.FechaTurno)
                .HasColumnType("datetime")
                .HasColumnName("Fecha_Turno");
            entity.Property(e => e.RazonDeTurno)
                .HasMaxLength(50)
                .HasColumnName("Razon_de_Turno");

            entity.HasOne(d => d.Cliente).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TURNO__ClienteId__5441852A");

            entity.HasOne(d => d.Profesional).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.ProfesionalId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TURNO__Profesion__5535A963");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
