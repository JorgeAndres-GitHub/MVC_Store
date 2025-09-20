using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CapaPresentacionAdmin.Models;
using CapaEntidad;
using CapaNegocio;
using System.Data;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;

namespace CapaPresentacionAdmin.Controllers;

[Authorize]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Usuarios()
    {
        return View();
    }

    [HttpGet]
    public JsonResult ListarUsuarios()
    {
        List<Usuario> oLista = new List<Usuario>();

        oLista = new CN_Usuarios().Listar();

        return Json(new { data = oLista });
    }

    [HttpPost]
    public JsonResult GuardarUsuario(Usuario obj)
    {
        object resultado;
        string mensaje = string.Empty;

        if (obj.IdUsuario == 0)
        {
            resultado = new CN_Usuarios().Registrar(obj, out mensaje);
        }
        else
        {
            resultado = new CN_Usuarios().Editar(obj, out mensaje);
        }

        return Json(new { resultado = resultado, mensaje = mensaje });
    }

    [HttpPost]
    public JsonResult EliminarUsuario(int id)
    {
        bool respuesta = false;
        string mensaje = string.Empty;

        respuesta = new CN_Usuarios().Eliminar(id, out mensaje);

        return Json(new { resultado = respuesta, mensaje = mensaje });
    }

    [HttpGet]
    public JsonResult ListaReporte(string fechaInicio, string fechaFin, string idTransaccion)
    {
        List<Reporte> lista = new List<Reporte>();

        lista = new CN_Reporte().Ventas(fechaInicio, fechaFin, idTransaccion);

        return Json(new { data = lista });
    }

    [HttpGet]
    public JsonResult VistaDashBoard()
    {
        DashBoard objeto = new CN_Reporte().VerDashBoard();

        return Json(new { resultado = objeto });
    }

    [HttpPost]
    public FileResult ExportarVenta(string fechaInicio, string fechaFin, string idTransaccion)
    {
        List<Reporte> lista = new List<Reporte>();
        lista = new CN_Reporte().Ventas(fechaInicio, fechaFin, idTransaccion);

        DataTable dt = new DataTable();
        dt.Locale = new System.Globalization.CultureInfo("es-CO");
        dt.Columns.Add("Fecha Venta", typeof(string));
        dt.Columns.Add("Cliente", typeof(string));
        dt.Columns.Add("Producto", typeof(string));
        dt.Columns.Add("Precio", typeof(decimal));
        dt.Columns.Add("Cantidad", typeof(int));
        dt.Columns.Add("Total", typeof(decimal));
        dt.Columns.Add("IdTransaccion", typeof(string));
        foreach (Reporte rp in lista)
        {
            dt.Rows.Add(new object[]{
                rp.FechaVenta, rp.Cliente, rp.Producto, rp.Precio, rp.Cantidad, rp.Total, rp.IdTransaccion 
                }
            );
        }

        dt.TableName = "Datos";

        using(XLWorkbook wb = new XLWorkbook())
        {
            wb.Worksheets.Add(dt);
            using (MemoryStream stream = new MemoryStream())
            {
                wb.SaveAs(stream);
                stream.Position = 0;
                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ReporteVentas" + DateTime.Now.ToString() +".xlsx");
            }
        }
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
