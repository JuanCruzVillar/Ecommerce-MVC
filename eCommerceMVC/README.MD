# 🖥️ Hardware Store - eCommerce MVC

> Sistema completo de comercio electrónico especializado en hardware de computadoras, desarrollado con ASP.NET Core 8 MVC.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet)](https://dotnet.microsoft.com/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-CC2927?style=flat&logo=microsoft-sql-server)](https://www.microsoft.com/sql-server)
[![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?style=flat&logo=bootstrap)](https://getbootstrap.com/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

---

## 📖 Introducción

Este es mi **primer proyecto completo**, desarrollado como parte de mi proceso de aprendizaje autodidacta. Implementa un sistema de eCommerce funcional utilizando arquitectura en capas, patrones de diseño y buenas prácticas de desarrollo.

Elegí desarrollar un eCommerce de hardware porque combina varios desafíos técnicos interesantes, y además soy un gran fan del mundo del gaming y del hardware 🎮

---

## 🎯 ¿Qué aprendí con este proyecto?

Durante el desarrollo de Hardware Store, pude aplicar y profundizar en:

- ✅ **Arquitectura en capas** (Presentation, Services, Repositories, Data)
- ✅ **Entity Framework Core** con Code First y migraciones
- ✅ **Autenticación y autorización** con roles (Admin/Cliente)
- ✅ **Manejo de sesiones** y cookies
- ✅ **AJAX** y peticiones asíncronas
- ✅ **Generación de reportes PDF** con QuestPDF
- ✅ **Validación de datos** en cliente y servidor
- ✅ **Manejo de errores** y logging
- ✅ **Patrones Repository** y Dependency Injection
- ✅ **Procesamiento de imágenes** con ImageSharp

---

## 🛠️ Tecnologías Utilizadas

### Backend
- **ASP.NET Core 8 MVC** - Framework web principal
- **Entity Framework Core 8** - ORM para acceso a datos
- **SQL Server 2022** - Base de datos relacional
- **Repository Pattern** - Capa de acceso a datos
- **Service Layer** - Lógica de negocio
- **Dependency Injection** - Inversión de control

### Frontend
- **Razor Views** - Motor de plantillas
- **Bootstrap 5.3** - Framework CSS
- **jQuery** - Manipulación DOM y AJAX
- **SweetAlert2** - Notificaciones elegantes
- **Bootstrap Icons** - Iconografía

### Librerías Adicionales
- **QuestPDF** - Generación de reportes PDF
- **SixLabors.ImageSharp** - Procesamiento de imágenes
- **ASP.NET Core Identity PasswordHasher** - Seguridad de contraseñas

---

## ⚡ Características Principales

### 🔧 Arma tu PC

Sistema interactivo paso a paso para configurar una PC personalizada segun presupuesto y necesidades del cliente:

- ✅ Selección guiada de componentes (CPU, Motherboard, RAM, GPU, etc.)
- ✅ **Validación automática de compatibilidad** entre procesador y motherboard
- ✅ **Guardado de configuraciones** personalizadas por usuario
- ✅ Resumen con precio total y specs completas
- ✅ Agregar configuración completa al carrito

### 🛒 E-Commerce Core

**Para Clientes:**
- 📦 Catálogo con **filtros avanzados** (categoría, marca, precio, búsqueda en tiempo real)
- 🔍 **Buscador con sugerencias** en tiempo real (funciona en todas las páginas)
- 🛍️ Carrito de compras **persistente** (localStorage + base de datos)
- 💳 Checkout completo con múltiples direcciones de envío
- 🎫 Sistema de **cupones de descuento** (fijo y porcentual)
- 📊 Estados de pedido en tiempo real (Pendiente → Procesando → Enviado → Entregado)
- 📜 Historial completo de compras con detalles
- 🏠 Gestión de direcciones de envío

### 👨‍💼 Panel de Administración

**Dashboard Analítico:**
- 📈 Métricas de ventas con gráficos interactivos
- 💰 Ingresos totales, ventas del mes y productos más vendidos
- 📊 Visualización de datos con Chart.js

**Gestión Completa:**
- ✏️ **CRUD completo**: Productos, Categorías, Marcas, Usuarios
- 🖼️ **Gestión de múltiples imágenes** por producto con galería
- 📋 **Especificaciones técnicas** personalizables
- 🏷️ Categorías jerárquicas (padre-hijo)
- 📄 **Exportación de reportes** de ventas a PDF (QuestPDF)
- 📦 Control de inventario y stock

### 🔐 Seguridad

- 🔑 Autenticación con **Cookie Authentication**
- 👥 Sistema de **roles**: Admin y Cliente
- 🔒 Contraseñas **hasheadas** con ASP.NET Core Identity PasswordHasher
- 🚪 **Separación de áreas** (Admin/Negocio) con autorización
- 🛡️ Validación de datos en cliente y servidor
- 🔄 Manejo seguro de sesiones

---

## 👥 Funcionalidad según Rol

### 🛒 CLIENTE

| Funcionalidad | Descripción |
|--------------|-------------|
| 📝 Registro y Login | Sistema completo de autenticación |
| 🔍 Catálogo | Navegación con filtros avanzados |
| 🛍️ Carrito | Agregar/quitar productos, persistencia |
| 🔧 Arma tu PC | Configurador interactivo paso a paso |
| 💳 Checkout | Proceso de compra con múltiples direcciones |
| 🎫 Cupones | Aplicar descuentos en el checkout |
| 📜 Historial | Ver todas las compras realizadas |
| 🏠 Direcciones | Gestionar direcciones de envío |
| 📦 Seguimiento | Ver estado de pedidos en tiempo real |

### 👨‍💼 ADMINISTRADOR

| Funcionalidad | Descripción |
|--------------|-------------|
| 📊 Dashboard | Métricas de ventas y gráficos |
| 📦 Productos | CRUD completo con imágenes y specs |
| 🖼️ Galería | Gestión de múltiples imágenes |
| 🏷️ Categorías | Gestión jerárquica de categorías |
| 🏢 Marcas | CRUD de marcas de productos |
| 👤 Usuarios | Gestión de clientes y admins |
| 💰 Ventas | Visualización y reportes |
| 📄 Reportes PDF | Exportación de ventas |
| 📦 Inventario | Control de stock en tiempo real |

---

## 🏗️ Arquitectura del Proyecto

```
┌─────────────────────────────────────────────────────────┐
│                   PRESENTATION LAYER                     │
│  ┌─────────────┐              ┌─────────────┐           │
│  │  Admin Area │              │ Negocio Area│           │
│  │ Controllers │              │ Controllers │           │
│  │   + Views   │              │   + Views   │           │
│  └──────┬──────┘              └──────┬──────┘           │
└─────────┼─────────────────────────────┼─────────────────┘
          │                             │
          └──────────────┬──────────────┘
                         │
          ┌──────────────▼──────────────┐
          │      SERVICE LAYER          │
          │  ┌──────────────────────┐   │
          │  │  Logica de negocio   │   │
          │  │  - ProductoService   │   │
          │  │  - CarritoService    │   │
          │  │  - CheckoutService   │   │
          │  │  - ArmaTuPcService   │   │
          │  └──────────┬───────────┘   │
          └─────────────┼───────────────┘
                        │
          ┌─────────────▼───────────────┐
          │    REPOSITORY LAYER         │
          │  ┌──────────────────────┐   │
          │  │  Acceso a datos      │   │
          │  │  - Repository<T>     │   │
          │  │  - UnitOfWork        │   │
          │  └──────────┬───────────┘   │
          └─────────────┼───────────────┘
                        │
          ┌─────────────▼───────────────┐
          │       DATA LAYER            │
          │  ┌──────────────────────┐   │
          │  │  Entity Framework    │   │
          │  │  DbContext + Models  │   │
          │  └──────────┬───────────┘   │
          └─────────────┼───────────────┘
                        │
                        ▼
                  ┌──────────┐
                  │SQL Server│
                  └──────────┘
```

---

## 📊 Modelo de Datos (Simplificado)

```
Cliente ──┬──→ Usuario
          ├──→ Carrito ───→ Producto
          ├──→ DireccionEnvio
          └──→ Venta ──┬──→ DetalleVenta ──→ Producto
                       ├──→ EstadoPedido
                       ├──→ MetodoPago
                       ├──→ Cupon
                       └──→ HistorialPedido

Producto ──┬──→ Categoria (jerárquica)
           ├──→ Marca
           ├──→ ProductoImagen
           └──→ ProductoEspecificacion

ConfiguracionPc ──→ ConfiguracionPcDetalle ──→ Producto
```

---

## 🖼️ Capturas de Pantalla

### 🛒 Área de Negocio (Cliente)

#### Catálogo Principal
![Catálogo](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/catalogomvc.png?raw=true)

#### Carrito de Compras
![Carrito](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/carritomvc.png?raw=true)

#### Arma tu PC
![ArmaTuPC](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/armatupcelegirmvc.png?raw=true)

#### Mi Perfil
![MiPerfil](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/miperfilmvc.png?raw=true)

#### Historial de Compras
![MisCompras](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/detallecompramvc.png?raw=true)

#### Direcciones de Envío
![MisDirecciones](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/misdireccionesmvc.png?raw=true)

### 👨‍💼 Panel de Administración

#### Dashboard
![Dashboard](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/dashboardmvc.png?raw=true)

#### Gestión de Usuarios
![Usuarios](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/usuariosmvc.png?raw=true)

#### Gestión de Categorías
![Categorias](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/categoriasmvc.png?raw=true)

#### Gestión de Marcas
![Marcas](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/marcasmvc.png?raw=true)

#### Gestión de Productos
![Productos](https://github.com/JuanCruzVillar/Ecommerce-MVC/blob/main/eCommerceMVC/Screenshots/productosmvc.png?raw=true)

---

## 🚀 Instalación y Configuración

### Requisitos Previos

- ✅ [.NET 8 SDK](https://dotnet.microsoft.com/download)
- ✅ [SQL Server 2019+](https://www.microsoft.com/sql-server) o SQL Server Express
- ✅ [Visual Studio 2022](https://visualstudio.microsoft.com/) (recomendado) o VS Code
- ✅ [SQL Server Management Studio (SSMS)](https://docs.microsoft.com/sql/ssms/download-sql-server-management-studio-ssms) (opcional)

### Pasos de Instalación

#### 1️⃣ Clonar el Repositorio

```bash
git clone https://github.com/JuanCruzVillar/Ecommerce-MVC.git
cd Ecommerce-MVC/eCommerceMVC
```

#### 2️⃣ Configurar la Cadena de Conexión

Edita el archivo `appsettings.json` y actualiza la cadena de conexión:

```json
{
  "ConnectionStrings": {
    "EcommerceContext": "Server=TU_SERVIDOR;Database=DBECOMMERCE;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Ejemplos comunes:**
- SQL Server local: `Server=localhost;Database=DBECOMMERCE;Trusted_Connection=True;TrustServerCertificate=True;`
- SQL Server Express: `Server=.\\SQLEXPRESS;Database=DBECOMMERCE;Trusted_Connection=True;TrustServerCertificate=True;`
- Con autenticación SQL: `Server=localhost;Database=DBECOMMERCE;User Id=sa;Password=TuPassword;TrustServerCertificate=True;`

#### 3️⃣ Restaurar Dependencias

```bash
dotnet restore
```

#### 4️⃣ Aplicar Migraciones

```bash
dotnet ef database update
```

Si no tienes las herramientas de EF Core instaladas:
```bash
dotnet tool install --global dotnet-ef
```

#### 5️⃣ Ejecutar el Proyecto

**Desde la terminal:**
```bash
dotnet run
```

**Desde Visual Studio:**
- Presiona `F5` o click en el botón ▶️ Play

#### 6️⃣ Acceder a la Aplicación

- 🌐 **Cliente**: `https://localhost:7XXX/Negocio/Catalogo`
- 👨‍💼 **Admin**: `https://localhost:7XXX/Admin/Auth/Login`

*(El puerto puede variar, revisa la consola)*

---

## 🔑 Credenciales de Prueba

### Administrador
```
Email: testadmin@gmail.com
Password: 123456
```

### Cliente
```
Email: test@correo.com
Password: 123456
```


---

## 📁 Estructura de Carpetas

```
eCommerceMVC/
├── Areas/
│   ├── Admin/
│   │   ├── Controllers/
│   │   │   ├── AuthController.cs
│   │   │   ├── HomeController.cs (Dashboard)
│   │   │   ├── ProductosController.cs
│   │   │   ├── CategoriasController.cs
│   │   │   └── UsuariosController.cs
│   │   └── Views/
│   └── Negocio/
│       ├── Controllers/
│       │   ├── CatalogoController.cs
│       │   ├── CarritoController.cs
│       │   ├── CheckoutController.cs
│       │   ├── ArmatuPcController.cs
│       │   └── PerfilController.cs
│       └── Views/
├── eCommerce.Services/
│   ├── Interfaces/
│   └── Implementations/
├── eCommerce.Repositories/
│   ├── Interfaces/
│   └── Implementations/
├── eCommerce.Data/
│   └── EcommerceDbContext.cs
├── eCommerce.Entities/
│   ├── Models/
│   └── ViewModels/
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
└── Program.cs
```

---

## 🎓 Controladores Importantes

### Área Admin

| Controlador | Responsabilidad |
|------------|----------------|
| `HomeController` | Dashboard con métricas, gráficos y exportación PDF |
| `ProductosController` | CRUD de productos, imágenes y especificaciones |
| `CategoriasController` | Gestión de categorías jerárquicas |
| `UsuariosController` | Administración de usuarios y roles |

### Área Negocio

| Controlador | Responsabilidad |
|------------|----------------|
| `CatalogoController` | Listado, búsqueda y detalle de productos |
| `CarritoController` | Gestión del carrito de compras |
| `CheckoutController` | Proceso completo de checkout y pago |
| `ArmatuPcController` | Sistema paso a paso de configuración de PC |
| `PerfilController` | Perfil, historial de compras y direcciones |

---


### Error 404 en Imágenes

**Solución**:
- Las imágenes de productos deben estar en `wwwroot/images/productos/`
- Verifica los permisos de escritura en la carpeta

---



## 🤝 Notas

Este es un proyecto de aprendizaje para poder adquirir experiencia y conocimientos como programador, pero estoy **abierto a feedback y sugerencias**.


---

## 📝 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo [LICENSE](LICENSE) para más detalles.

---

## 📬 Contacto

**Juan Cruz Villar**

- 💼 LinkedIn: [linkedin.com/in/juancruzvillar](https://www.linkedin.com/in/juancruzvillar/)
- 📧 Email: juuancvillar@gmail.com
- 🐙 GitHub: [@JuanCruzVillar](https://github.com/JuanCruzVillar)

---

## 💝 Agradecimientos

Gracias por tomarte el tiempo de revisar mi proyecto. Este eCommerce representa meses de aprendizaje, investigación y desarrollo. 

Si te gustó o te sirvió de referencia, **una ⭐ en el repo me ayudaria un monton** 😊

---

<div align="center">



[⬆ Volver arriba](#️-hardware-store---ecommerce-mvc)

</div>