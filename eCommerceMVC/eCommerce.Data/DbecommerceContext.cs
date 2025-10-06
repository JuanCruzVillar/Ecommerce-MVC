using eCommerce.Entities;
using eCommerceMVC.eCommerce.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace eCommerce.Data
{
    public partial class DbecommerceContext : DbContext
    {
        

        public DbecommerceContext(DbContextOptions<DbecommerceContext> options)
            : base(options) { }

       
        public virtual DbSet<Categoria> Categorias { get; set; }
        public virtual DbSet<Producto> Productos { get; set; }
        public virtual DbSet<Marca> Marcas { get; set; }
        public virtual DbSet<Venta> Ventas { get; set; }
        public virtual DbSet<DetalleVentas> DetalleVentas { get; set; }
        public virtual DbSet<Usuario> Usuarios { get; set; }
        public virtual DbSet<Cliente> Clientes { get; set; }
        public virtual DbSet<Carrito> Carritos { get; set; }
        public virtual DbSet<DireccionEnvio> DireccionesEnvio { get; set; }
        public virtual DbSet<MetodoPago> MetodosPago { get; set; }
        public virtual DbSet<EstadoPedido> EstadosPedido { get; set; }
        public virtual DbSet<HistorialPedido> HistorialPedidos { get; set; }
        public virtual DbSet<Cupon> Cupones { get; set; }

        public virtual DbSet<ProductoEspecificacion> ProductoEspecificaciones { get; set; }

        public virtual DbSet<ProductoImagen> ProductoImagenes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
            modelBuilder.HasAnnotation("ProductVersion", "9.0.8");


            


            // Categoria
            modelBuilder.Entity<Categoria>(entity =>
            {
                entity.HasKey(e => e.IdCategoria);
                entity.ToTable("CATEGORIA");

                entity.Property(e => e.Descripcion)
                      .HasMaxLength(100)
                      .IsUnicode(false);

                entity.Property(e => e.Activo)
                      .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                      .HasDefaultValueSql("(getdate())")
                      .HasColumnType("datetime");

                entity.HasOne(e => e.CategoriaPadre)
                      .WithMany(e => e.SubCategorias)
                      .HasForeignKey(e => e.IdCategoriaPadre)
                      .OnDelete(DeleteBehavior.Restrict);
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

            // Carrito
            modelBuilder.Entity<Carrito>(entity =>
            {
                entity.HasKey(e => e.IdCarrito)
                      .HasName("PK_CARRITO");

                entity.ToTable("CARRITO");

                entity.Property(e => e.IdCarrito).HasColumnName("IdCarrito");
                entity.Property(e => e.IdUsuario).HasColumnName("IdUsuario");
                entity.Property(e => e.IdProducto).HasColumnName("IdProducto");
                entity.Property(e => e.Cantidad).HasColumnName("Cantidad");
                entity.Property(e => e.IdCliente).HasColumnName("IdCliente"); 

                // Relación con Usuario
                entity.HasOne(d => d.IdUsuarioNavigation)
                      .WithMany(p => p.Carritos)
                      .HasForeignKey(d => d.IdUsuario)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_CARRITO_USUARIO");

                // Relación con Producto
                entity.HasOne(d => d.IdProductoNavigation)
                      .WithMany(p => p.Carritos)
                      .HasForeignKey(d => d.IdProducto)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_CARRITO_PRODUCTO");

                
                entity.HasOne(d => d.IdClienteNavigation)
                      .WithMany(p => p.Carritos)
                      .HasForeignKey(d => d.IdCliente)
                      .OnDelete(DeleteBehavior.Restrict)
                      .HasConstraintName("FK_CARRITO_CLIENTE");
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
    .HasMaxLength(256)
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

                // Relaciones existentes
                entity.HasOne(d => d.IdClienteNavigation).WithMany(p => p.Venta)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK__VENTA__IdCliente__4D94879B");

                // Nuevas relaciones
                entity.HasOne(d => d.IdDireccionEnvioNavigation)
                    .WithMany()
                    .HasForeignKey(d => d.IdDireccionEnvio)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_VENTA_DIRECCION_ENVIO");

                entity.HasOne(d => d.IdMetodoPagoNavigation)
                    .WithMany(p => p.Ventas)
                    .HasForeignKey(d => d.IdMetodoPago)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_VENTA_METODO_PAGO");

                entity.HasOne(d => d.IdEstadoPedidoNavigation)
                    .WithMany(p => p.Ventas)
                    .HasForeignKey(d => d.IdEstadoPedido)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_VENTA_ESTADO_PEDIDO");

                entity.HasOne(d => d.IdCuponNavigation)
                    .WithMany(p => p.Ventas)
                    .HasForeignKey(d => d.IdCupon)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_VENTA_CUPON");

                // Nuevas propiedades con valores por defecto corregidos
                entity.Property(e => e.DescuentoAplicado)
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.CostoEnvio)
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.IdEstadoPedido)
                    .HasDefaultValue(1);

                entity.Property(e => e.NotasEspeciales)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FechaEstimadaEntrega)
                    .HasColumnType("datetime");
            });

            // DireccionEnvio
            modelBuilder.Entity<DireccionEnvio>(entity =>
            {
                entity.HasKey(e => e.IdDireccionEnvio);
                entity.ToTable("DIRECCION_ENVIO");

                entity.Property(e => e.NombreCompleto)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Direccion)
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Referencias)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Ciudad)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Provincia)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.CodigoPostal)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Telefono)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EsDireccionPrincipal)
                    .HasDefaultValue(false);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdClienteNavigation)
                    .WithMany(p => p.DireccionesEnvio)
                    .HasForeignKey(d => d.IdCliente)
                    .HasConstraintName("FK_DIRECCION_ENVIO_CLIENTE");
            });

            // MetodoPago
            modelBuilder.Entity<MetodoPago>(entity =>
            {
                entity.HasKey(e => e.IdMetodoPago);
                entity.ToTable("METODO_PAGO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.RequiereDatosAdicionales)
                    .HasDefaultValue(false);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            // EstadoPedido
            modelBuilder.Entity<EstadoPedido>(entity =>
            {
                entity.HasKey(e => e.IdEstadoPedido);
                entity.ToTable("ESTADO_PEDIDO");

                entity.Property(e => e.Nombre)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            // HistorialPedido
            modelBuilder.Entity<HistorialPedido>(entity =>
            {
                entity.HasKey(e => e.IdHistorialPedido);
                entity.ToTable("HISTORIAL_PEDIDO");

                entity.Property(e => e.Comentarios)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.FechaCambio)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdVentaNavigation)
                    .WithMany(p => p.HistorialPedidos)
                    .HasForeignKey(d => d.IdVenta)
                    .HasConstraintName("FK_HISTORIAL_PEDIDO_VENTA");

                entity.HasOne(d => d.IdEstadoPedidoNavigation)
                    .WithMany(p => p.HistorialPedidos)
                    .HasForeignKey(d => d.IdEstadoPedido)
                    .HasConstraintName("FK_HISTORIAL_PEDIDO_ESTADO");

                entity.HasOne(d => d.IdUsuarioNavigation)
                    .WithMany(p => p.HistorialPedidos)
                    .HasForeignKey(d => d.IdUsuario)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("FK_HISTORIAL_PEDIDO_USUARIO");
            });

            // Cupon
            modelBuilder.Entity<Cupon>(entity =>
            {
                entity.HasKey(e => e.IdCupon);
                entity.ToTable("CUPON");

                entity.Property(e => e.Codigo)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasIndex(e => e.Codigo)
                    .IsUnique()
                    .HasDatabaseName("UQ_CUPON_CODIGO");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.DescuentoFijo)
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.DescuentoPorcentaje)
                    .HasColumnType("decimal(5, 2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.MontoMinimo)
                    .HasColumnType("decimal(10, 2)")
                    .HasDefaultValue(0m);

                entity.Property(e => e.UsosMaximos)
                    .HasDefaultValue(1);

                entity.Property(e => e.UsosActuales)
                    .HasDefaultValue(0);

                entity.Property(e => e.FechaInicio)
                    .HasColumnType("datetime");

                entity.Property(e => e.FechaVencimiento)
                    .HasColumnType("datetime");

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
            });

            // ProductoEspecificacion
            modelBuilder.Entity<ProductoEspecificacion>(entity =>
            {
                entity.HasKey(e => e.IdEspecificacion);
                entity.ToTable("PRODUCTO_ESPECIFICACION");

                entity.Property(e => e.Clave)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Valor)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Orden)
                    .HasDefaultValue(0);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.Especificaciones)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK_PRODUCTO_ESPECIFICACION_PRODUCTO");
            });

            // ProductoImagen
            modelBuilder.Entity<ProductoImagen>(entity =>
            {
                entity.HasKey(e => e.IdImagen);
                entity.ToTable("PRODUCTO_IMAGEN");

                entity.Property(e => e.RutaImagen)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.NombreImagen)
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Orden)
                    .HasDefaultValue(0);

                entity.Property(e => e.EsPrincipal)
                    .HasDefaultValue(false);

                entity.Property(e => e.Activo)
                    .HasDefaultValue(true);

                entity.Property(e => e.FechaRegistro)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.IdProductoNavigation)
                    .WithMany(p => p.Imagenes)
                    .HasForeignKey(d => d.IdProducto)
                    .HasConstraintName("FK_PRODUCTO_IMAGEN_PRODUCTO");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}