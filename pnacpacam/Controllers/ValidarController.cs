using pnacpacam.Models;
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
    public class ValidarController : Controller
    {
        // GET: Validar
        public ActionResult Index(string IdCode)
        {
            if (string.IsNullOrEmpty(IdCode)) {
                ViewBag.IdDcoumento = "";
                ViewBag.CodigoVErificacion = "";
            }
            else {
                string[] data = IdCode.Split('-');
                ViewBag.IdDocumento = data[0];
                ViewBag.CodigoVErificacion = data[1];
            }
            return View();
        }

        // GET: Validar/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Validar/Create
        public ActionResult Create()
        {
            return View();
        }




        // GET: Validar/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Validar/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Validar/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Validar/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
