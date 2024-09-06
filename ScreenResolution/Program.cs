using System;
using System.Runtime.InteropServices;

class Program
{
    // Define constants for changing display settings
    private const int ENUM_CURRENT_SETTINGS = -1;
    private const int CDS_UPDATEREGISTRY = 0x01;
    private const int DISP_CHANGE_SUCCESSFUL = 0;
    
    // Import the necessary Windows API methods
    [DllImport("user32.dll")]
    private static extern int ChangeDisplaySettings(ref DEVMODE devMode, int flags);
    
    [DllImport("user32.dll")]
    private static extern bool EnumDisplaySettings(string deviceName, int modeNum, ref DEVMODE devMode);

    // Define DEVMODE struct as required by the Windows API
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct DEVMODE
    {
        private const int CCHDEVICENAME = 32;
        private const int CCHFORMNAME = 32;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHDEVICENAME)]
        public string dmDeviceName;
        public short dmSpecVersion;
        public short dmDriverVersion;
        public short dmSize;
        public short dmDriverExtra;
        public int dmFields;

        public int dmPositionX;
        public int dmPositionY;
        public int dmDisplayOrientation;
        public int dmDisplayFixedOutput;

        public short dmColor;
        public short dmDuplex;
        public short dmYResolution;
        public short dmTTOption;
        public short dmCollate;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = CCHFORMNAME)]
        public string dmFormName;
        public short dmLogPixels;
        public int dmBitsPerPel;
        public int dmPelsWidth;
        public int dmPelsHeight;
        public int dmDisplayFlags;
        public int dmDisplayFrequency;
    }

    static void Main(string[] args)
    {
        // Assert command line arguments
        if (args.Length!= 2)
        {
            Console.WriteLine("Usage: ScreenResolution.exe <width> <height>");
            return;
        }

        // Parse command line arguments
        int width;
        int height;
        if (!int.TryParse(args[0], out width) ||!int.TryParse(args[1], out height))
        {
            Console.WriteLine("Invalid width or height.");
            return;
        }

        // Validate input values
        if (width <= 0 || height <= 0)
        {
            Console.WriteLine("Width and height must be positive integers.");
            return;
        }

        // // Validate input values against maximum supported resolutions
        // if (width > 1920 || height > 1080)
        // {
        //     Console.WriteLine("Maximum supported resolution is 1920x1080.");
        //     return;
        // }

        // Clear the device name and form name fields in the DEV
        // Create a DEVMODE object to store display settings
        DEVMODE dm = new DEVMODE();
        dm.dmDeviceName = new string(new char[32]);
        dm.dmFormName = new string(new char[32]);
        dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

        // Get the current display settings
        if (EnumDisplaySettings(null, ENUM_CURRENT_SETTINGS, ref dm))
        {
            // Show current settings
            Console.WriteLine("Current Resolution: {0}x{1}, Color Depth: {2}-bit, Refresh Rate: {3}Hz",
                                      dm.dmPelsWidth, dm.dmPelsHeight, dm.dmBitsPerPel, dm.dmDisplayFrequency);
            
            // Set the desired resolution
            dm.dmPelsWidth = width;  // Set the width in pixels
            dm.dmPelsHeight = height; // Set the height in pixels
            dm.dmFields = 0x180000; // DM_PELSWIDTH | DM_PELSHEIGHT

            // Change the display settings
            int result = ChangeDisplaySettings(ref dm, CDS_UPDATEREGISTRY);

            if (result == DISP_CHANGE_SUCCESSFUL)
            {
                Console.WriteLine("Resolution changed successfully.");
            }
            else
            {
                Console.WriteLine("Failed to change resolution. Error code: " + result);
                
                Console.WriteLine("Available Screen Resolutions:");

                int modeIndex = 0;
                while (EnumDisplaySettings(null, modeIndex, ref dm))
                {
                    Console.WriteLine($"Resolution: {dm.dmPelsWidth}x{dm.dmPelsHeight}, " +
                                      $"Color Depth: {dm.dmBitsPerPel}-bit, " +
                                      $"Refresh Rate: {dm.dmDisplayFrequency}Hz");
                    modeIndex++;
                }

                if (modeIndex == 0)
                {
                    Console.WriteLine("No available screen resolutions found.");
                }
            }
        }
        else
        {
            Console.WriteLine("Unable to get display settings.");
        }
    }
}
