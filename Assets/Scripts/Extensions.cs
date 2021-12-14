using System;
using System.IO;
using UnityEngine;

public static class Extensions {
    public static Color Invert(this Color color) {
        return new Color(1 - color.r, 1 - color.g, 1 - color.b, color.a);
    }

    // extension without '.'
    public static long CountFilesOfType(this DirectoryInfo d, string extension) {
        long i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis) {
            if (fi.Extension.Equals("." + extension, StringComparison.OrdinalIgnoreCase))
                i++;
        }
        return i;
    }
}
