using dulceSabor_caja_ListadodeMesas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace dulceSabor_caja_ListadodeMesas.Controllers
{
    public class listadoMesasController : Controller
    {
        private readonly dulceSaborDbContext _dulceSaborDbContext;

        public listadoMesasController(dulceSaborDbContext dulceSaborDbContext)
        {
            _dulceSaborDbContext = dulceSaborDbContext;
        }

        //ESTE ES EL MÉTODO PARA CONTROLAR LA VISTA INDEX DEL CONTROLADOR DE listadoMesasController

        public async Task<IActionResult> Index()
        {
            //Aqui se obtienen los datos de marcas de la base para mostrarlo en la tabla
            var listaDeMesas = (from m in _dulceSaborDbContext.mesas
                                join e in _dulceSaborDbContext.estados on m.id_estado equals e.id_estado
                                join c in _dulceSaborDbContext.cuenta on m.id_mesa equals c.Id_mesa
                                where e.id_estado == 12 && c.Estado_cuenta.Equals("Cerrada")
                                select new
                                 {
                                     id_mesa = m.id_mesa,
                                     cantidad_personas = m.cantidad_personas,
                                     nombre_estado = e.nombre,
                                     nombre_mesa = m.nombre_mesa
                                 }).Distinct().ToList();
            ViewData["listadoDeMesas"] = listaDeMesas;

            return View(await _dulceSaborDbContext.mesas.ToListAsync());
        }

        //ESTE ES EL MÉTODO PARA CONTROLAR LA VISTA lista_cuentas DEL CONTROLADOR DE listadoMesasController

        public async Task<IActionResult> listaCuentas(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var listadoMesas = await _dulceSaborDbContext.mesas
                .FirstOrDefaultAsync(m => m.id_mesa == id);

            if (listadoMesas == null)
            {
                return NotFound();
            }

            //SE CREA UNA LISTA CON LAS CUENTAS ABIERTAS EN LA MESA SELECCIONADA
            
            var cuentas = (from m in _dulceSaborDbContext.mesas
                                join c in _dulceSaborDbContext.cuenta on m.id_mesa equals c.Id_mesa
                                where m.id_mesa == id && c.Estado_cuenta.Equals("Cerrada")
                                select new
                                {
                                    id_cuenta = c.Id_cuenta,
                                    id_mesa = c.Id_mesa,
                                    nombre_cliente = c.Nombre,
                                    cantidad_personas = c.Cantidad_Personas,
                                    estado_Cuenta = c.Estado_cuenta,
                                    hora_Fecha = c.Fecha_Hora
                                }).ToList();
            ViewData["lista_cuentas"] = cuentas;

            //SE CREA UNA LISTA CON EL DETALLE DEL PEDIDO EN LA MESA SELECCIONADA

            var detalle_Pedido = (from dp in _dulceSaborDbContext.Detalle_Pedido
                                  join im in _dulceSaborDbContext.items_menu on dp.Id_plato equals im.id_item_menu
                                  join c in _dulceSaborDbContext.cuenta on dp.Id_cuenta equals c.Id_cuenta
                                  join m in _dulceSaborDbContext.mesas on c.Id_mesa equals m.id_mesa
                           select new
                           {
                               id_detP = dp.Id_DetalleCuenta,
                               id_cuenta = c.Id_cuenta,
                               id_plato = dp.Id_plato,
                               nombre_plato = im.nombre,
                               cantidad = dp.Cantidad,
                               estado_detPedido = dp.Estado,
                               tipo_plato = dp.Tipo_Plato,
                               precio = dp.Precio
                           }).ToList();
            ViewData["lista_detPedidos"] = detalle_Pedido;

            return View(listadoMesas);
        }

        //FIN DEL MÉTODO DE lista_cuentas

        //ESTE ES EL MÉTODO PARA CONTROLAR LA VISTA detalleFactura DEL CONTROLADOR DE listadoMesasController
        public async Task<IActionResult> GenerarFactura(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var cuenta = await _dulceSaborDbContext.cuenta
                .FirstOrDefaultAsync(c => c.Id_cuenta == id);


            if (cuenta == null)
            {
                return NotFound();
            }

            var detalle_Pedido = (from dp in _dulceSaborDbContext.Detalle_Pedido
                                  join im in _dulceSaborDbContext.items_menu on dp.Id_plato equals im.id_item_menu
                                  join c in _dulceSaborDbContext.cuenta on dp.Id_cuenta equals c.Id_cuenta
                                  join m in _dulceSaborDbContext.mesas on c.Id_mesa equals m.id_mesa
                                  where dp.Estado.Equals("Entregado")
                                  select new
                                  {
                                      id_detP = dp.Id_DetalleCuenta,
                                      id_cuenta = c.Id_cuenta,
                                      id_plato = dp.Id_plato,
                                      nombre_plato = im.nombre,
                                      cantidad = dp.Cantidad,
                                      estado_detPedido = dp.Estado,
                                      tipo_plato = dp.Tipo_Plato,
                                      precio = dp.Precio
                                  }).ToList();
            ViewData["lista_detPedidos"] = detalle_Pedido;

            var clientesDatos = await _dulceSaborDbContext.clientes.ToListAsync();
            ViewData["listaClientes"] = clientesDatos;

            return View(cuenta);

        }

        //FIN DEL MÉTODO DE detalleFactura

        //ESTE ES EL MÉTODO que se ejecuta cada vez que le den a un botón en el Detalle de los pedidos

        [HttpPost]
        public async Task<IActionResult> Facturar(string dui_existente, string dui_nuevo, string nombre, string apellido, string correo, string metodo_pago, int cantidad, decimal totalPagar, string idsPedidosSeleccionados)
        {
            DateTime fechaActual = DateTime.Now;
            int? idCliente = null;
            var ids_detP = idsPedidosSeleccionados.Split(',').Select(int.Parse).ToList();
            var cuenta_id = await _dulceSaborDbContext.Detalle_Pedido
            .FirstOrDefaultAsync(dp => dp.Id_DetalleCuenta == ids_detP.First());


            // Verificar si dui_existente está vacío o nulo
            if (string.IsNullOrEmpty(dui_existente))
            {
                // Crear un nuevo cliente con los datos proporcionados
                var nuevoCliente = new clientes
                {
                    nombre = nombre,
                    apellido = apellido,
                    correo = correo,
                    dui = dui_nuevo // Asignar el nuevo DUI proporcionado
                };

                // Agregar el nuevo cliente a la base de datos
                _dulceSaborDbContext.clientes.Add(nuevoCliente);
                await _dulceSaborDbContext.SaveChangesAsync(); // Guardar los cambios para obtener el ID generado

                // Obtener el ID del cliente recién creado
                var clienteExistente = await _dulceSaborDbContext.clientes.FirstOrDefaultAsync(cl => cl.dui == nuevoCliente.dui);
                idCliente = clienteExistente.id_cliente;
            }
            else
            {
                // Buscar el cliente existente por su DUI
                var clienteExistente = await _dulceSaborDbContext.clientes.FirstOrDefaultAsync(cl => cl.dui == dui_existente);

                // Verificar si se encontró un cliente con el DUI proporcionado
                if (clienteExistente != null)
                {
                    // Obtener el ID del cliente existente
                    idCliente = clienteExistente.id_cliente;
                }
                else
                {
                    // Manejar el caso cuando el DUI no existe en la base de datos
                    ModelState.AddModelError("dui_existente", "El DUI proporcionado no existe en la base de datos.");
                    return RedirectToAction("ErrorGenerarFactura", cuenta_id.Id_cuenta);
                }
                // Si no se encuentra un cliente con el DUI existente, puedes manejarlo según tus necesidades
                // Por ejemplo, puedes lanzar una excepción o redirigir a una página de error
            }

            // Crear un nuevo encabezado de factura
            var nuevoEncabezadoFac = new encabezado_fac
            {
                id_pedido = cuenta_id.Id_cuenta,
                id_cliente = idCliente,
                fecha_cobro = fechaActual,
                total_cobrado = totalPagar,
                metodo_pago = metodo_pago
            };

            // Agregar el nuevo encabezado de factura a la base de datos
            _dulceSaborDbContext.Add(nuevoEncabezadoFac);
            await _dulceSaborDbContext.SaveChangesAsync();
            var lastEncFactura = _dulceSaborDbContext.encabezado_fac.OrderByDescending(lef => lef.fecha_cobro).FirstOrDefault();

            if (!string.IsNullOrEmpty(idsPedidosSeleccionados))
            {
                // Dividir la cadena en una lista de IDs
                var ids = idsPedidosSeleccionados.Split(',').Select(int.Parse).ToList();

                // Aquí puedes realizar la inserción en la base de datos para cada ID
                foreach (var id_detPedido in ids)
                {
                    var detPedidoCompleto = await _dulceSaborDbContext.Detalle_Pedido
                    .FirstOrDefaultAsync(dp => dp.Id_DetalleCuenta == id_detPedido);

                    var detalleFacturaModelo = new detalle_fac
                    {
                        id_factura = lastEncFactura.id_factura,
                        id_detallepedido = id_detPedido,
                        precio = detPedidoCompleto.Precio,
                        total_plato = (detPedidoCompleto.Precio * detPedidoCompleto.Cantidad),
                        cantidad = detPedidoCompleto.Cantidad,
                    };
                    _dulceSaborDbContext.Add(detalleFacturaModelo);
                    await _dulceSaborDbContext.SaveChangesAsync();
                    detPedidoCompleto.Estado = "Cancelado";
                    _dulceSaborDbContext.SaveChanges();
                }
                var count = await _dulceSaborDbContext.Detalle_Pedido
                    .Where(dp => dp.Id_cuenta == cuenta_id.Id_cuenta && dp.Estado == "Entregado")
                    .CountAsync();
                if (count <= 0)
                {
                    var cuentaToUpdate = await _dulceSaborDbContext.cuenta.FindAsync(cuenta_id.Id_cuenta);
                    cuentaToUpdate.Estado_cuenta = "Finalizado";
                    await _dulceSaborDbContext.SaveChangesAsync();
                }
            }
            return RedirectToAction("facturaGenerada", new { id = lastEncFactura.id_factura });
        }


        public async Task<IActionResult> facturaGenerada(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encabezado_factura = await _dulceSaborDbContext.encabezado_fac
                .FirstOrDefaultAsync(m => m.id_factura == id);

            if (encabezado_factura == null)
            {
                return NotFound();
            }



            return View(encabezado_factura);
        }

    }
}
