
using CliWrap;
using Tesseract;

namespace Home.Extractor {
    public class PdfConverter {


        public async Task<string> ToImage(string pdfFilePath) {
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(pdfFilePath);

            var result = await Cli.Wrap("pdftoppm")
                .WithArguments(new[] { "-png", pdfFilePath, fileNameWithoutExtension })
                .ExecuteAsync();

            if (result?.ExitCode != 0) {
                throw new Exception("error pdf to image");
            }

            var imageFilePath = $"{fileNameWithoutExtension}-1.png";

            return imageFilePath;
        }


        public async Task<string> ToString(string pdfFilePath) {
            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(pdfFilePath);

            var result = await Cli.Wrap("pdftoppm")
                .WithArguments(new[] { "-png", pdfFilePath, fileNameWithoutExtension })
                .ExecuteAsync();

            if (result?.ExitCode != 0) {
                throw new Exception("error pdf to image");
            }

            var imageFilePath = $"{fileNameWithoutExtension}-1.png";

            result = await Cli.Wrap("tesseract")
              .WithArguments(new[] { "-l", "cat", imageFilePath, fileNameWithoutExtension })
              .ExecuteAsync();

            if (result?.ExitCode != 0) {
                throw new Exception("error image to text");
            }

            File.Delete(imageFilePath);

            var textFilePath = $"{fileNameWithoutExtension}.txt";
            var text = System.IO.File.ReadAllText(textFilePath);


            File.Delete(textFilePath);
            return text;
        }
    }

    public class Ocr {
        public Task<string> Scan(string imageFilePath) {

            using (var engine = new TesseractEngine(@"./tessdata", "spa", EngineMode.Default)) {
                using (var img = Pix.LoadFromFile(imageFilePath)) {
                    using (var page = engine.Process(img)) {

                        var text = page.GetText();

                        return Task.FromResult(text);
                    }
                }
            }
        }
        /*
        public async Task<string> Scan(string imageFilePath) {

            var fileNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(imageFilePath);
            var result = await Cli.Wrap("tesseract")
              .WithArguments(new[] { "-l", "cat", imageFilePath, fileNameWithoutExtension })
              .ExecuteAsync();

            if (result?.ExitCode != 0) {
                throw new Exception("error image to text");
            }

            var textFilePath = $"{fileNameWithoutExtension}.txt";
            var text = System.IO.File.ReadAllText(textFilePath);

            File.Delete(textFilePath);

            return text;
        }*/
    }
}
