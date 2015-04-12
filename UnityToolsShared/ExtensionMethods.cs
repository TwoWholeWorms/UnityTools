using System.IO;

namespace TwoWholeWorms.UnityTools.Shared
{
    
    public static class ExtensionMethods
    {
        
        public static bool IsPowerOfTwo(this ulong x)
        {
            return (x != 0) && ((x & (x - 1)) == 0);
        }

    }

}
