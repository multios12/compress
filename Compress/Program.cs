namespace Compress
{
    using System;
    using System.IO;
    using System.Linq;
    using McMaster.Extensions.CommandLineUtils;
    using SharpCompress.Common;
    using SharpCompress.Writers;

    [SuppressDefaultHelpOption]
    public class Program
    {
        [Argument(0, "targetPath", "圧縮対象ファイル/フォルダ")]
        public string FilePath { get; }

        [Option("-d|--distPath", "出力先フォルダ", CommandOptionType.SingleValue)]
        public string DistFolderPath { get; }

        [Option("-r|--remove", "圧縮に成功した場合、元ファイル/フォルダを削除する", CommandOptionType.NoValue)]
        public bool Remove { get; }

        ///// <summary>
        ///// プログラムのエントリポイント
        ///// </summary>
        ///// <param name="args">コマンドライン引数</param>
        public static int Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        public int OnExecute()
        {
            try
            {
                string distFilePath;
                var result = ExecuteCompress(this.FilePath, this.DistFolderPath, out distFilePath);

                if (result == 0 && this.Remove)
                {
                    ExecuteRemove(this.FilePath);
                }

                Logger.WriteLine($"ok:{(this.Remove ? "r" : " ")}: {distFilePath}");

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"error: {ex.Message}\r\n{ex.StackTrace}");
                return -1;
            }
        }

        /// <summary>
        /// 圧縮実行メソッド
        /// </summary>
        /// <param name="sourcePath">圧縮元パス</param>
        /// <param name="distFolderPath">圧縮先フォルダパス</param>
        /// <param name="distFilePath">圧縮後ファイルパス</param>
        /// <returns>正常に終了した場合、0</returns>
        private static int ExecuteCompress(string sourcePath, string distFolderPath, out string distFilePath)
        {
            using (CompressorBase compressor = CompressorBase.AnalyzeSourceType(sourcePath))
            {
                compressor.Compress();

                if (compressor.Streams.Count == 0)
                {
                    Console.WriteLine("圧縮失敗: 画像ファイルが見つかりませんでした");
                    distFilePath = null;
                    return 1;
                }

                distFilePath = compressor.CreatedistFilePath(distFolderPath);

                using (Stream zipStream = File.Create(distFilePath))
                using (IWriter writer = WriterFactory.Open(zipStream, ArchiveType.Zip, CompressionType.None))
                {
                    for (int index = 0; index < compressor.Streams.Count(); index++)
                    {
                        writer.Write(compressor.FileNames[index], compressor.Streams[index], DateTime.Now);
                        compressor.Streams[index].Close();
                    }
                }

                Console.WriteLine($"圧縮成功: {distFilePath}");
                return 0;
            }
        }

        /// <summary>
        /// ファイルまたはフォルダの削除実行メソッド
        /// </summary>
        /// <param name="sourcePath">対象パス</param>
        private static void ExecuteRemove(string sourcePath)
        {
            File.SetAttributes(sourcePath, FileAttributes.Normal);
            if (File.Exists(sourcePath))
            {
                File.Delete(sourcePath);
            }
            else
            {
                DirectoryDelete(sourcePath);
            }
        }

        private static void DirectoryDelete(string sourcePath)
        {
            // ディレクトリ以外の全ファイルを削除
            string[] filePaths = Directory.GetFiles(sourcePath);
            foreach (string filePath in filePaths)
            {
                File.SetAttributes(filePath, FileAttributes.Normal);
                File.Delete(filePath);
            }

            // ディレクトリの中のディレクトリも再帰的に削除
            string[] directoryPaths = Directory.GetDirectories(sourcePath);
            foreach (string directoryPath in directoryPaths)
            {
                File.SetAttributes(directoryPath, FileAttributes.Normal);
                DirectoryDelete(directoryPath);
            }

            Directory.Delete(sourcePath);
        }
    }
}
