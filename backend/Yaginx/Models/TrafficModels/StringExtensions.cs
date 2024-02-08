namespace Yaginx.Models.TrafficModels
{
    public static class StringExtensions
    {
        public static string GetByteHumanString(this long bytes)
        {
            if (bytes == 0)
                return 0.ToString();

            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int order = 0;
            while (bytes >= 1024 && order < sizes.Length - 1)
            {
                order++;
                bytes = bytes / 1024;
            }

            // Adjust the format string to your preferences. For example "{0:0.#}{1}" would
            // show a single decimal place, and no space.
            return string.Format("{0:0.##}{1}", bytes, sizes[order]);
        }
    }
}
