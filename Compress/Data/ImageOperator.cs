namespace Compress.Data
{
    using System;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    /// <summary>画像操作クラス</summary>
    public class ImageOperator
    {
        /// <summary>サイズ変更モード</summary>
        public ResizeModeConstants ResizeMode { get; set; } = ResizeModeConstants.RatioKeep;

        public int ResizePercent { get; set; } = 100;

        public bool IsSizeChange { get; set; } = true;

        public int LimitHeight { get; set; } = 800;

        public int LimitWidth { get; set; } = 1400;

        public int Quality { get; set; } = 70;

        /// <summary>保存時の画像フォーマット</summary>
        public ImageFormat ImageFormat { get; set; } = ImageFormat.Jpeg;

        /// <summary>画像サイズ変更処理</summary>
        /// <param name="stream">ストリーム</param>
        /// <param name="resizeMode">変換モード</param>
        /// <param name="height">高さ最大値</param>
        /// <param name="width">幅最大値</param>
        /// <returns>画像ファイル</returns>
        public Bitmap ResizeBitmap(Stream stream, ResizeModeConstants resizeMode, float height, float width)
        {
            // Bitmapオブジェクトの作成
            Bitmap img = new Bitmap(stream);

            float ratioHeight;
            float ratioWidth;

            switch (resizeMode)
            {
                case ResizeModeConstants.Fit:
                    // 指定サイズへ変更
                    ratioHeight = height / img.Height;
                    ratioWidth = width / img.Width;
                    break;
                case ResizeModeConstants.RatioKeep:
                    // 比率を維持して、指定サイズ以下へ変更
                    ratioHeight = img.Height > img.Width ? height / img.Height : width / img.Width;
                    ratioWidth = img.Height > img.Width ? height / img.Height : width / img.Width;
                    break;
                default:
                    // 指定のパーセンテージ拡大する
                    ratioHeight = this.ResizePercent / 100;
                    ratioWidth = this.ResizePercent / 100;
                    break;
            }

            // 変更後の画像を格納するためのキャンパス
            var distBitmap = new Bitmap((int)(img.Width * ratioWidth), (int)(img.Height * ratioHeight));

            // 画像の縮小処理
            using (var g = Graphics.FromImage(distBitmap))
            {
                // 保管方法
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                RectangleF rect =
                    new RectangleF(0, 0, ratioWidth * img.Width, ratioHeight * img.Height);
                g.DrawImage(img, 0, 0, ratioWidth * img.Width, ratioHeight * img.Height);
            }

            return distBitmap;
        }

        /// <summary>指定されたbitmapをJpegで保存し、Byte型データとして受け渡す</summary>
        /// <param name="bmp">保存するビットマップオブジェクト</param>
        /// <param name="quality">保存する画像の品質</param>
        /// <returns>保存に成功した場合、画像をビットマップとして返す</returns>
        public Stream SaveImageToStream(Bitmap bmp, int quality)
        {
            // EncoderParameterオブジェクトを1つ格納できる
            // EncoderParametersクラスの新しいインスタンスを初期化
            // ここでは品質のみ指定するため1つだけ用意する
            var eps = new EncoderParameters(1);
            var ep = eps.Param[0] = new EncoderParameter(Encoder.Quality, quality);

            // イメージエンコーダに関する情報を取得する
            var ici = this.GetEncoderInfo(this.ImageFormat);

            // 保存する
            var memory = new MemoryStream();
            try
            {
                bmp.Save(memory, ici, eps);
                memory.Position = 0;
                return memory;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private ImageCodecInfo GetEncoderInfo(ImageFormat format)
        {
            ImageCodecInfo[] encoders = ImageCodecInfo.GetImageEncoders();

            int j = 0;
            while (j < encoders.Length)
            {
                if (encoders[j].FormatID == format.Guid)
                {
                    return encoders[j];
                }

                j += 1;
            }

            return null;
        }
    }
}
