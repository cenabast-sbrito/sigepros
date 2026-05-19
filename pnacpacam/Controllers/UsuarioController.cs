using DocumentFormat.OpenXml.Office2010.ExcelAc;
using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Web.Mvc;

namespace pnacpacam.Controllers
{
    [Authorize]
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class UsuarioController : Controller
    {
        public JsonResult GetUsuarios()
        {
            Usuario uvmm = new Usuario();
            return Json(uvmm.GeUsuarios(), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUsuario(string rut)
        {
            Usuario usuario = new Usuario();
            usuario = usuario.GetUsuario2 (rut);
            return Json( usuario, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SetUsuario(Usuario user)
        {

            string i = user.Ingresar(user);
            if (i == "")
            {
                return Json(new { status = true, message = "Se ha agregado el Usuario exitosamente." });
            }
            else
            {
                return Json(new { status = false, message = i });
            }

        }

        public JsonResult ToggleUsuario(Usuario usuario)
        {
            try
            {
                var rows = usuario.toggleUsuario(usuario); // el método corregido
                return rows > 0
                    ? Json(new { status = true, message = "Usuario actualizado correctamente." })
                    : Json(new { status = false, message = "No se actualizó ningún registro." });
            }
            catch (SqlException ex)
            {
                // Los THROW 5000x del SP caen aquí con Number personalizado
                return Json(new { status = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Ocurrió un error al actualizar el usuario." });
            }
        }
        public JsonResult GetName(string name)
        {
            UsuarioViewModel users = new UsuarioViewModel();
            var usersList = users.GetName(name);
            return Json(usersList, JsonRequestBehavior.AllowGet);
        }
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
        public JsonResult DelUsuario(string rut, int estado)
        {
            Usuario user = new Usuario();
            int i = user.Eliminar(rut, estado);
            if (i > 0)
            {
                return Json(new { status = true, message = "Se ha eliminado el Usuario exitosamente." });
            }
            else
            {
                return Json(new { status = false, message = "Ha ocurrido un error." });
            }
        }

        

    }
}
