using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pdf_Processor.Services.PdfService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Pdf_Processor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PdfController : ControllerBase
    {
        private readonly IPdfService pdfService;

        public PdfController(
            IPdfService pdfService
            )
        {
            this.pdfService = pdfService;
        }

        [HttpPost("[action]")]
        public async Task<object> UploadPdf([FromForm] IFormFile file)
        {
            var directoryPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\pdf-files"
                       );

            bool exists = System.IO.Directory.Exists(directoryPath);
            if (!exists)
            {
                System.IO.Directory.CreateDirectory(directoryPath);
            }

            var filePath = Path.Combine(
                directoryPath,
                file.FileName
                );

            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
                stream.Close();
            }

            return new
            {
                FileName = file.FileName
            };
        }

        [HttpGet("[action]")]
        public async Task<object> GetFilesList()
        {
            var directoryPath = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot\\pdf-files"
                       );

            var files = Directory.GetFiles(directoryPath);

            return new
            {
                Files = files
            };
        }

        [HttpGet("[action]")]
        public async Task<object> Search(string fileName, string query)
        {
            var result = await this.pdfService.Search(fileName, query);

            return result;
        }
    }
}
