using eCommerce.Areas.Negocio.Controllers;
using eCommerce.Entities.ViewModels;
using eCommerce.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace eCommerceMVC.Areas.Negocio.Controllers
{
    [Area("Negocio")]
    public class ArmatuPcController : BaseNegocioController
    {
        private readonly IArmatuPcService _armatuPcService;
        private readonly ICarritoService _carritoService;

        public ArmatuPcController(IArmatuPcService armatuPcService, ICarritoService carritoService)
        {
            _armatuPcService = armatuPcService;
            _carritoService = carritoService;
        }

        // PASO 1: Seleccionar marca (AMD o Intel)
        public IActionResult Index()
        {
            var model = new ArmaPcViewModel
            {
                Paso = 1,
                Titulo = "¡Arma tu PC!",
                Descripcion = "Selecciona la marca de procesador que prefieres"
            };

            return View(model);
        }

        // PASO 2: Seleccionar procesador
        [HttpPost]
        public async Task<IActionResult> SeleccionarMarca(string marca)
        {
            if (string.IsNullOrEmpty(marca) || (marca != "AMD" && marca != "Intel"))
            {
                TempData["Error"] = "Selecciona una marca válida";
                return RedirectToAction("Index");
            }

            var procesadores = await _armatuPcService.ObtenerProcesadoresAsync(marca);

            if (!procesadores.Any())
            {
                TempData["Error"] = $"No hay procesadores disponibles para {marca}";
                return RedirectToAction("Index");
            }

            var model = new ArmaPcViewModel
            {
                Paso = 2,
                Titulo = "Selecciona tu Procesador",
                Descripcion = $"Procesadores {marca} disponibles",
                MarcaSeleccionada = marca,
                ProcesadoresDisponibles = procesadores
            };

            return View("Paso2", model);
        }

        // PASO 2B: Ir a seleccionar motherboard
        [HttpPost]
        public async Task<IActionResult> SeleccionarProcesador(int idProcesador, string marca)
        {
            var procesador = await _armatuPcService.ObtenerProductoDetalladoAsync(idProcesador);
            if (procesador == null)
            {
                TempData["Error"] = "Procesador no encontrado";
                return RedirectToAction("Index");
            }

            var motherboards = await _armatuPcService.ObtenerMotherboardsAsync(marca);

            var model = new ArmaPcViewModel
            {
                Paso = 2,
                Titulo = "Selecciona tu Motherboard",
                Descripcion = "Elige el motherboard compatible con tu procesador",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                ProcesadorSeleccionadoInfo = new ComponenteSeleccionadoDTO
                {
                    IdProducto = procesador.IdProducto,
                    Nombre = procesador.Nombre,
                    Marca = procesador.Marca,
                    Tipo = "Procesador",
                    PrecioUnitario = procesador.Precio,
                    Cantidad = 1,
                    RutaImagen = procesador.RutaImagen
                },
                MotherboardsDisponibles = motherboards,
                Subtotal = procesador.Precio,
                Total = procesador.Precio,
                TotalComponentes = 1
            };

            return View("Paso2B", model);
        }

        // PASO 3: Seleccionar RAM
        [HttpPost]
        public async Task<IActionResult> SeleccionarMotherboard(int idProcesador, int idMotherboard, string marca)
        {
            var procesador = await _armatuPcService.ObtenerProductoDetalladoAsync(idProcesador);
            var motherboard = await _armatuPcService.ObtenerProductoDetalladoAsync(idMotherboard);

            if (procesador == null || motherboard == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Index");
            }

            var rams = await _armatuPcService.ObtenerRamsAsync();

            var model = new ArmaPcViewModel
            {
                Paso = 3,
                Titulo = "Selecciona tu Memoria RAM",
                Descripcion = "Puedes seleccionar 1 o más módulos de RAM",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                ProcesadorSeleccionadoInfo = MapearAComponente(procesador, "Procesador"),
                MotherboardSeleccionadoInfo = MapearAComponente(motherboard, "Motherboard"),
                RamsDisponibles = rams,
                Subtotal = procesador.Precio + motherboard.Precio,
                Total = procesador.Precio + motherboard.Precio,
                TotalComponentes = 2
            };

            return View("Paso3", model);
        }

        // PASO 4: Seleccionar GPU
        [HttpPost]
        public async Task<IActionResult> SeleccionarRam(int idProcesador, int idMotherboard, string marca,
            [FromForm] List<int> idsRam)
        {
            var procesador = await _armatuPcService.ObtenerProductoDetalladoAsync(idProcesador);
            var motherboard = await _armatuPcService.ObtenerProductoDetalladoAsync(idMotherboard);

            if (procesador == null || motherboard == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Index");
            }

            var gpus = await _armatuPcService.ObtenerGpusAsync();
            var ramSeleccionados = new List<ComponenteSeleccionadoDTO>();
            decimal totalRam = 0;

            if (idsRam != null && idsRam.Any())
            {
                foreach (var idRam in idsRam.Distinct())
                {
                    var ram = await _armatuPcService.ObtenerProductoDetalladoAsync(idRam);
                    if (ram != null)
                    {
                        ramSeleccionados.Add(MapearAComponente(ram, "Memoria RAM"));
                        totalRam += ram.Precio;
                    }
                }
            }

            var model = new ArmaPcViewModel
            {
                Paso = 4,
                Titulo = "Selecciona tu Tarjeta Gráfica",
                Descripcion = "Elige una tarjeta gráfica (opcional)",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                IdsRamSeleccionados = idsRam ?? new List<int>(),
                ProcesadorSeleccionadoInfo = MapearAComponente(procesador, "Procesador"),
                MotherboardSeleccionadoInfo = MapearAComponente(motherboard, "Motherboard"),
                ComponentesSeleccionados = ramSeleccionados,
                GpusDisponibles = gpus,
                Subtotal = procesador.Precio + motherboard.Precio + totalRam,
                Total = procesador.Precio + motherboard.Precio + totalRam,
                TotalComponentes = 2 + ramSeleccionados.Count
            };

            return View("Paso4", model);
        }

        // PASO 5: Seleccionar Almacenamiento
        [HttpPost]
        public async Task<IActionResult> SeleccionarGpu(int idProcesador, int idMotherboard, string marca,
            [FromForm] List<int> idsRam, int? idGpu)
        {
            var procesador = await _armatuPcService.ObtenerProductoDetalladoAsync(idProcesador);
            var motherboard = await _armatuPcService.ObtenerProductoDetalladoAsync(idMotherboard);

            if (procesador == null || motherboard == null)
            {
                TempData["Error"] = "Producto no encontrado";
                return RedirectToAction("Index");
            }

            var almacenamiento = await _armatuPcService.ObtenerAlmacenamientoAsync();
            var componentesSeleccionados = await ObtenerComponentesSeleccionadosAsync(
                idProcesador, idMotherboard, idsRam, idGpu);

            var total = componentesSeleccionados.Sum(c => c.Subtotal);

            var model = new ArmaPcViewModel
            {
                Paso = 5,
                Titulo = "Selecciona tu Almacenamiento",
                Descripcion = "SSD o HDD (puedes seleccionar varios)",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                IdsRamSeleccionados = idsRam ?? new List<int>(),
                IdGpuSeleccionada = idGpu,
                ComponentesSeleccionados = componentesSeleccionados,
                AlmacenamientoDisponible = almacenamiento,
                Subtotal = total,
                Total = total,
                TotalComponentes = componentesSeleccionados.Count
            };

            return View("Paso5", model);
        }

        // PASO 6: Seleccionar Fuente
        [HttpPost]
        public async Task<IActionResult> SeleccionarAlmacenamiento(int idProcesador, int idMotherboard, string marca,
            [FromForm] List<int> idsRam, int? idGpu, [FromForm] List<int> idsAlmacenamiento)
        {
            var psu = await _armatuPcService.ObtenerPsusAsync();
            var componentesSeleccionados = await ObtenerComponentesSeleccionadosAsync(
                idProcesador, idMotherboard, idsRam, idGpu, idsAlmacenamiento);

            var total = componentesSeleccionados.Sum(c => c.Subtotal);

            var model = new ArmaPcViewModel
            {
                Paso = 6,
                Titulo = "Selecciona tu Fuente de Poder",
                Descripcion = "Elige una fuente de poder adecuada",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                IdsRamSeleccionados = idsRam ?? new List<int>(),
                IdGpuSeleccionada = idGpu,
                IdsAlmacenamientoSeleccionados = idsAlmacenamiento ?? new List<int>(),
                ComponentesSeleccionados = componentesSeleccionados,
                PsusDisponibles = psu,
                Subtotal = total,
                Total = total,
                TotalComponentes = componentesSeleccionados.Count
            };

            return View("Paso6", model);
        }

        // PASO 7: Seleccionar Cooler (Opcional)
        [HttpPost]
        public async Task<IActionResult> SeleccionarPsu(int idProcesador, int idMotherboard, string marca,
            [FromForm] List<int> idsRam, int? idGpu, [FromForm] List<int> idsAlmacenamiento, int idPsu)
        {
            var coolers = await _armatuPcService.ObtenerCoolersAsync();
            var componentesSeleccionados = await ObtenerComponentesSeleccionadosAsync(
                idProcesador, idMotherboard, idsRam, idGpu, idsAlmacenamiento, idPsu);

            var total = componentesSeleccionados.Sum(c => c.Subtotal);

            var model = new ArmaPcViewModel
            {
                Paso = 7,
                Titulo = "Selecciona un Cooler (Opcional)",
                Descripcion = "Mejora la refrigeración de tu PC",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                IdsRamSeleccionados = idsRam ?? new List<int>(),
                IdGpuSeleccionada = idGpu,
                IdsAlmacenamientoSeleccionados = idsAlmacenamiento ?? new List<int>(),
                IdPsuSeleccionada = idPsu,
                ComponentesSeleccionados = componentesSeleccionados,
                CoolersDisponibles = coolers,
                Subtotal = total,
                Total = total,
                TotalComponentes = componentesSeleccionados.Count
            };

            return View("Paso7", model);
        }

        // PASO 8: Resumen y confirmación
        [HttpPost]
        public async Task<IActionResult> SeleccionarCooler(int idProcesador, int idMotherboard, string marca,
            [FromForm] List<int> idsRam, int? idGpu, [FromForm] List<int> idsAlmacenamiento,
            int idPsu, [FromForm] List<int> idsCooler)
        {
            var componentesSeleccionados = await ObtenerComponentesSeleccionadosAsync(
                idProcesador, idMotherboard, idsRam, idGpu, idsAlmacenamiento, idPsu, idsCooler);

            var total = componentesSeleccionados.Sum(c => c.Subtotal);

            var model = new ArmaPcViewModel
            {
                Paso = 8,
                Titulo = "Resumen de tu PC",
                Descripcion = "Revisa tu configuración antes de agregar al carrito",
                MarcaSeleccionada = marca,
                IdProcesadorSeleccionado = idProcesador,
                IdMotherboardSeleccionado = idMotherboard,
                IdsRamSeleccionados = idsRam ?? new List<int>(),
                IdGpuSeleccionada = idGpu,
                IdsAlmacenamientoSeleccionados = idsAlmacenamiento ?? new List<int>(),
                IdPsuSeleccionada = idPsu,
                IdsCoolerSeleccionados = idsCooler ?? new List<int>(),
                ComponentesSeleccionados = componentesSeleccionados,
                Subtotal = total,
                Total = total,
                TotalComponentes = componentesSeleccionados.Count
            };

            return View("Paso8", model);
        }

        // Agregar todos los componentes al carrito
        [HttpPost]
        public async Task<IActionResult> AgregarAlCarrito(int idProcesador, int idMotherboard,
            [FromForm] List<int> idsRam, int? idGpu, [FromForm] List<int> idsAlmacenamiento,
            int idPsu, [FromForm] List<int> idsCooler)
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                TempData["Error"] = "Debes iniciar sesión para agregar productos al carrito";
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                // Agregar cada componente al carrito
                await _carritoService.AgregarProductoAsync(clienteId, idProcesador, 1);
                await _carritoService.AgregarProductoAsync(clienteId, idMotherboard, 1);

                if (idsRam != null)
                    foreach (var idRam in idsRam.Distinct())
                        await _carritoService.AgregarProductoAsync(clienteId, idRam, 1);

                if (idGpu.HasValue)
                    await _carritoService.AgregarProductoAsync(clienteId, idGpu.Value, 1);

                if (idsAlmacenamiento != null)
                    foreach (var idAlm in idsAlmacenamiento.Distinct())
                        await _carritoService.AgregarProductoAsync(clienteId, idAlm, 1);

                await _carritoService.AgregarProductoAsync(clienteId, idPsu, 1);

                if (idsCooler != null)
                    foreach (var idCooler in idsCooler.Distinct())
                        await _carritoService.AgregarProductoAsync(clienteId, idCooler, 1);

                TempData["Success"] = "¡PC agregado al carrito exitosamente!";
                return RedirectToAction("Index", "Carrito");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al agregar el PC al carrito: " + ex.Message;
                return RedirectToAction("Index");
            }
        }

        // Guardar configuración
        [HttpPost]
        public async Task<IActionResult> GuardarConfiguracion(GuardarConfiguracionViewModel model)
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                return Json(new { success = false, message = "Debes iniciar sesión" });
            }

            if (string.IsNullOrWhiteSpace(model.Nombre))
            {
                return Json(new { success = false, message = "El nombre de la configuración es requerido" });
            }

            var resultado = await _armatuPcService.GuardarConfiguracionAsync(clienteId, model);
            return Json(new { success = resultado, message = resultado ? "Configuración guardada exitosamente" : "Error al guardar configuración" });
        }

        // Obtener configuraciones guardadas
        public async Task<IActionResult> MisConfiguraciones()
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                TempData["Error"] = "Debes iniciar sesión";
                return RedirectToAction("Login", "Auth");
            }

            var configuraciones = await _armatuPcService.ObtenerConfiguracionesGuardasAsync(clienteId);
            return View(configuraciones);
        }

        // Eliminar configuración guardada
        [HttpPost]
        public async Task<IActionResult> EliminarConfiguracion(int id)
        {
            var clienteId = GetClienteId();
            if (clienteId == 0)
            {
                return Json(new { success = false, message = "No autorizado" });
            }

            var resultado = await _armatuPcService.EliminarConfiguracionAsync(id, clienteId);
            return Json(new { success = resultado });
        }

        // Métodos privados
        private ComponenteSeleccionadoDTO MapearAComponente(ProductoDTO producto, string tipo)
        {
            return new ComponenteSeleccionadoDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Tipo = tipo,
                Marca = producto.Marca,
                PrecioUnitario = producto.Precio,
                Cantidad = 1,
                RutaImagen = producto.RutaImagen
            };
        }

        private async Task<List<ComponenteSeleccionadoDTO>> ObtenerComponentesSeleccionadosAsync(
            int idProcesador, int idMotherboard, List<int> idsRam, int? idGpu = null,
            List<int> idsAlmacenamiento = null, int? idPsu = null, List<int> idsCooler = null)
        {
            var componentes = new List<ComponenteSeleccionadoDTO>();

            var procesador = await _armatuPcService.ObtenerProductoDetalladoAsync(idProcesador);
            if (procesador != null)
                componentes.Add(MapearAComponente(procesador, "Procesador"));

            var motherboard = await _armatuPcService.ObtenerProductoDetalladoAsync(idMotherboard);
            if (motherboard != null)
                componentes.Add(MapearAComponente(motherboard, "Motherboard"));

            if (idsRam != null)
                foreach (var idRam in idsRam.Distinct())
                {
                    var ram = await _armatuPcService.ObtenerProductoDetalladoAsync(idRam);
                    if (ram != null)
                        componentes.Add(MapearAComponente(ram, "Memoria RAM"));
                }

            if (idGpu.HasValue)
            {
                var gpu = await _armatuPcService.ObtenerProductoDetalladoAsync(idGpu.Value);
                if (gpu != null)
                    componentes.Add(MapearAComponente(gpu, "Tarjeta Gráfica"));
            }

            if (idsAlmacenamiento != null)
                foreach (var idAlm in idsAlmacenamiento.Distinct())
                {
                    var almacenamiento = await _armatuPcService.ObtenerProductoDetalladoAsync(idAlm);
                    if (almacenamiento != null)
                        componentes.Add(MapearAComponente(almacenamiento, "Almacenamiento"));
                }

            if (idPsu.HasValue)
            {
                var psu = await _armatuPcService.ObtenerProductoDetalladoAsync(idPsu.Value);
                if (psu != null)
                    componentes.Add(MapearAComponente(psu, "Fuente de Poder"));
            }

            if (idsCooler != null)
                foreach (var idCooler in idsCooler.Distinct())
                {
                    var cooler = await _armatuPcService.ObtenerProductoDetalladoAsync(idCooler);
                    if (cooler != null)
                        componentes.Add(MapearAComponente(cooler, "Cooler"));
                }

            return componentes;


        }
    }
}