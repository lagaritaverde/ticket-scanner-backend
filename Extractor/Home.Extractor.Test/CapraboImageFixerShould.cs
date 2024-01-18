
using SkiaSharp;

namespace Home.Extractor.Test {
    public class CapraboImageFixerShould {


        [Fact]
        public void Fix() {
            var ticketName = "Ticket_08092023_CAPRABO";
            var imagePath = System.IO.Path.Combine("data/", ticketName + ".png");


            var fixer = new CapraboImageFixer();

            fixer.Fix(imagePath);

        }

        [Fact]
        public void Cut() {
            var ticketName = "Ticket_08092023_CAPRABO";
            var imagePath = System.IO.Path.Combine("data/", ticketName + ".png");






            //using var imageStream = File.OpenRead(imagePath);

            //using SKBitmap originBitmap = SKBitmap.Decode(imageStream);


            //842,2476 ,159,42


            //var rect = new SKRectI(842, 2476, 842+159, 2476+50);
            //var n = originBitmap.Copy(rect);

            //using var fs = File.Create("a.png");

            //n.Encode(fs, SKEncodedImageFormat.Png, 100);
        }

    }


}