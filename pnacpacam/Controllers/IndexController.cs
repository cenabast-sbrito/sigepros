using pnacpacam.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace pnacpacam.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(); // o PartialView()
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Login(FormCollection collection)
        {
            string[] user = collection["rutusername"].Replace(".", "").Trim().Split('-');
            string pass = collection["password"].Trim();
            string prog = collection["programa"].Trim();

            Usuario usuario = new Usuario();
              
            if (!usuario.Authorization(user[0], prog))    // SELECT rut FROM usuarios where rut = @Rut and estado=1;
                return Json(new { state = false, message = "Usuario no habilitado" });

            if (!usuario.Authenticate(user[0], pass))
                return Json(new { state = false, message = "Usuario o contraseña incorrectos" });

            usuario = usuario.GetUsuario(user[0], prog);

            if (!usuario.Estado)
                return Json(new { state = false, message = "Cuenta desactivada" });

            // Crear ticket con ROL
            var ticket = new FormsAuthenticationTicket(
                1,
                usuario.Rut,
                DateTime.Now,
                DateTime.Now.AddMinutes(60),
                false,
                usuario.Rol // AQUÍ VA EL ROL
            );

            string encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Secure = Request.IsSecureConnection
            };

            Response.Cookies.Add(cookie);

            // UI (NO seguridad)
            Session["PNACPACAM_RutUsuario"] = usuario.Rut;
            Session["PNACPACAM_NombreUsuario"] = usuario.Nombre.Trim() + " " + usuario.Apellido.Trim();
            Session["Email"] = usuario.email;
            Session["PNACPACAM_Rol"] = usuario.Rol;

            return Json(new
            {
                state = true,
                Nombre = Session["PNACPACAM_NombreUsuario"],
                Rol = usuario.Rol,
                Rut = usuario.Rut
            });
        }

    }
}






