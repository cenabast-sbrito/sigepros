using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace pnacpacam.Controllers
{
    public class RolController : Controller
    {
        public JsonResult GetRolesUsuario(string rut)
        {
            RolesUsuarios uvmm = new RolesUsuarios();

            List<RolesUsuarios> res = uvmm.GetRolesUsuario(rut);
            if (res != null) return Json(new { res = res, status = true, message = "" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { res = res, status = false, message = "No existen roles" }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult DelRolesUsuario(string rut, string organismo)
        {
            RolesUsuarios uvmm = new RolesUsuarios();

            string res = uvmm.DelRolesUsuario(rut);
            if (res == "OK") return Json(new { res = res, status = true, message = "" }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { status = false, message = res }, JsonRequestBehavior.AllowGet);

        }
        public JsonResult getRoles(string organismo, string rut)
        {
            Rol rol = new Rol();
            return Json(rol.GetRoles(organismo, rut), JsonRequestBehavior.AllowGet);
        }
        public JsonResult setRolUsuario(string rut, string rolUsuario)
        {
            Rol rol = new Rol();
            return Json(rol.setRolUsuario( rut, rolUsuario), JsonRequestBehavior.AllowGet);
        }
    }
}
