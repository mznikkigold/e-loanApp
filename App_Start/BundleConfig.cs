using System.Web;
using System.Web.Optimization;

namespace e_loanApp
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                         "~/Scripts/popper.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryajax").Include(
               "~/Scripts/jquery.unobtrusive-ajax.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                       "~/Content/login.css"));


            bundles.Add(new StyleBundle("~/Content/Forms/css").Include(
                      "~/Content/hamburger.min.css",
                      "~/Content/animate.css",
                       "~/Content/select2.min.css",
                       "~/Content/main.css",
                       "~/Content/util.css"));

            //bundles.Add(new StyleBundle("~/NewContent/css").Include(
            //       "~/Content/all.min.css",
            //        "~/Content/adminlte.min.css"));

            //bundles.Add(new StyleBundle("~/Content/css").Include(
            //    "~/Content/login.css"));
        }
    }
}
