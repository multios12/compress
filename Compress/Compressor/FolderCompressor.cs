namespace Compress
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>フォルダ圧縮クラス</summary>
    public class FolderCompressor : CompressorBase
    {
        /// <summary>圧縮</summary>
        public override void Compress()
        {
            var targetFiles = this.SearchFolder(this.FilePath);

            for (int imageIndex = 0; imageIndex < targetFiles.Count; imageIndex++)
            {
                var targetFile = targetFiles[imageIndex];

                using (var imagestream = new FileStream(targetFile, FileMode.Open, FileAccess.Read))
                {
                    this.SaveImageToStream(imagestream, targetFile);
                }
            }
        }

        /// <summary>
        /// 指定されたファルダから、圧縮対象となる画像ファイルを探して返す
        /// </summary>
        /// <param name="folderPath">フォルダパス</param>
        /// <returns>ファイルリスト</returns>
        private List<string> SearchFolder(string folderPath)
        {
            var targetFiles = Directory.GetFiles(folderPath, "*");
            targetFiles = targetFiles.Where((f) => this.Exts.Contains(Path.GetExtension(f).ToUpper())).ToArray();

            if (Directory.GetDirectories(folderPath).Count() == 0)
            {
                return targetFiles.ToList();
            }
            else if (targetFiles.Count() == 0 && Directory.GetDirectories(folderPath).Count() == 1)
            {
                return this.SearchFolder(Directory.GetDirectories(folderPath)[0]);
            }

            throw new Exception("フォルダ構成に異常があります。");
        }
    }
}
