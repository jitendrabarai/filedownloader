using FileDownloader.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Threading;
namespace FileDownloader.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private IWebHostEnvironment _environment;
        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment environment)
        {
            _logger = logger;
            this._environment = environment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Download(CancellationToken cancellationToken)
        {
            string[] files = Directory.GetFiles(Path.Combine(this._environment.WebRootPath, "contentfiles/"));
            var content = new byte[1];
            foreach (var file in files)
            {
                cancellationToken.ThrowIfCancellationRequested();
                content = CombineByteData(content, await System.IO.File.ReadAllBytesAsync(file));
                Thread.Sleep(2000);
            }
            string filename = $"{DateTime.Now.Ticks}.txt";
            using (FileStream fs = System.IO.File.Create(Path.Combine(Path.Combine(this._environment.WebRootPath, "contentfiles"), filename)))
            {
                // Add some information to the file.
                fs.Write(content, 0, content.Length);
            }
            return Json(filename);
        }
        public IActionResult DownloadFile(string filename)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(Path.Combine(Path.Combine(this._environment.WebRootPath, "contentfiles"), filename));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, filename);
        }


        byte[] CombineByteData(byte[] existingdata, byte[] newdata)
        {
            byte[] bytes = new byte[existingdata.Length + newdata.Length];
            Buffer.BlockCopy(existingdata, 0, bytes, 0, existingdata.Length);
            Buffer.BlockCopy(newdata, 0, bytes, existingdata.Length, newdata.Length);
            return bytes;
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
