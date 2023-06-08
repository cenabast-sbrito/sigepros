using pnacpacam.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pnacpacam.Controllers
{
    [Authorize]
    [SessionExpire]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class UsuarioController : Controller
    {
        // GET: Usuarios
        public JsonResult Index()
        {
            UsuarioViewModelModify uvmm = new UsuarioViewModelModify();
//            return Json(uvmm.GeUsuarios(Session["StringConexion"].ToString()), JsonRequestBehavior.AllowGet);
            return Json(uvmm.GeUsuarios(), JsonRequestBehavior.AllowGet);
        }

        // GET: Usuario
        public JsonResult GetUsuario(string rut)
        {
            UsuarioViewModelModify usuario = new UsuarioViewModelModify();
            usuario = usuario.GetUsuario(rut, Session["StringConexion"].ToString());
            return Json(usuario, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        
        public JsonResult SetUsuario(Usuario user)
        {
            int i = user.Ingresar(user);
            if (i > 0)
            {
                return Json(new { status = true, message = "Se ha agregado el Usuario exitosamente." });
            }
            else {
                return Json(new { status = false, message = "Ha ocurrido un error." });
            }

        }
        public JsonResult GetName(string name)
        {
            UsuarioViewModel users = new UsuarioViewModel();
            var usersList = users.GetName(name);
            return Json(usersList, JsonRequestBehavior.AllowGet);
        }
        
        [HttpPut]
        public JsonResult PutUsuario(Usuario user)
        {
            int i = user.Actualizar(user);
            if (i > 0)
            {
                return Json(new { status = true, message = "Se ha actualizado el Usuario exitosamente." });
            }
            else
            {
                return Json(new { status = false, message = "Ha ocurrido un error." });
            }
        }
        
        /*
        public JsonResult GetDepartamentos()
        {
            Compra departamentos = new Compra();
            var funcsList = departamentos.getDepartamentos();
            return Json(funcsList, JsonRequestBehavior.AllowGet);
        }*/
        /*
        public JsonResult GetSubDepartamentos(string CodigoDpto)
        {
            Compra.SubDepartamentos departamentos = new Compra.SubDepartamentos();
            var funcsList = departamentos.getSubDepartamentos(CodigoDpto);
            return Json(funcsList, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetUnidades(string CodigoDpto, string CodigoSubDepto)
        {
            Compra.SubDepartamentos.Unidades unidades = new Compra.SubDepartamentos.Unidades();
            var funcsList = unidades.getUnidades(CodigoDpto, CodigoSubDepto);
            return Json(funcsList, JsonRequestBehavior.AllowGet);
        }
    */
    }
}
