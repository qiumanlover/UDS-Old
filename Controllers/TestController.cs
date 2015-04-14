using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UDS.Controllers
{
    public class TestController : Controller
    {
        //
        // GET: /Test/
        const int ChunkSize = 1024 * 1024; 
        public ActionResult Index()
        {
            return View("Upload");
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase[] fileToUpload)
        {

            foreach (HttpPostedFileBase file in fileToUpload)
            {

                string path = System.IO.Path.Combine(Server.MapPath("~/App_Data"), System.IO.Path.GetFileName(file.FileName));

                file.SaveAs(path);

            }



            ViewBag.Message = "File(s) uploaded successfully";

            return RedirectToAction("Index");

        }

    }
}
