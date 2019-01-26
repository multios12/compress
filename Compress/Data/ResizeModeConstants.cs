namespace Compress.Data
{
    /// <summary>サイズ変更モード定数</summary>
    public enum ResizeModeConstants
    {
        /// <summary>指定サイズに変更する</summary>
        Fit,

        /// <summary>比率を維持して、指定サイズ以下にサイズ変更する</summary>
        RatioKeep,

        /// <summary>指定したパーセンテージに拡大する</summary>
        Percent
    }
}
