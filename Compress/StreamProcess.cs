namespace Compress
{
    using System;
    using System.Diagnostics;
    using System.IO;

    public class StreamProcess : Process
    {
        public Stream Execute(string exe, string args, Stream sourceStream)
        {
            try
            {
                this.StartInfo = new ProcessStartInfo(exe, args)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true
                };

                this.Start();

                using (Stream s = this.StandardInput.BaseStream)
                {
                    sourceStream.CopyTo(s);
                }

                using (Stream s = this.StandardOutput.BaseStream)
                {
                    var distStream = new MemoryStream();
                    s.CopyTo(distStream);
                    distStream.Position = 0;
                    return distStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"■Archiver側のエラー\r\n{ex.Message}\r\n{ex.StackTrace}");
                Console.WriteLine($"■{exe}側のエラー\r\n{this.StandardError.ReadToEnd()}");
                throw ex;
            }
        }
    }
}