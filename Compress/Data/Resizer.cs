namespace Data
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    public class Resizer
    {
        /// <summary>
        /// アプリケーションのスタートアップポイント
        /// </summary>
        /// <param name="args">コマンドライン引数</param>
        /// <returns>正常に終了した場合、0</returns>
        public static int Execute(string[] args)
        {
            float height = args?.Length == 3 ? float.Parse(args[0]) : 800;
            float width = args?.Length == 3 ? float.Parse(args[1]) : 1400;
            ImageFormat imageFormat = args?.Length == 3 && args[2].ToLower() == "jpeg" ? ImageFormat.Jpeg : ImageFormat.Png;

            try
            {
                using (Stream inputStream = Console.OpenStandardInput())
                using (MemoryStream sourceStream = new MemoryStream(1024 * 1024))
                using (Stream outputStream = Console.OpenStandardOutput())
                {
                    inputStream.CopyTo(sourceStream);
                    sourceStream.Position = 0;

                    var imageOperator = new ImageOperator
                    {
                        ImageFormat = imageFormat
                    };
                    Bitmap bitmap = imageOperator.ResizeBitmap(sourceStream, ImageOperator.ResizeModeConstants.RatioKeep, height, width);
                    Stream s = imageOperator.SaveImageToStream(bitmap, 70);
                    s.CopyTo(outputStream);
                }

                return 0;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);

                return -1;
            }
        }
    }
}
