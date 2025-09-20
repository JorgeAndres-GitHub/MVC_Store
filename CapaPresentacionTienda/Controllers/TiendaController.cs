using CapaDatos;
using CapaEntidad;
using CapaEntidad.Paypal;
using CapaNegocio;
using CapaPresentacionTienda.Models;
using CapaPresentacionTienda.NewFolder;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Globalization;

namespace CapaPresentacionTienda.Controllers
{
    public class TiendaController : Controller
    {
        private readonly IConfiguration _configuration;

        public TiendaController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult DetalleProducto(int idproducto = 0)
        {
            Producto oProducto = new Producto();
            bool conversion;

            oProducto = new CN_Producto().Listar().Where(p => p.IdProducto == idproducto).FirstOrDefault();

            if(oProducto != null)
            {
                oProducto.Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oProducto.RutaImagen, oProducto.NombreImagen), out conversion);
                oProducto.Extension = Path.GetExtension(oProducto.NombreImagen);
            }

            return View(oProducto);
        }

        [HttpGet]
        public JsonResult ListaCategorias()
        {
            List<Categoria> lista = new List<Categoria>();

            lista = new CN_Categoria().Listar();

            return Json(new { data = lista });
        }

        [HttpPost]
        public JsonResult ListarMarcaPorCategoria(int idCategoria)
        {
            List<Marca> lista = new List<Marca>();

            lista = new CN_Marca().ListarMarcaPorCategoria(idCategoria);

            return Json(new { data = lista });
        }

        [HttpPost]
        public JsonResult ListarProductos(int idCategoria, int idMarca)
        {
            List<Producto> lista = new List<Producto>();
            bool conversion;

            lista = new CN_Producto().Listar().Select(p => new Producto()
            {
                IdProducto = p.IdProducto,
                Nombre = p.Nombre,
                Descripcion = p.Descripcion,
                oMarca = p.oMarca,
                oCategoria = p.oCategoria,
                Precio = p.Precio,
                Stock = p.Stock,
                RutaImagen = p.RutaImagen,
                Base64 = CN_Recursos.ConvertirBase64(Path.Combine(p.RutaImagen, p.NombreImagen), out conversion),
                Extension = Path.GetExtension(p.NombreImagen),
                Activo = p.Activo
            }).Where(p => p.oCategoria.IdCategoria == (idCategoria == 0 ? p.oCategoria.IdCategoria : idCategoria) && p.oMarca.IdMarca == (idMarca == 0 ? p.oMarca.IdMarca : idMarca) 
            && p.Stock > 0 && p.Activo).ToList();

            var jsonResult = Json(new { data = lista });

            return jsonResult;
        }

        [HttpPost]
        public JsonResult AgregarCarrito(int idProducto)
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");            
            int idCliente = cliente.IdCliente;
            bool existe = new CN_Carrito().ExisteCarrito(idCliente, idProducto);
            bool respuesta = false;
            string mensaje = string.Empty;

            if(existe)
            {
                mensaje = "El producto ya se encuentra en el carrito";
            }
            else
            {
                respuesta = new CN_Carrito().OperacionCarrito(idCliente, idProducto, true, out mensaje);
            }
            return Json(new { respuesta, mensaje });
        }

        [HttpGet]
        public JsonResult CantidadEnCarrito()
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");
            int idCliente = cliente.IdCliente;
            int cantidad = new CN_Carrito().CantidadEnCarrito(idCliente);
            return Json(new { cantidad });
        }

        [HttpPost]
        public JsonResult ListarProductosCarrito()
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");
            int idCliente = cliente.IdCliente;
            List<Carrito> oLista = new List<Carrito>();
            bool conversion;
            oLista = new CN_Carrito().ListarProducto(idCliente).Select(oc => new Carrito()
            {
                oProducto = new Producto()
                {
                    IdProducto = oc.oProducto.IdProducto,
                    Nombre = oc.oProducto.Nombre,
                    oMarca = oc.oProducto.oMarca,
                    Precio = oc.oProducto.Precio,
                    RutaImagen = oc.oProducto.RutaImagen,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oc.oProducto.RutaImagen, oc.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(oc.oProducto.NombreImagen),
                    Activo = oc.oProducto.Activo
                },
                Cantidad = oc.Cantidad
            }).ToList();

            return Json(new { data = oLista });
        }

        [HttpPost]
        public JsonResult OperacionCarrito(int idProducto, bool sumar)
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");
            int idCliente = cliente.IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;

            respuesta = new CN_Carrito().OperacionCarrito(idCliente, idProducto, sumar, out mensaje);
            return Json(new { respuesta, mensaje });
        }

        [HttpPost]
        public JsonResult EliminarCarrito(int idProducto)
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");
            int idCliente = cliente.IdCliente;
            bool respuesta = false;
            string mensaje = string.Empty;
            respuesta = new CN_Carrito().EliminarCarrito(idCliente, idProducto);
            return Json(new { respuesta, mensaje });
        }

        [HttpPost]
        public JsonResult ObtenerDepartamento()
        {
            List<Departamento> oLista = new List<Departamento>();
            oLista = new CN_Ubicacion().ObtenerDepartamento();
            return Json(new { lista = oLista });
        }

        [HttpPost]
        public JsonResult ObtenerProvincia(string idDepartamento)
        {
            List<Provincia> oLista = new List<Provincia>();
            oLista = new CN_Ubicacion().ObtenerProvincia(idDepartamento);
            return Json(new { lista = oLista });
        }

        [HttpPost]
        public JsonResult ObtenerDistrito(string idDepartamento, string idProvincia)
        {
            List<Distrito> oLista = new List<Distrito>();
            oLista = new CN_Ubicacion().ObtenerDistrito(idDepartamento, idProvincia);
            return Json(new { lista = oLista });
        }

        [ValidarSession]
        [Authorize]
        public ActionResult Carrito()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> ProcesarPago(List<Carrito> oListaCarrito, Venta oVenta)
        {
            
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");

            decimal total = 0;
            DataTable detalleVenta = new DataTable();
            detalleVenta.Locale = new CultureInfo("es-PE");
            detalleVenta.Columns.Add("IdProducto", typeof(string));
            detalleVenta.Columns.Add("Cantidad", typeof(int));
            detalleVenta.Columns.Add("Total", typeof(decimal));

            List<Item> oListaItem = new List<Item>();

            foreach (Carrito oCarrito in oListaCarrito)
            {
                decimal subtotal = Convert.ToDecimal(oCarrito.Cantidad.ToString()) * oCarrito.oProducto.Precio;
                total += subtotal;
                oListaItem.Add(new Item()
                {
                    name = oCarrito.oProducto.Nombre,
                    quantity = oCarrito.Cantidad.ToString(),
                    unit_amount = new UnitAmount()
                    {
                        currency_code = "USD",
                        value = oCarrito.oProducto.Precio.ToString("G", new CultureInfo("es-PE"))
                    }
                });
                detalleVenta.Rows.Add(new object[] {
                    oCarrito.oProducto.IdProducto.ToString(),
                    oCarrito.Cantidad,
                    subtotal
                });
            }

            PurchaseUnit purchaseUnit = new PurchaseUnit()
            {
                amount = new Amount()
                {
                    currency_code = "USD",
                    value = total.ToString("G", new CultureInfo("es-PE")),
                    breakdown = new Breakdown()
                    {
                        item_total = new ItemTotal()
                        {
                            currency_code = "USD",
                            value = total.ToString("G", new CultureInfo("es-PE"))
                        }
                    }
                },
                description = "Compra de articulo de mi tienda",
                items = oListaItem
            };

            Checkout_Order order = new Checkout_Order()
            {
                intent = "CAPTURE",
                purchase_units = new List<PurchaseUnit>() { purchaseUnit },
                application_context = new ExperienceContext()
                {
                    brand_name = "Mi Tienda",
                    landing_page = "NO_PREFERENCE",
                    user_action = "PAY_NOW",
                    return_url = "https://localhost:7015/Tienda/PagoEfectuado",
                    cancel_url = "https://localhost:7015/Tienda/Carrito"
                }
            };

            oVenta.MontoTotal = total;
            oVenta.IdCliente = cliente.IdCliente;

            // Serializar a JSON para TempData
            TempData["VentaJson"] = System.Text.Json.JsonSerializer.Serialize(oVenta);

            // Convertir DataTable a List para serialización
            var detalleVentaList = detalleVenta.AsEnumerable()
                .Select(row => new {
                    IdProducto = row.Field<string>("IdProducto"),
                    Cantidad = row.Field<int>("Cantidad"),
                    Total = row.Field<decimal>("Total")
                }).ToList();

            TempData["DetalleVentaJson"] = System.Text.Json.JsonSerializer.Serialize(detalleVentaList);

            var settings = _configuration.GetSection("Paypal").Get<PaypalSettings>();
            CN_Paypal paypal = new CN_Paypal(settings);

            Response_Paypal<Response_Checkout> response_paypal = new Response_Paypal<Response_Checkout>();
            response_paypal = await paypal.CrearSolicitud(order);

            return Json(response_paypal);
        }

        [ValidarSession]
        [Authorize]
        public async Task<ActionResult> PagoEfectuado()
        {
            try
            {
                string token = Request.Query["token"].ToString();

                var settings = _configuration.GetSection("Paypal").Get<PaypalSettings>();
                CN_Paypal paypal = new CN_Paypal(settings);
                Response_Paypal<Response_Capture> response_paypal = new Response_Paypal<Response_Capture>();
                response_paypal = await paypal.AprobarPago(token);

                ViewData["Status"] = response_paypal.Status;
                if (response_paypal.Status)
                {
                    // Deserializar desde TempData
                    var ventaJson = TempData["VentaJson"]?.ToString();
                    var detalleVentaJson = TempData["DetalleVentaJson"]?.ToString();

                    if (!string.IsNullOrEmpty(ventaJson) && !string.IsNullOrEmpty(detalleVentaJson))
                    {
                        Venta oVenta = System.Text.Json.JsonSerializer.Deserialize<Venta>(ventaJson);
                        var detalleVentaList = System.Text.Json.JsonSerializer.Deserialize<List<dynamic>>(detalleVentaJson);

                        // Recrear DataTable
                        DataTable detalleVenta = new DataTable();
                        detalleVenta.Locale = new CultureInfo("es-PE");
                        detalleVenta.Columns.Add("IdProducto", typeof(string));
                        detalleVenta.Columns.Add("Cantidad", typeof(int));
                        detalleVenta.Columns.Add("Total", typeof(decimal));

                        // Llenar DataTable con los datos deserializados...
                        foreach (var item in detalleVentaList)
                        {
                            detalleVenta.Rows.Add(new object[] {
                                item.GetProperty("IdProducto").GetString(),
                                item.GetProperty("Cantidad").GetInt32(),
                                item.GetProperty("Total").GetDecimal()
                            });
                        }

                        oVenta.IdTransaccion = response_paypal.Response.purchase_units[0].payments.captures[0].id;
                        string mensaje = string.Empty;
                        bool respuesta = new CN_Venta().Registrar(oVenta, detalleVenta, out mensaje);

                        ViewData["IdTransaccion"] = oVenta.IdTransaccion;
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en PagoEfectuado: {ex.Message}");
                ViewData["Status"] = false;
                return View();
            }
        }

        [ValidarSession]
        [Authorize]
        public ActionResult MisCompras()
        {
            var cliente = HttpContext.Session.GetObjectFromJson<Cliente>("Cliente");
            int idCliente = cliente.IdCliente;
            List<DetalleVenta> oLista = new List<DetalleVenta>();
            bool conversion;
            oLista = new CN_Venta().ListarCompras(idCliente).Select(oc => new DetalleVenta()
            {
                oProducto = new Producto()
                {
                    Nombre = oc.oProducto.Nombre,
                    Precio = oc.oProducto.Precio,
                    Base64 = CN_Recursos.ConvertirBase64(Path.Combine(oc.oProducto.RutaImagen, oc.oProducto.NombreImagen), out conversion),
                    Extension = Path.GetExtension(oc.oProducto.NombreImagen)
                },
                Cantidad = oc.Cantidad,
                Total = oc.Total,
                IdTransaccion = oc.IdTransaccion,
            }).ToList();

            return View(oLista);
        }
    }
}
