
using SkiaSharp;

namespace Home.Extractor {
    public class CapraboImageFixer {

        //https://github.com/mono/SkiaSharp/issues/2405
        public string Fix(string imageFilePath) {
            var imageWithOutExtension = Path.GetFileNameWithoutExtension(imageFilePath);

            using var imageStream = File.OpenRead(imageFilePath);

            using SKBitmap originBitmap = SKBitmap.Decode(imageStream);

            using SKBitmap bitmap = new SKBitmap(originBitmap.Width, originBitmap.Height);


            using var paint = new SKPaint();

            using SKCanvas canvas = new SKCanvas(bitmap);

            using SKPaint thinLinePaint = new SKPaint {
                Style = SKPaintStyle.Stroke,
                Color = SKColors.Black,
                StrokeWidth = 4
            };

            var r = originBitmap.RayCast(95, 792, 0, 1, SKColors.Black);


            canvas.DrawBitmap(originBitmap, 0, 0, paint);

            var priceRect = new SKRect(842, r.Y + 7, 842 + 159, r.Y + 50 + 7);

            using var copy = originBitmap.Copy(priceRect);


            using SKPaint white = new SKPaint {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White
            };

            canvas.DrawRect(priceRect, white);
            canvas.DrawBitmap(copy, priceRect.Left, priceRect.Top + 10);
            canvas.DrawRect(new SKRect(1000, priceRect.Top, 1200, priceRect.Bottom), white);

            //Uni
            //canvas.DrawLine(95, 792, 95, r.Y, thinLinePaint);
            //PricePerUnit
            //canvas.DrawLine(660, 792, 660, r.Y, thinLinePaint);
            //Price
            //canvas.DrawLine(840, 792, 840, r.Y, thinLinePaint);
            //Club
            //canvas.DrawLine(1030, 792, 1030, r.Y, thinLinePaint);


            //int y = 792;

            //var paint = new SKPaint {
            //    TextSize = 10,
            //    IsAntialias = true,
            //    Color = SKColors.Black,
            //    IsStroke = false
            //};

            //while (y < r.Y) {
            //    canvas.DrawText("asd", 93, y, paint);
            //    y += (31 + 19);
            //}

            //using var fsc = File.Create("a.png");

            //copy.Encode(fsc, SKEncodedImageFormat.Png, 100);

            var fixedImageFilePath = $"{imageWithOutExtension}Fixed.png";
            using var fs = File.Create(fixedImageFilePath);

            bitmap.Encode(fs, SKEncodedImageFormat.Png, 100);

            return $"{imageWithOutExtension}Fixed.png";

        }

    }
}
