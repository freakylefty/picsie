using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Security;

namespace Picsie.Utils {
    [SuppressUnmanagedCodeSecurity]
    internal static class NativeMethods
    {
        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        public static extern int StrCmpLogicalW(string psz1, string psz2);
    }

    public sealed class NaturalStringComparer : IComparer<string> {
        public int Compare(string a, string b) {
            return NativeMethods.StrCmpLogicalW(a, b);
        }
    }

    public sealed class NaturalFileInfoNameComparer : IComparer<FileInfo> {
        public int Compare(FileInfo a, FileInfo b) {
            return NativeMethods.StrCmpLogicalW(a.Name, b.Name);
        }
    }
}
