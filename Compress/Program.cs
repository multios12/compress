﻿namespace Compress
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
                var result = ExecuteCompress(this.FilePath, this.DistFolderPath);
                if (result == 0 && this.Remove)
                {
                    ExecuteRemove(this.FilePath);
                }

                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"圧縮失敗: {ex.Message}\r\n{ex.StackTrace}");
                return -1;
            }
        }

        /// <summary>
        /// 圧縮実行メソッド
        /// </summary>
        /// <param name="sourcePath">圧縮元パス</param>
        /// <param name="distFolderPath">圧縮先フォルダパス</param>
        /// <returns>正常に終了した場合、0</returns>
        private static int ExecuteCompress(string sourcePath, string distFolderPath)
        {
            using (CompressorBase compressor = CompressorBase.AnalyzeSourceType(sourcePath))
            {
                compressor.Compress();

                if (compressor.Streams.Count == 0)
                {
                    Console.WriteLine("圧縮失敗: 画像ファイルが見つかりませんでした");
                    return 1;
                }

                var distFilePath = compressor.CreatedistFilePath(distFolderPath);

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
            if (File.Exists(sourcePath))
            {
                File.Delete(sourcePath);
            }
            else
            {
                Directory.Delete(sourcePath);
            }
        }
    }
}
