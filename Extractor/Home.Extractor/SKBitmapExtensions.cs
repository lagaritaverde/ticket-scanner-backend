using SkiaSharp;

namespace Home.Extractor {
    public static class SKBitmapExtensions {

        public static SKPoint RayCast(this SKBitmap bitmap, int x, int y, int direcctionX, int direcctionY, SKColor match) {

            bool found = false;

            while (!found) {

                x += direcctionX;
                y += direcctionY;

                if (!InBound(bitmap, x, y)) {
                    x -= direcctionX;
                    y -= direcctionY;
                    break;
                }

                var color = bitmap.GetPixel(x, y);

                found = color == match;
            }

            return new SKPoint(x, y);
        }

        public static SKBitmap Copy(this SKBitmap bitmap, SKRect rect) {

            SKBitmap imagenRecortada = new SKBitmap((int)rect.Width, (int)rect.Height);
            using (SKCanvas canvas = new SKCanvas(imagenRecortada)) {
                canvas.DrawBitmap(bitmap,  rect, new SKRect(0, 0, imagenRecortada.Width, imagenRecortada.Height));
            }

            return imagenRecortada;
        }

        private static bool InBound(SKBitmap bitmap, int x, int y) {
            return x >= 0 && y >= 0 && x < bitmap.Width && y < bitmap.Height;
        }
    }
}
