using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace AirVitamin.Client
{
    public class CustomFont
    {
        public static int CeraRoundPro_Bold = 0;
        public static int CeraRoundPro_Medium = 1;

        public static PrivateFontCollection GetCustomFont(byte[] fontdata)
        {
            //Create your private font collection object.
            PrivateFontCollection pfc = new PrivateFontCollection();

            //Select your font from the resources.
            int fontLength = fontdata.Length;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);

            // free up the unsafe memory
            Marshal.FreeCoTaskMem(data);

            return pfc;
        }

        public static void AddFont(PrivateFontCollection pfc, byte[] fontdata)
        {
            //Select your font from the resources.
            int fontLength = fontdata.Length;

            // create an unsafe memory block for the font data
            System.IntPtr data = Marshal.AllocCoTaskMem(fontLength);

            // copy the bytes to the unsafe memory block
            Marshal.Copy(fontdata, 0, data, fontLength);

            // pass the font to the font collection
            pfc.AddMemoryFont(data, fontLength);

            // free up the unsafe memory
            Marshal.FreeCoTaskMem(data);
        }
    }
}
