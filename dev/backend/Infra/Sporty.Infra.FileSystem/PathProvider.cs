using System;
using System.Runtime.InteropServices;

namespace Sporty.Infra.FileSystem
{
    public class PathProvider : IPathProvider
    {
        private const char c_windowsSeparator = '\\';
        private const char c_unixSeparator = '/';

        public string GetOsPath(string path)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return path.Replace(c_windowsSeparator, c_unixSeparator);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return path.Replace(c_unixSeparator, c_windowsSeparator);
            }

            throw new NotSupportedException($"operating system not supported");
        }
    }
}
