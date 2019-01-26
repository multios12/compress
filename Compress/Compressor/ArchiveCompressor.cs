namespace Compress
{
    using System.IO;
    using System.Linq;
    using SharpCompress.Archives;

    /// <summary>
    /// アーカイブファイル圧縮クラス
    /// </summary>
    /// <remarks>ZIP/RARファイルを再圧縮する/</remarks>
    public class ArchiveCompressor : CompressorBase
    {
        public override void Compress()
        {
            using (IArchive archive = ArchiveFactory.Open(this.FilePath))
            {
                var entries = archive.Entries.Where(f => this.Exts.Contains(Path.GetExtension(f.Key).ToUpper())).ToArray();
                foreach (IArchiveEntry entry in entries)
                {
                    using (var entryStream = entry.OpenEntryStream())
                    {
                        this.SaveImageToStream(entryStream, entry.Key);
                    }
                }
            }
        }
    }
}