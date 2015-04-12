using System;
using System.IO;

namespace TwoWholeWorms.UnityTools.Shared
{
    public static class Utilities
    {

        public static byte[] LoadFile(string file)
        {
            FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read);

            byte[] buffer = new byte[102400000];
            using (MemoryStream ms = new MemoryStream())
            {
                while (true)
                {
                    int read = stream.Read (buffer, 0, buffer.Length);
                    if (read <= 0)
                        return ms.ToArray();
                    ms.Write (buffer, 0, read);
                }
            }
        }

        public static byte[] ReadBytesFromFile(string file, long start, int length)
        {
            byte[] bytes = new byte[length];
            using (FileStream stream = new FileStream(file, FileMode.Open, FileAccess.Read)) {
                using (BinaryReader reader = new BinaryReader(stream)) {
                    reader.BaseStream.Seek(start, SeekOrigin.Begin);
                    reader.Read(bytes, 0, length);
                }
            }
            return bytes;
        }

        public static bool IsDirectoryWriteable(string folderPath)
        {
            try
            {
                // Attempt to get a list of security permissions from the folder. 
                // This will raise an exception if the path is read only or do not have access to view the permissions. 
//                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
//                if (ds == null) {
//                    return false;
//                }
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

    }

}

