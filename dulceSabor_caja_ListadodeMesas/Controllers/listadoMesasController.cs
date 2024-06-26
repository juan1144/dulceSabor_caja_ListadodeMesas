﻿using dulceSabor_caja_ListadodeMesas.Models;
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
                                where c.Id_mesa == id && c.Estado_cuenta.Equals("Cerrada")
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
                                  join c in _dulceSaborDbContext.cuenta on dp.Id_cuenta equals c.Id_cuenta
                                  join m in _dulceSaborDbContext.mesas on c.Id_mesa equals m.id_mesa
                                  join im in _dulceSaborDbContext.items_menu on dp.Id_plato equals im.id_item_menu
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
                                      precio = dp.Precio,
                                      // Si el tipo de plato es Combo, obtener información del combo
                                      combo = dp.Tipo_Plato == 'c' ?
                                          (from ic in _dulceSaborDbContext.items_combo
                                           join co in _dulceSaborDbContext.combos on ic.id_combo equals co.id_combo
                                           where ic.id_item_menu == im.id_item_menu
                                           select new
                                           {
                                               id_combo = co.id_combo,
                                               descripcion_combo = co.descripcion,
                                               precio_combo = co.precio,
                                               imagen_combo = co.imagen
                                           }).FirstOrDefault() : null,
                                      // Si el tipo de plato es Promoción, obtener información de la promoción
                                      promocion = dp.Tipo_Plato == 'p' ?
                                          (from ip in _dulceSaborDbContext.items_promo
                                           join pr in _dulceSaborDbContext.promociones on ip.id_promo equals pr.id_promo
                                           where ip.id_item_menu == im.id_item_menu
                                           select new
                                           {
                                               id_promo = pr.id_promo,
                                               descripcion_promo = pr.nombre,
                                               precio_promo = pr.precio,
                                               fecha_inicio_promo = pr.fecha_inicio,
                                               fecha_final_promo = pr.fecha_final,
                                               imagen_promo = pr.imagen
                                           }).FirstOrDefault() : null
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

                    decimal precioFactura = detPedidoCompleto.Precio;

                    var comboPrecio = await (from ic in _dulceSaborDbContext.items_combo
                                             join c in _dulceSaborDbContext.combos on ic.id_combo equals c.id_combo
                                             where ic.id_item_menu == detPedidoCompleto.Id_plato
                                             select c.precio)
                                             .FirstOrDefaultAsync();
                    if (comboPrecio != null)
                    {
                        precioFactura = comboPrecio.Value;
                    }
                    else
                    {
                        var promocionPrecio = await (from ip in _dulceSaborDbContext.items_promo
                                                     join p in _dulceSaborDbContext.promociones on ip.id_promo equals p.id_promo
                                                     where ip.id_item_menu == detPedidoCompleto.Id_plato
                                                     select p.precio)
                                                    .FirstOrDefaultAsync();
                        if (promocionPrecio != null)
                        {
                            precioFactura = promocionPrecio.Value;
                        }
                    }

                    var detalleFacturaModelo = new detalle_fac
                    {
                        id_factura = lastEncFactura.id_factura,
                        id_detallepedido = id_detPedido,
                        precio = precioFactura,
                        total_plato = precioFactura * detPedidoCompleto.Cantidad,
                        cantidad = detPedidoCompleto.Cantidad,
                    };

                    _dulceSaborDbContext.Add(detalleFacturaModelo);
                }

                await _dulceSaborDbContext.SaveChangesAsync();

                foreach (var id_detPedido in ids)
                {
                    var detPedidoCompleto = await _dulceSaborDbContext.Detalle_Pedido
                        .FirstOrDefaultAsync(dp => dp.Id_DetalleCuenta == id_detPedido);
                    detPedidoCompleto.Estado = "Cancelado";
                }

                await _dulceSaborDbContext.SaveChangesAsync();


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

            var factura = await _dulceSaborDbContext.encabezado_fac
                .FirstOrDefaultAsync(f => f.id_factura == id);

            if (factura == null)
            {
                return NotFound();
            }

            var datosCliente = await _dulceSaborDbContext.clientes
                .FirstOrDefaultAsync(f => f.id_cliente == factura.id_cliente);
            ViewData["cliente"] = datosCliente;

            var datosDetFac = (from ef in _dulceSaborDbContext.encabezado_fac
                               join df in _dulceSaborDbContext.detalle_fac on ef.id_factura equals df.id_factura
                               join dp in _dulceSaborDbContext.Detalle_Pedido on df.id_detallepedido equals dp.Id_DetalleCuenta
                               where df.id_factura == factura.id_factura
                               select new
                               {
                                   id_detFac = df.id_detallefac,
                                   precio = df.precio,
                                   total = df.total_plato,
                                   cantidad = df.cantidad,
                                   nombreItem = (from im in _dulceSaborDbContext.items_menu
                                                 where im.id_item_menu == dp.Id_plato
                                                 select im.nombre).FirstOrDefault(),
                                   nombreCombo = (from ic in _dulceSaborDbContext.items_combo
                                                  join c in _dulceSaborDbContext.combos on ic.id_combo equals c.id_combo
                                                  where ic.id_item_menu == dp.Id_plato
                                                  select c.descripcion).FirstOrDefault(),
                                   nombrePromo = (from ip in _dulceSaborDbContext.items_promo
                                                  join p in _dulceSaborDbContext.promociones on ip.id_promo equals p.id_promo
                                                  where ip.id_item_menu == dp.Id_plato
                                                  select p.nombre).FirstOrDefault()
                               }).ToList();
            ViewData["detFacs"] = datosDetFac;


            return View(factura);
        }


    }
}
