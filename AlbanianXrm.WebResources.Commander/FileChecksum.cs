using System;
using System.IO;

namespace AlbanianXrm.WebResources
{
    internal class FileChecksum
    {
        public static string GetSHA1Checksum(string filename)
        {
            using (var stream = File.OpenRead(filename))
            {
                return GetSHA1Checksum(stream);
            }
        }

        public static string GetSHA1Checksum(Stream stream)
        {
            using (var md5 = System.Security.Cryptography.SHA1.Create())
            {
                var hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "");
            }
        }
    }
}
