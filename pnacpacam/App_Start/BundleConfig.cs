using System.Web;
using System.Web.Optimization;


namespace pnacpacam
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add( new StyleBundle("~/css")
                .Include(
                        "~/Content/css/normalize.css",
                        "~/Content/css/style.css",                        
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/pnacpacam.css",
                        "~/Content/css/css-select/select2.min.css"
                        )
            );
            bundles.Add(new ScriptBundle("~/core-js")
                .Include(
                        "~/Scripts/js/modernizr-2.6.2.js",
                        "~/Scripts/js/jquery-1.12.4.min.js",
                        "~/Scripts/js/bootstrap.min.js",
                        "~/Scripts/js/html2canvas.min.js"
            ));
            bundles.Add(new App_Start.ProperStyleBundle("~/plugins-css")
                .Include("~/Scripts/js/plugins/DataTables/css/dataTables.bootstrap.min.css", new CssRewriteUrlTransform())
            );
            bundles.Add(new ScriptBundle("~/plugins-js")
                .Include(
                        "~/Scripts/js/plugins/jquery-validate/jquery.Rut.js",
                        "~/Scripts/js/plugins/jquery-validate/additional-methods.js",
                        "~/Scripts/js/plugins/jquery-validate/jquery.validate.js",
                        "~/Scripts/js/plugins/jquery-validate/localization/messages_es.js",
                        "~/Content/fontawesome-6.0.0/js/all.js",
                        "~/Scripts/js/plugins/DataTables/jquery.dataTables.js",
                        "~/Scripts/js/plugins/DataTables/dataTables.bootstrap.js",
                        "~/Scripts/js/plugins/js-select/select2.min.js"
            ));
            bundles.Add(new ScriptBundle("~/login").Include("~/Scripts/js/app/login.js"));
            bundles.Add(new ScriptBundle("~/app").Include("~/Scripts/js/app/app.js"));
            bundles.Add(new ScriptBundle("~/inventario").Include("~/Scripts/js/app/inventario.js"));
            bundles.Add(new ScriptBundle("~/complementario").Include("~/Scripts/js/app/complementario.js"));
            bundles.Add(new ScriptBundle("~/revision-mv").Include("~/Scripts/js/app/detalleInventario.js"));
            bundles.Add(new ScriptBundle("~/reporte").Include("~/Scripts/js/app/reporte.js"));
            bundles.Add(new ScriptBundle("~/carga").Include("~/Scripts/js/app/cargaInventario.js"));
            bundles.Add(new ScriptBundle("~/cierre").Include("~/Scripts/js/app/cierreInventario.js"));
            bundles.Add(new ScriptBundle("~/compra").Include("~/Scripts/js/app/compra.js"));
            bundles.Add(new ScriptBundle("~/venta").Include("~/Scripts/js/app/venta.js"));
            bundles.Add(new ScriptBundle("~/producto").Include("~/Scripts/js/app/producto.js"));
            bundles.Add(new ScriptBundle("~/usuarios").Include("~/Scripts/js/app/usuarios.js"));
            bundles.Add(new ScriptBundle("~/proveedor").Include("~/Scripts/js/app/proveedor.js"));
            bundles.Add(new ScriptBundle("~/poblarListas").Include("~/Scripts/js/app/poblarListas.js"));
            bundles.Add(new ScriptBundle("~/administraFormularios").Include("~/Scripts/js/app/administraFormularios.js"));
            BundleTable.EnableOptimizations = true;
        }
    }
}
