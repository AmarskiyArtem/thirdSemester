namespace MD5;

using System;
using System.Collections.Generic;
using System.Security.Cryptography;

/// <summary>
/// Static class for computing MD5 checksums
/// </summary>
public static class CheckSum
{
    /// <summary>
    /// Computes the MD5 checksum for a given file or directory
    /// </summary>
    /// <param name="path">The path to the file or directory</param>
    /// <returns>The computed MD5 checksum</returns>
    public static byte[] ComputeCheckSum(string path)
    {
        if (Directory.Exists(path))
        {
            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            Array.Sort(dirs);
            Array.Sort(files);
            var resultCheckSum = new List<byte>();
            foreach (var dir in dirs)
            {
                resultCheckSum.AddRange(ComputeCheckSum(dir));
            }
            foreach (var file in files)
            {
                resultCheckSum.AddRange(GetCheckSumFromFile(file));
            }
            var md5 = MD5.Create();
            return md5.ComputeHash(resultCheckSum.ToArray());
        }
        
        if (File.Exists(path))
        {
            return GetCheckSumFromFile(path);
        }

        throw new ArgumentException("File or directory not exist");
    }

    private static byte[] GetCheckSumFromFile(string path)
    {
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path); 
        return md5.ComputeHash(stream);
    }

    /// <summary>
    /// Computes the MD5 checksum for a directory in parallel
    /// </summary>
    /// <param name="path">The path to the directory</param>
    /// <returns>The computed MD5 checksum</returns>
    public static byte[] ComputeCheckSumParallel(string path)
    {
        if (File.Exists(path))
        {
            return GetCheckSumFromFile(path);
        }

        if (Directory.Exists(path))
        {
            var dirs = Directory.GetDirectories(path);
            var files = Directory.GetFiles(path);
            Array.Sort(dirs);
            Array.Sort(files);
            var resultCheckSumDirs = new byte[dirs.Length][];
            var resultCheckSumFiles = new byte[files.Length][];
            Parallel.For(0, resultCheckSumDirs.Length, i =>
            {
                resultCheckSumDirs[i] = ComputeCheckSumParallel(dirs[i]);
            });
            Parallel.For(0, resultCheckSumFiles.Length, i =>
            {
                resultCheckSumFiles[i] = GetCheckSumFromFile(files[i]);
            });
            var md5 = MD5.Create();
            var resultCheckSum = resultCheckSumDirs.Concat(resultCheckSumFiles).ToArray();
            return md5.ComputeHash(resultCheckSum.SelectMany(subArray => subArray).ToArray());
        }
        
        throw new ArgumentException("File or directory not exist");
    }
}
