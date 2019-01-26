namespace TestCompress
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [DeploymentItem(@"pngquant.exe")]
    [DeploymentItem(@"TestData\01.zip", @"TestData")]
    public class ProgramTest
    {
        PrivateType program = new PrivateType(typeof(Compress.Program));

        [TestInitialize]
        public void Initialize()
        {
            Directory.CreateDirectory(@"Result");
        }

        [TestMethod]
        public void Main_ok_zip()
        {
            var files = Copy01ZipTo("Main_ok");
            Compress.Program.Main(new string[] { files[0], "-d", @"Result" });
            Assert.IsTrue(File.Exists(files[0]));
            Assert.IsTrue(File.Exists(files[1]));
        }

        [TestMethod]
        public void Main_ok_zip_optionRemove()
        {
            var files = Copy01ZipTo("Main_ok_optionRemove");
            Compress.Program.Main(new string[] { files[0], "-d", @"Result", "-r" });
            Assert.IsFalse(File.Exists(files[0]));
            Assert.IsTrue(File.Exists(files[1]));
        }

        [TestMethod]
        public void Main_ok_Folder()
        {
            var files = Copy01FolderTo("Main_ok_Folder");
            Compress.Program.Main(new string[] { files[0], "-d", @"Result" });
            Assert.IsTrue(Directory.Exists(files[0]));
            Assert.IsTrue(File.Exists(files[1]));
        }

        [TestMethod]
        public void Main_ok_Folder_optionRemove()
        {
            var files = Copy01ZipTo("Main_ok_Folder_optionRemove");
            Compress.Program.Main(new string[] { files[0], "-d", @"Result", "-r" });
            Assert.IsFalse(File.Exists(files[0]));
            Assert.IsTrue(File.Exists(files[1]));
        }

        //[TestMethod]
        //public void Main_Exception()
        //{
        //    var args = new string[] { "filenotfound.zip", "-d", @"Result", "-r" };
        //    Assert.ThrowsException(Compress.Program.Main, "",args);
        //}
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
        #endregion

        private string[] Copy01ZipTo(string filename)
        {
            File.Copy(@"TestData\01.zip", $"TestData\\{filename}.zip");
            return new string[] { $"TestData\\{filename}.zip", $"Result\\{filename}.zip"};
        }

        private string[] Copy01FolderTo(string filename)
        {
            ZipFile.ExtractToDirectory(@"TestData\01.zip", $"TestData\\{filename}");
            return new string[] { $"TestData\\{filename}", $"Result\\{filename}.zip" };
        }
    }
}
