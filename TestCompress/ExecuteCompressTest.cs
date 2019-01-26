namespace TestCompress
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.IO;
    using System.IO.Compression;

    [TestClass]
    [DeploymentItem(@"pngquant.exe")]
    [DeploymentItem(@"TestData\01.zip", @"TestData")]
    public class ExecuteCompressTest
    {
        PrivateType program = new PrivateType(typeof(Compress.Program));

        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory(@"Result");
        }

        #region ExecuteCompressメソッド
        [TestMethod]
        public void ExecuteCompress_ok()
        {
            var files = Copy01ZipTo("ExecuteCompress_ok");
            program.InvokeStatic("ExecuteCompress", files[0], @"Result");
            Assert.IsTrue(File.Exists(files[0]));
            Assert.IsTrue(File.Exists(files[1]));
        }

        [TestMethod]
        public void ExecuteRemove_ok()
        {
            var files = Copy01ZipTo("ExecuteRemove_ok");
            program.InvokeStatic("ExecuteRemove", files[0]);
            Assert.IsFalse(File.Exists(files[0]));
        }

        [TestMethod]
        public void ExecuteRemove_ok_file_readonly()
        {
            var files = Copy01ZipTo("ExecuteRemove_ok_file_readonly");

            // 読み取り専用属性の設定
            File.SetAttributes(files[0], File.GetAttributes(files[0]) | FileAttributes.ReadOnly);

            program.InvokeStatic("ExecuteRemove", files[0]);
            Assert.IsFalse(File.Exists(files[0]));
        }

        [TestMethod]
        public void ExecuteRemove_ok_folder_readonly()
        {
            var files = Copy01FolderTo("ExecuteRemove_ok_folder_readonly");

            // 読み取り専用属性の設定
            File.SetAttributes(files[0], File.GetAttributes(files[0]) | FileAttributes.ReadOnly);

            program.InvokeStatic("ExecuteRemove", files[0]);
            Assert.IsFalse(Directory.Exists(files[0]));
        }
        #endregion

        private string[] Copy01ZipTo(string filename)
        {
            File.Copy(@"TestData\01.zip", $"TestData\\{filename}.zip");
            return new string[] { $"TestData\\{filename}.zip", $"Result\\{filename}.zip" };
        }

        private string[] Copy01FolderTo(string filename)
        {
            ZipFile.ExtractToDirectory(@"TestData\01.zip", $"TestData\\{filename}");
            return new string[] { $"TestData\\{filename}", $"Result\\{filename}.zip" };
        }
    }
}
