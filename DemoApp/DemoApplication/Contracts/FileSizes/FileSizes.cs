namespace DemoApplication.Contracts.FileSizes
{
    public enum FileSizes
    {
        Kilo = 0,
        Mega = 1,
        Giga = 2
    }

    public static class FileSizesExtensions
    {
        public static string GetShortNameWithByte(this FileSizes fileSize)
        {
            switch (fileSize)
            {
                case FileSizes.Kilo:
                    return "KB";
                case FileSizes.Mega:
                    return "MB";
                case FileSizes.Giga:
                    return "GB";
                default:
                    throw new Exception("File size not found");
            }

        }

        public static long GetSize(this FileSizes fileSize)
        {
            switch (fileSize)
            {
                case FileSizes.Kilo:
                    return 1000;
                case FileSizes.Mega:
                    return 1000_000;
                case FileSizes.Giga:
                    return 1000_000_000;
                default:
                    throw new Exception("File size not found");
            }

        }
    }
}
