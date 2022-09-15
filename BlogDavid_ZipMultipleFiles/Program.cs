using System;
using System.IO.Compression;

namespace BlogDavid_ZipMultipleFiles
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (File.Create("a.txt")) { }
            using (File.Create("b.txt")) { }
            using (File.Create("c.txt")) { }


            ZipMultipleFiles(
                    new List<FileInfo> {
                    new FileInfo("a.txt"),
                    new FileInfo("b.txt"),
                    new FileInfo("c.txt")
                    },
                    "zipped.zip");
        }


        public static void ZipMultipleFiles(IEnumerable<FileInfo> files, string zipPath)
        {
            using (var archiveStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create, true))
                {
                    // iterating through files, if doesn't exists, skip that file
                    foreach (var file in files)
                    {
                        if (!file.Exists)
                            continue;

                        // creating place for the file in the zip
                        var zipArchiveEntry = archive.CreateEntry(file.Name, CompressionLevel.Fastest);

                        using (var zipStream = zipArchiveEntry.Open())
                        using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                            // copying the file's content into the zip
                            fileStream.CopyTo(zipStream);
                    }
                }
                // stream needs to be seeked back to the beginning
                archiveStream.Seek(0, SeekOrigin.Begin);

                // writing the whole zip to a new file
                var zipFile = new FileInfo(zipPath);
                using (var fileStream = zipFile.Create())
                    archiveStream.CopyTo(fileStream);
            }
        }
    }
}

