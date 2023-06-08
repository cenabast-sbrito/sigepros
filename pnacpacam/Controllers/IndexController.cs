using pnacpacam.Models;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Security.Principal;

namespace pnacpacam.Controllers
{
    [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
    public class IndexController : Controller
    {
        // GET: Index
        public ActionResult Index()
        {
        
              //        https://testaplicacionesweb.cenabast.cl/ProyectosTI/pnac
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("https://testaplicacionesweb.cenabast.cl/");
//            client.BaseAddress = new Uri("https://testaplicacionesweb.cenabast.cl/ProyectosTI/pnac");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            if (Session["Perfil"] == null)
            {
                return PartialView();
            } else 
            {
                return Redirect("/Home/#/#bienvenida");
//                if (Session["Perfil"].ToString() != "3") { return Redirect("/Home/#/#bienvenida"); }
//                else
//                    return Redirect("/Home/#/#ingreso-resultados");
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult Login(FormCollection collection)
        {
            string[] user = collection["username"].Replace(".", "").Trim().Split('-');
            string pass = collection["password"].Trim();
            Usuario usuario = new Usuario();
            Funcionario funcionario = new Funcionario();
            Periodo per = new Periodo();
            if (usuario.Authorization(user[0])) 
            {
                if (funcionario.Authenticate(user[0], pass))
                {
                    funcionario = funcionario.GetFuncionario(user[0]);

                    ///AQUI DEBO OBTENER LOS PARAMETROS DE CONFIGURACION GLOBALES DEL SISTEMA TAL COMO EL IVA, ETC.

                    Session["Rut"] = funcionario.Rut;
                    Session["Nombre"] = funcionario.Nombre.Trim() + " " + funcionario.Apellido.Trim();
                    Session["Email"] = funcionario.Email;
                    // usuario = usuario.GetUsuario(user[0], "primary"); 
                    usuario = usuario.GetUsuario(funcionario.Rut);
                    Session["Perfil"] = usuario.IdPerfil;

                    //                    string comoConecto = "primary";
                    /*if (usuario.Rut == "1") comoConecto = "primaryQA";
                    if (usuario.Rut == "2") comoConecto = "primaryQA"; // este rut es perfil 2 por lo que no es necesario que este como usuario en desa lo mismo para el 3
                    if (usuario.Rut == "3") comoConecto = "primaryQA";
                    */
                    //       Session["StringConexion"] = "primary";
                    /// Session["Periodo"] = per.GetPeriodo("primary").PeriodoActivo;

                    FormsAuthentication.SetAuthCookie(funcionario.Rut, false);

                    return Json(new { state = true, perfil = usuario.IdPerfil,  Nombre= Session["Nombre"], message = "" });

                }
                else
                {
                    return Json(new { state = false, perfil = "", message = "Estimado, su usuario y pasword no coinciden. Comuniquese con el administrador." });
                }
            }
            else
            {
                return Json(new { state = false, message = "Estimado, su usuario no se encuentra habilitado en esta plataforma. Comúniquese con el administrador." });
            }
        }

    }
}
