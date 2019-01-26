namespace Compress
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;
    using System.Linq;
    using Compress.Properties;
    using Data;

    /// <summary>
    /// 圧縮ベースクラス
    /// </summary>
    public abstract partial class CompressorBase : IDisposable
    {
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        /// <summary>画像ファイルストリームリスト</summary>
        public List<Stream> Streams { get; set; } = new List<Stream>();

        /// <summary>ファイル名リスト</summary>
        public List<string> FileNames { get; set; } = new List<string>();

        /// <summary>主力元ファイルパス</summary>
        public string FilePath { get; set; }

        /// <summary>拡張子一覧</summary>
        protected string[] Exts { get; set; } = { ".JPG", ".JPEG", ".BMP", ".PNG", ".GIF" };

        public static CompressorBase AnalyzeSourceType(string sourcePath)
        {
            if (Directory.Exists(sourcePath))
            {
                return new FolderCompressor() { FilePath = sourcePath };
            }

            string[] archiveExts = { ".ZIP", ".RAR" };
            if (File.Exists(sourcePath) || archiveExts.Contains(Path.GetExtension(sourcePath).ToUpper()))
            {
                return new ArchiveCompressor() { FilePath = sourcePath };
            }

            throw new ArgumentException("未対応ファイル", sourcePath);
        }

        /// <summary>リスナーをシャットダウンして廃棄します</summary>
        /// <remarks>このコードは、破棄可能なパターンを正しく実装できるように追加されました。</remarks>
        public void Dispose()
        {
            // このコードを変更しないでください。クリーンアップ コードを上の Dispose(bool disposing) に記述します。
            this.Dispose(true);
        }

        public string CreatedistFilePath(string distFolderPath)
        {
            for (int index = 0; index < 50; index++)
            {
                var distFilePath = Path.GetFileNameWithoutExtension(this.FilePath);
                distFilePath = index == 0 ? distFilePath : $"{distFilePath}({index})";
                distFilePath = Path.Combine(distFolderPath, $"{distFilePath}.zip");

                if (!File.Exists(distFilePath))
                {
                    return distFilePath;
                }
            }

            throw new IOException("ファイルが作成できません");
        }

        public abstract void Compress();

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                }

                this?.Streams.ForEach((s) => s.Dispose());
                this.disposedValue = true;
            }
        }

        protected void SaveImageToStream(Stream stream, string filename)
        {
            Stream s;
            if (Settings.Default.IsSizeChange)
            {
                using (var pngquant = new StreamProcess())
                {
                    var imageOperator = new ImageOperator { ImageFormat = ImageFormat.Png };
                    Bitmap bitmap = imageOperator.ResizeBitmap(stream, ResizeModeConstants.RatioKeep, Settings.Default.LimitHeight, Settings.Default.LimitWidth);
                    s = imageOperator.SaveImageToStream(bitmap, 70);
                    s = pngquant.Execute("Pngquant.exe", "--speed 1 -", s);
                    filename = $"{Path.GetFileNameWithoutExtension(filename)}.png";
                }
            }
            else
            {
                s = new MemoryStream();
                stream.CopyTo(s);
            }

            this.Streams.Add(s);
            this.FileNames.Add(filename);
        }
    }
}
