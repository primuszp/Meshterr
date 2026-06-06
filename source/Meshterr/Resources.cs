using System.Drawing;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System;

namespace Meshterr
{
    static class Resources
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetIconInfo(IntPtr hIcon, ref IconInfo pIconInfo);

        [DllImport("user32.dll")]
        private static extern IntPtr CreateIconIndirect(ref IconInfo pIconInfo);

        private struct IconInfo
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }
        
        static ResourceManager resources = null;

        static Resources()
        {
            resources = new ResourceManager("Meshterr.Properties.Resources", Assembly.GetCallingAssembly());
        }

        static public Cursor GetCursor(string name, int xHotSpot, int yHotSpot)
        {
            IconInfo icon = new IconInfo();
            GetIconInfo(GetBitmap(name).GetHicon(), ref icon);

            icon.xHotspot = xHotSpot;
            icon.yHotspot = yHotSpot;
            icon.fIcon = false;

            return (new Cursor(CreateIconIndirect(ref icon)));
        }

        static public Bitmap GetBitmap(string name)
        {
            return ((Bitmap)resources.GetObject(name));
        }
    }
}
