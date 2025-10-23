
## Introduccion a Hardware Store


Sistema completo de comercio electrónico especializado en hardware de computadoras.

Este es mi primer proyecto, fue desarrollado como parte de mi proceso de aprendizaje en el que aun me encuentro. Implementa un sistema de eCommerce completo utilizando **ASP.NET Core 8 MVC**, con arquitectura de capas, patrones de diseño y buenas prácticas de desarrollo.

Elegí desarrollar un eCommerce de hardware porque combina varios desafíos técnicos interesantes, y ademas soy un gran fan del mundo del gaming y del hardware. 😁

## Que aprendi con este proyecto?

- Arquitectura en capas (Presentation, Services, Repositories, Data)
-  Entity Framework Core con migraciones 
-  Autenticación y autorización con roles
-  Manejo de sesiones y cookies
-  AJAX
-  Generación de reportes PDF
-  Validación de datos en cliente y servidor
-  Manejo de errores y logging
-  Patrones Repository y Dependency Injection
## Tecnologias utilizadas

**Backend**

ASP.NET Core 8 MVC - Framework web

Entity Framework Core 8 - ORM

SQL Server 2022 - Base de datos

Repositories - Acceso a datos

Services - Lógica de negocio

Dependency Injection - IoC Container

**Frontend**

Razor Views - Motor de vistas

Bootstrap 5.3 - Framework CSS

jQuery - AJAX requests

Librerías Adicionales

QuestPDF - Generación de PDF

ImageSharp - Procesamiento de imágenes

ASP.NET Core - Hash de contraseñas

## Caracteristicas del proyecto

**Arma tu PC**

Sistema interactivo paso a paso para elegir cada componente segun su compatibilidad:

Validación de compatibilidad entre procesador y motherboard

Guardado de configuraciones personalizadas

Posibilidad de armar tu propia PC segun tu presupuesto y tus necesidades.


 **E-Commerce Core**

Catálogo de productos con buscador y filtros (categoría, marca, precio, búsqueda)

Carrito de compras persistente

Checkout completo con múltiples direcciones de envío

Cupones de descuento (fijo y porcentual)

Sistema de estados de pedido (Pendiente // Procesando // Enviado // Entregado)

Historial de compras del cliente

**Panel de Administración**

Dashboard con métricas de ventas en tiempo real

CRUD completo: Productos, Categorías, Marcas, Usuarios

Gestión de múltiples imágenes por producto

Especificaciones técnicas de productos

Exportación de reportes de ventas a PDF (QuestPDF)

Gestión de inventario 

**Seguridad**

Autenticación con Cookie Authentication

Roles: Admin y Cliente

Contraseñas hasheadas con ASP.NET Core Identity PasswordHasher

Separación de áreas (Admin/Negocio)
## Funcionalidad segun rol

**CLIENTE** 🛒

 Registro y login

 Navegación en el catálogo con filtros

 Búsqueda de productos

 Agregar productos al carrito

 Arma tu PC 

 Checkout con distintas direcciones

 Aplicar cupones de descuento

 Ver historial de compras

 Gestionar direcciones de envío

 Ver detalle de pedidos


**ADMINISTRADOR** 👨‍💼


 Dashboard con métricas

 Gestión de productos (CRUD completo)

 Gestión de múltiples imágenes para la galeria en el catalogo (Negocio)

 Gestión de especificaciones técnicas

 Gestión de categorías jerárquicas

 Gestión de marcas

 Gestión de usuarios

 Visualización de ventas

 Exportación de reportes PDF

 Control de stock de productos
## Arquitectura

https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/estructuramvc.png?raw=true
## Vista previa Negocio


[Home] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/catalogomvc.png?raw=true

[Carrito] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/carritomvc.png?raw=true

[ArmaTuPC] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/armatupcelegirmvc.png?raw=true

[MiPerfil] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/miperfilmvc.png?raw=true

[MisCompras] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/detallecompramvc.png?raw=true

[MisDirecciones] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/misdireccionesmvc.png?raw=true

## Vista previa Admin

[Dashboard] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/dashboardmvc.png?raw=true

[Usuarios] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/usuariosmvc.png?raw=true

[Categorias] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/categoriasmvc.png?raw=true

[Marcas] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/marcasmvc.png?raw=true

[Productos] https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/productosmvc.png?raw=true





## Requisitos

.NET 8 SDK

SQL Server 2019+ o SQL Server Express

Visual Studio 2022 

SQL Server Management Studio (SSMS) 
## Instalacion

1️⃣ **Clonar el repositorio**
```bash
git clone https://github.com/JuanCruzVillar/Ecommerce-MVC.git
cd Ecommerce-MVC
```

2️⃣ **Configurar la cadena de conexión**

Edita `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "EcommerceContext": "Server=TU_SERVIDOR;Database=DBECOMMERCE;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

3️⃣ **Ejecutar las migraciones**
```bash
dotnet ef database update
```

4️⃣ **Ejecutar el proyecto**
```bash
dotnet run
```

O desde Visual Studio: `F5`

5️⃣ **Acceder a la aplicación**
- 🌐 Cliente: `https://localhost:7XXX/Negocio/Catalogo`
- 👨‍💼 Admin: `https://localhost:7XXX/Admin/Auth/Login`

### 🔑 Credenciales de Prueba

**Administrador:**
- Email: `testadmin@gmail.com`
- Password: `123456`

**Cliente:**
- Email: `test@correo.com`
- Password: `123456`

---

## Estructura del proyecto
````
Cliente ──┬─→ Usuario
          ├─→ Carrito ──→ Producto
          ├─→ DireccionEnvio
          └─→ Venta ──┬─→ DetalleVenta ──→ Producto
                      ├─→ EstadoPedido
                      ├─→ MetodoPago
                      ├─→ Cupon
                      └─→ HistorialPedido

Producto ──┬─→ Categoria (jerárquica)
           ├─→ Marca
           ├─→ ProductoImagen
           └─→ ProductoEspecificacion

ConfiguracionPc ──→ ConfiguracionPcDetalle ──→ Producto
````
**Informacion sobre controladores importantes**

**Area Admin:**

ProductosController: CRUD de productos e imágenes.

HomeController: Dashboard de las ventas y exportación PDF.


**Area Negocio:**

CatalogoController: Listado y búsqueda de productos para su compra.

CheckoutController: Proceso de compra con formulario para datos de envio.

ArmatuPcController: Sistema paso a paso para armado de pc segun presupuesto o necesidad.

PerfilController: Historial de compras y datos del cliente.
## Notas

Este proyecto fue desarrollado como parte de mi proceso de aprendizaje en el que aun me encuentro. Estoy abierto a **feedback** y sugerencias para mejorar el código. Si encontrás algún bug o tenés ideas, no dudes en abrir un issue o en escribirme.

---


## Autor

Juan Cruz Villar 

LinkedIn: https://www.linkedin.com/in/juancruzvillar/

Email: juuancvillar@gmail.com