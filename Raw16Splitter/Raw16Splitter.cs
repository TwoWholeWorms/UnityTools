/**
 * Raw16Splitter.cs
 * 
 * <summary> 
 * Splits a 16-bit RAW file into the specified number of tiles.
 * </summary>
 * 
 * <remarks>
 * Apologies for the state of the code in this file. It was written very quickly to solve a specific problem. >.<
 * </remarks>
 **/

using System;
using System.IO;
using System.Text.RegularExpressions;
using TwoWholeWorms.UnityTools.Shared;

namespace TwoWholeWorms.UnityTools.Raw16Splitter
{

    class Raw16Splitter
    {
    
        public static void Main(string[] args)
        {
            uint tileCount;
            ulong tileEdgeSize;
            string file;
            string dir;
            CheckThings(args, out tileCount, out tileEdgeSize, out file, out dir);
            ProcessFile(file, dir, tileCount, tileEdgeSize);

            Console.WriteLine("Processing complete, you should now have a stack of files in `" + dir + "`.");
        }

        public static void CheckThings(string[] args, out uint tileCount, out ulong tileEdgeSize, out string file, out string dir)
        {
            if (args.Length != 3) {
                throw new ArgumentException(string.Format("Usage: {0} <gridSize> <file> <outputDir>",
                    AppDomain.CurrentDomain.FriendlyName));
            }

            file = args[1];
            if (!File.Exists(file)) {
                Console.WriteLine("File `" + file + "` does not exist.");
            }

            FileInfo f = new FileInfo(file);
            decimal width = (decimal)Math.Sqrt(f.Length / 2.0);
            if (width % 1 != 0) {
                throw new ArgumentException(string.Format("Unable to determine file dimensions. Filesize {0}, calculated width: {1}", f.Length, width));
            }

            dir = args[2];
            if (!Directory.Exists(dir)) {
                Console.WriteLine("Dir `" + dir + "` does not exist.");
            }

            if (!Utilities.IsDirectoryWriteable(dir)) {
                Console.WriteLine("Dir `" + dir + "` is not writable.");
            }

            Regex r = new Regex(@"^(?<x>\d+)x(?<y>\d+)$");
            Match m = r.Match(args[0]);
            if (!m.Success) {
                throw new ArgumentException("Grid size must match 2x2, 3x3, 4x4, 5x5, etc.");
            }

            uint gridX = uint.Parse(m.Groups["x"].Value);
            uint gridY = uint.Parse(m.Groups["y"].Value);
            if (gridX != gridY) {
                throw new ArgumentException(string.Format("Grid must be square, eg {0}x{0} or {1}x{1}.", gridX, gridY));
            }

            if (gridX < 2) {
                throw new ArgumentException("Grid size must be at least 2x2");
            }

            tileCount = gridX;

            decimal tileEdgeSizeDec = width / gridX;
            if (tileEdgeSizeDec % 1 != 0) {
                throw new ArgumentException(string.Format("File width must be exactly divisible by grid width. {0} / {1} = {2}", width, tileCount, tileEdgeSizeDec));
            }

            tileEdgeSize = (ulong)tileEdgeSizeDec;
        }

        public static void ProcessFile(string file, string dir, uint tileCount, ulong tileEdgeSize)
        {
            ulong fileWidth = tileCount * tileEdgeSize;
            Console.WriteLine("Source file size: " + ((fileWidth * fileWidth) * 2));
            Console.WriteLine("Source file dimensions: " + fileWidth + "x" + fileWidth);
            ulong tileSizeBytes = ((tileEdgeSize * tileEdgeSize) * 2);
            Console.WriteLine("Tile file size: " + tileSizeBytes);
            Console.WriteLine("Tile file dimensions: " + tileEdgeSize + "x" + tileEdgeSize);
            ulong bytesPerFileLine = (fileWidth * 2);
            ulong bytesPerTileLine = (tileEdgeSize * 2);
            for (uint tileY = 0; tileY < tileCount; tileY++) {
                for (uint tileX = 0; tileX < tileCount; tileX++) {
                    ulong offset = (bytesPerFileLine * (tileY * tileEdgeSize)) + (bytesPerTileLine * tileX);
                    Console.WriteLine(string.Format("Tile {0}x{1} start byte: {2}", tileX, tileY, offset));

                    string tileFileName = Path.GetFileNameWithoutExtension(file) + "_" + tileX + "x" + tileY + Path.GetExtension(file);
                    Console.Write("Generating tile `" + tileFileName + "`… ");

                    byte[] tileBytes = new byte[tileSizeBytes];

                    // tileByteY < tileEdgeSize NOT tileByteY < bytesPerTileLine, because you double ONLY the width for Int16, not the height as well
                    for (uint tileByteY = 0; tileByteY < tileEdgeSize; tileByteY++) {
                        long startPos = (long)(offset + (tileByteY * bytesPerFileLine));
                        byte[] readBytes = Utilities.ReadBytesFromFile(file, startPos, (int)bytesPerTileLine);
                        long tileBytePos = (long)(tileByteY * bytesPerTileLine);
                        for (int i = 0; i < readBytes.Length; i++) {
                            try {
                                tileBytes[tileBytePos + i] = readBytes[i];
                            } catch (Exception e) {
                                Console.WriteLine(tileBytes.Length);
                                Console.WriteLine(tileBytePos);
                                Console.WriteLine(tileBytePos + i);
                                Console.WriteLine(readBytes.Length);
                                Console.WriteLine(i);

                                throw e;
                            }
                        }
                    }

                    using (FileStream f = new FileStream(dir + Path.DirectorySeparatorChar + tileFileName, FileMode.OpenOrCreate, FileAccess.Write)) {
                        f.Write(tileBytes, 0, tileBytes.Length);
                        f.Close();
                    }

                    Console.WriteLine("Done.");
                }
            }
        }

    }

}
