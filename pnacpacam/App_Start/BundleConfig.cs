using System.Web.Optimization;


namespace pnacpacam
{
    public class BundleConfig
    {
        // Para obtener más información sobre las uniones, visite https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new StyleBundle("~/css")
                .Include(
                        "~/Content/css/normalize.css",
                        "~/Content/css/style.css",
                        "~/Content/css/bootstrap.css",
                        "~/Content/css/css-select/select2.min.css",
                        "~/Content/css/Spinner.css"
                         )
            );
            bundles.Add(new StyleBundle("~/datatableExport-css")
                .Include(
                        "~/Content/datatableExport/buttons.dataTables.css",
                        "~/Content/datatableExport/dataTables.dataTables.css"
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
            bundles.Add(new ScriptBundle("~/factura").Include("~/Scripts/js/app/_factura.js"));
            bundles.Add(new ScriptBundle("~/guia").Include("~/Scripts/js/app/_guia.js"));
            bundles.Add(new ScriptBundle("~/proveedor").Include("~/Scripts/js/app/_proveedor.js"));
            bundles.Add(new ScriptBundle("~/login").Include("~/Scripts/js/app/login.js"));
            bundles.Add(new ScriptBundle("~/app").Include("~/Scripts/js/app/app.js"));
            bundles.Add(new ScriptBundle("~/tools").Include("~/Scripts/js/app/tools.js"));
            bundles.Add(new ScriptBundle("~/producto").Include("~/Scripts/js/app/producto.js"));
            bundles.Add(new ScriptBundle("~/rol").Include("~/Scripts/js/app/_rol.js"));
            bundles.Add(new ScriptBundle("~/usuarios").Include("~/Scripts/js/app/usuarios.js"));
            bundles.Add(new ScriptBundle("~/administraFormularios").Include("~/Scripts/js/app/administraFormularios.js"));
            bundles.Add(new ScriptBundle("~/zdis022").Include("~/Scripts/js/app/_zdis022.js"));
            bundles.Add(new ScriptBundle("~/filtro").Include("~/Scripts/js/app/_filtro.js"));
            bundles.Add(new ScriptBundle("~/documento").Include("~/Scripts/js/app/_documento.js"));

            bundles.Add(new ScriptBundle("~/datatableExport-js")
           .Include(
            "~/Scripts/datatableExport/jquery-3.7.1.js",
            "~/Scripts/datatableExport/dataTables.js",
            "~/Scripts/datatableExport/dataTables.buttons.js",
            "~/Scripts/datatableExport/buttons.dataTables.js",
            "~/Scripts/datatableExport/jszip.min.js",
            "~/Scripts/datatableExport/pdfmake.min.js",
            "~/Scripts/datatableExport/vfs_fonts.js",
            "~/Scripts/datatableExport/buttons.html5.min.js",
            "~/Scripts/datatableExport/buttons.print.min.js"
            ));


            BundleTable.EnableOptimizations = true;
        }
    }
}
