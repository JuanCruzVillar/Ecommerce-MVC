using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eCommerce.Entities;

namespace eCommerce.Data
{
    public partial class DbecommerceContext : DbContext
    {
        public DbecommerceContext() { }

        public DbecommerceContext(DbContextOptions<DbecommerceContext> options)
            : base(options) { }

        // DbSet en plural apuntando a entidades en singular
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<DetalleVentas> DetalleVentas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Carrito> Carritos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Carrito
            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.HasKey(e => e.IdCarrito).HasName("PK__CARRITO__8B4A618C50BBD072");
                entity.ToTable("CARRITO");

                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Carritos)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK__CARRITO__IdClien__49C3F6B7");

                entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.Carritos)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK__CARRITO__IdProdu__4AB81AF0");
            });

            // Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria).HasName("PK__CATEGORI__A3C02A1004CB3D7C");
                entity.ToTable("CATEGORIA");

                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            // Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.IdCliente).HasName("PK__CLIENTE__D5946642A7D1C625");
                entity.ToTable("CLIENTE");

                entity.Property(e => e.Apellidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Contraseña)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Nombres)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Restablecer).HasDefaultValue(false);
            });

            // DetalleVenta
            modelBuilder.Entity<DetalleVentas>(entity =>
            {
                entity.HasKey(e => e.IdDetalleVenta).HasName("PK__DETALLE___AAA5CEC28EC83973");
                entity.ToTable("DETALLE_VENTA");

                entity.Property(e => e.Total).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.IdProductoNavigation).WithMany(p => p.DetalleVenta)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK__DETALLE_V__IdPro__52593CB8");

                entity.HasOne(d => d.IdVentaNavigation).WithMany(p => p.DetalleVenta)
                    .HasForeignKey(d => d.IdVenta)
                    .HasConstraintName("FK__DETALLE_V__IdVen__5165187F");
            });

            // Marca
            modelBuilder.Entity<Marca>(entity =>
            {
                entity.HasKey(e => e.IdMarca).HasName("PK__MARCA__4076A887C7600A88");
                entity.ToTable("MARCA");

                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            // Producto
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.HasKey(e => e.IdProducto).HasName("PK__PRODUCTO__0988921006888CEC");
                entity.ToTable("PRODUCTO");

                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.Descripcion)
                    .HasMaxLength(400)
                    .IsUnicode(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Nombre)
                    .HasMaxLength(400)
                    .IsUnicode(false);
                entity.Property(e => e.NombreImagen)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Precio)
                    .HasDefaultValue(0m)
                    .HasColumnType("decimal(10, 2)");
                entity.Property(e => e.RutaImagen)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdCategoriaNavigation).WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdCategoria)
                    .HasConstraintName("FK__PRODUCTO__IdCate__403A8C7D");

                entity.HasOne(d => d.IdMarcaNavigation).WithMany(p => p.Productos)
                    .HasForeignKey(d => d.IdMarca)
                    .HasConstraintName("FK__PRODUCTO__IdMarc__3F466844");
            });

            // Usuario
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.HasKey(e => e.IdUsuario).HasName("PK__USUARIO__5B65BF97679AF4C5");
                entity.ToTable("USUARIO");

                entity.Property(e => e.Activo).HasDefaultValue(true);
                entity.Property(e => e.Apellidos)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Contraseña)
                    .HasMaxLength(64)
                    .IsUnicode(false);
                entity.Property(e => e.Correo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.Nombres)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Restablecer).HasDefaultValue(true);

                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Usuarios)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK__USUARIO__IdClien__5535A963");
            });

            // Venta
            modelBuilder.Entity<Venta>(entity =>
            {
                entity.HasKey(e => e.IdVenta).HasName("PK__VENTA__BC1240BD415D780E");
                entity.ToTable("VENTA");

                entity.Property(e => e.Contacto)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Direccion)
                    .HasMaxLength(300)
                    .IsUnicode(false);
                entity.Property(e => e.FechaVenta)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.IdProvincia)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.IdTransaccion)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.ImporteTotal).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK__VENTA__IdCliente__4D94879B");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
