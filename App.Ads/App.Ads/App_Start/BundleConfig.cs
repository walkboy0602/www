﻿using System.Web;
using System.Web.Optimization;

namespace App.Ads
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {

            #region Script


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new StyleBundle("~/Content/css").Include("~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/jqHead").Include(
                "~/Scripts/common/holder.js"
            ));

            //plugin
            bundles.Add(new ScriptBundle("~/bundles/plugin").Include(
                "~/Scripts/common/global.js",
                "~/Scripts/common/bootstrap.js",
                "~/Scripts/common/bootstrap.datepicker.js",
                "~/Scripts/common/bootstrap.js",
                "~/Scripts/common/select2.js",
                "~/Scripts/common/theme.js",
                "~/Scripts/common/spin.js",
                "~/Scripts/common/summernote.js",
                "~/Scripts/common/owl.carousel.js",
                "~/Content/fancybox/jquery.fancybox.js", //http://fancyapps.com/fancybox/
                "~/Content/fancybox/helpers/jquery.fancybox-buttons.js",
                "~/Content/fancybox/helpers/jquery.fancybox-media.js"
            ));

            //Validation
            bundles.Add(new ScriptBundle("~/bundles/validation").Include(
                "~/Scripts/validation/languages/jquery.validationEngine-en.js", //Validation Engine English
                "~/Scripts/validation/jquery.validationEngine.js" //Validation Engine
                ));

            //Angular
            bundles.Add(new ScriptBundle("~/bundles/angular").Include(
                 "~/Scripts/angular/1.2.26/angular.js",
                 "~/Scripts/angular/1.2.26/angular-resource.js",
                 "~/Scripts/angular/1.2.26/angular-route.js",
                 "~/Scripts/angular/1.2.26/angular-sanitize.js",
                 "~/Scripts/angular/apps.js",
                 "~/Scripts/angular/api.js",
                 "~/Scripts/angular/service.js",
                 "~/Scripts/angular/directive.js",
                 "~/Scripts/angular/controller/listing.js",
                 "~/Scripts/angular/controller/enquiry.js",
                 "~/Scripts/angular/controller/account.js",
                 "~/Scripts/angular/controller/admin.js",
                 "~/Scripts/angular/3rd/angular-bootstrap-select.js",
                 "~/Scripts/angular/3rd/ui-bootstrap-tpls-0.6.0.js",
                 "~/Scripts/angular/3rd/loading-bar.js",
                 "~/Scripts/angular/3rd/angular-strap.js",
                 "~/Scripts/angular/3rd/angular-strap.tpl.js"
                 ));


            //File uploader
            bundles.Add(new ScriptBundle("~/bundles/fileupload").Include(
                "~/Scripts/fileupload/vendor/jquery.ui.widget.js",
                "~/Scripts/fileupload/load-image.js",
                "~/Scripts/fileupload/jquery.iframe-transport.js",
                "~/Scripts/fileupload/jquery.fileupload.js",
                "~/Scripts/fileupload/jquery.fileupload-process.js",
                "~/Scripts/fileupload/jquery.fileupload-image.js",
                "~/Scripts/fileupload/jquery.fileupload-validate.js",
                "~/Scripts/fileupload/jquery.fileupload-angular.js",
                "~/Scripts/fileupload/app.js"
                ));

            //CkEditor
            bundles.Add(new ScriptBundle("~/bundles/ckeditor").Include(
                "~/Scripts/ckeditor/ckeditor.js"
                ));

            #endregion

            #region CSS

            //Bootstrap
            bundles.Add(new StyleBundle("~/Content/bootstrap").Include(
                         "~/Content/bootstrap/bootstrap.css",
                         "~/Content/bootstrap/bootstrap-bazaar.css",
                         "~/Content/bootstrap/bootstrap-additions.css",
                         "~/Content/bootstrap/bootstrap-style.css",
                         "~/Content/bootstrap/bootstrap-theme.css"
                         ).Include("~/Content/bootstrap/font-awesome.css", new CssRewriteUrlTransform()));


            //Bazzar Themes
            bundles.Add(new StyleBundle("~/Content/css").Include(
                        "~/Content/themes/bazaar/theme.css",
                        "~/Content/global/elements.css",
                        "~/Content/global/common.css",
                        "~/Content/global/layout.css",
                        "~/Content/global/table.css",
                        "~/Content/global/icons.css",
                        "~/Content/themes/bazaar/signin.css",
                        "~/Content/fancybox/jquery.fancybox.css",
                        "~/Content/fancybox/helpers/jquery.fancybox-buttons.css",
                        "~/Content/libraries/simple-sidebar.css",
                        "~/Content/libraries/loading-bar.css",
                        "~/Content/libraries/owl.carousel.css",
                        "~/Content/libraries/owl.theme.css"
                        ));

            //Form CSS
            bundles.Add(new StyleBundle("~/Content/css/form").Include(
                            "~/Content/libraries/select2.css",
                            "~/Content/libraries/select2-bootstrap.css",
                            "~/Content/themes/bazaar/gallery.css",
                            "~/Content/fileupload/jquery.fileupload-ui.css",
                            "~/Content/fileupload/jquery.fileupload.css",
                            "~/Content/libraries/summernote.css"
                ));

            #endregion
            bundles.IgnoreList.Clear();
            BundleTable.EnableOptimizations = false;
        }
    }
}