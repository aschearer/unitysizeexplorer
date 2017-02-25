namespace SpottedZebra.UnitySizeExplorer.WPF.ViewModels.Pages
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Threading.Tasks;

    public static class FileBuilder
    {
        public static async Task<List<Tuple<string, float>>> FromStream(Stream stream)
        {
            List<Tuple<string, float>> files;
            using (var reader = new StreamReader(stream))
            {
                // Step 1: Read the raw input data and convert it into a list of <file names, file size> tuples.
                files = new List<Tuple<string, float>>();
                var startProcessing = false;
                string line;
                while (!reader.EndOfStream)
                {
                    line = await reader.ReadLineAsync();

                    if (line.Trim() == Resource.LogStartWord)
                    {
                        // This is the line before the size breakdown
                        startProcessing = true;
                        continue;
                    }

                    if (!startProcessing)
                    {
                        continue;
                    }

                    Tuple<string, float> data;
                    if (GetPathAndMegabytesFrom(line, out data))
                    {
                        files.Add(data);
                    }
                    else
                    {
                        break;
                    }
                }

                // Step 2: Sort the list of files by file name.
                files.Sort(new PathAndSizeComparer());
            }

            return files;
        }

        /// <summary>
        ///     Convert Unity editor log file size line into metadata to process.
        /// </summary>
        /// <remarks>
        ///     Data comes in like: " {size} {unit} {percent}% {path}/{file}"
        /// </remarks>
        private static bool GetPathAndMegabytesFrom(string line, out Tuple<string, float> data)
        {
            try
            {
                line = line.Trim();
                var size = line.Substring(0, line.IndexOf(' '));
                var unit = line.Substring(line.IndexOf(' ') + 1, 2);
                var megabytes = float.Parse(size, CultureInfo.InvariantCulture);
                switch (unit)
                {
                    case "kb":
                        megabytes /= 1000;
                        break;
                }

                var path = line.Substring(line.IndexOf("% ", StringComparison.Ordinal) + 2);
                data = new Tuple<string, float>(path, megabytes);
                return true;
            }
            catch
            {
                data = new Tuple<string, float>(string.Empty, -1);
                return false;
            }
        }
    }
}