using Home.Extractor.Entities;
using Home.Extractor.Parsers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Home.Extractor.Controllers {
    [ApiController]
    [Route("[controller]/{shop}")]
    public class ExtractorController : ControllerBase {
        private readonly PdfConverter pdfConverter;
        private readonly ILogger<ExtractorController> _logger;

        public ExtractorController(PdfConverter pdfConverter, ILogger<ExtractorController> logger) {
            this.pdfConverter = pdfConverter;
            _logger = logger;
        }


        [HttpPost("Image", Name = "Image")]
        public async Task<Ticket> FromImage([FromRoute] string shop, IFormFile pdfFile) {
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath)) {
                await pdfFile.CopyToAsync(stream);
            }
            var s = Stopwatch.StartNew();

            var ocr = new Ocr();

            var capraboFixer = new CapraboImageFixer();

            var fixedImagePath = capraboFixer.Fix(filePath);
            System.IO.File.Delete(filePath);

            var result = await ocr.Scan(fixedImagePath);

            System.IO.File.Delete(fixedImagePath);

            var capraboParser = new CapraboTicketParser();

            var ticket = capraboParser.Parse(result);
            s.Stop();

            return ticket;

        }

        [HttpPost("PDF", Name = "PDF")]
        public async Task<Ticket> FromPdf([FromRoute] string shop, IFormFile pdfFile) {
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath)) {
                await pdfFile.CopyToAsync(stream);
            }

            var result = await pdfConverter.ToString(filePath);

            System.IO.File.Delete(filePath);

            var capraboParser = new CapraboTicketParser();

            var ticket = capraboParser.Parse(result);

            return ticket;

        }


        [HttpPost("PDF2", Name = "PDF2")]
        public async Task<Ticket> FromPdf2([FromRoute] string shop, IFormFile pdfFile) {
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath)) {
                await pdfFile.CopyToAsync(stream);
            }

            var pdfImageFilePath = await pdfConverter.ToImage(filePath);

            System.IO.File.Delete(filePath);


            var imageFixer = new CapraboImageFixer();

            var pdfImageFixedFilePath = imageFixer.Fix(pdfImageFilePath);

            System.IO.File.Delete(pdfImageFilePath);

            var ocr = new Ocr();

            var result = await ocr.Scan(pdfImageFixedFilePath);

            System.IO.File.Delete(pdfImageFilePath);

            var capraboParser = new CapraboTicketParser();

            var ticket = capraboParser.Parse(result);

            return ticket;

        }

        [HttpPost("PDFText", Name = "PDFText")]
        public async Task<string> PDFText([FromRoute] string shop, IFormFile pdfFile) {
            var filePath = Path.GetTempFileName();

            using (var stream = System.IO.File.Create(filePath)) {
                await pdfFile.CopyToAsync(stream);
            }

            var pdfImageFilePath = await pdfConverter.ToImage(filePath);

            System.IO.File.Delete(filePath);


            var imageFixer = new CapraboImageFixer();

            var pdfImageFixedFilePath = imageFixer.Fix(pdfImageFilePath);

            System.IO.File.Delete(pdfImageFilePath);

            var ocr = new Ocr();

            var result = await ocr.Scan(pdfImageFixedFilePath);

            System.IO.File.Delete(pdfImageFilePath);

            return result;

        }

    }


}