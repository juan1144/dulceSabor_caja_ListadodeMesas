using dulceSabor_caja_ListadodeMesas.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

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
                                select new
                                 {
                                     id_mesa = m.id_mesa,
                                     cantidad_personas = m.cantidad_personas,
                                     nombre_estado = e.nombre,
                                     nombre_mesa = m.nombre_mesa
                                 }).ToList();
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
                                where m.id_mesa == id
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

            return View(cuenta);

        }

        //FIN DEL MÉTODO DE detalleFactura
    }
}
