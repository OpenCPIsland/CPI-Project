using System;
using System.Runtime.InteropServices;

namespace Disney.Kelowna.Common
{
    public static class AppWindowUtil
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
        [DllImport("user32.dll")]
        private static extern bool SetWindowText(IntPtr hwnd, string lpString);

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();
#endif

#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
        // X11 functions
        [DllImport("libX11.so")]
        private static extern IntPtr XOpenDisplay(string display);

        [DllImport("libX11.so")]
        private static extern IntPtr XDefaultRootWindow(IntPtr display);

        [DllImport("libX11.so")]
        private static extern int XStoreName(IntPtr display, IntPtr window, string windowName);
#endif

#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX_ARM || UNITY_EDITOR_OSX || UNITY_EDITOR_OSX_ARM
        // P/Invoke to macOS Cocoa framework for window title manipulation
        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr NSApp();

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr NSApplicationSharedApplication();

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern IntPtr NSWindowTitle(IntPtr window);

        [DllImport("/System/Library/Frameworks/AppKit.framework/AppKit")]
        private static extern void NSWindowSetTitle(IntPtr window, string title);
#endif

        public static void SetTitle(string title)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            IntPtr foregroundWindow = GetForegroundWindow();
            SetWindowText(foregroundWindow, title);
#elif UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            if (IsWayland())
            {
                // Wayland does not expose direct APIs to set the window title
                Console.WriteLine($"Running under Wayland. Unable to change window title directly: {title}");
            }
            else
            {
                SetX11WindowTitle(title);
            }
#elif UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX_ARM || UNITY_EDITOR_OSX || UNITY_EDITOR_OSX_ARM
            SetMacOSWindowTitle(title);
#endif
        }

        private static void SetX11WindowTitle(string title)
        {
#if UNITY_STANDALONE_LINUX || UNITY_EDITOR_LINUX
            IntPtr display = XOpenDisplay(null);
            if (display == IntPtr.Zero)
            {
                Console.WriteLine("Unable to open X display");
                return;
            }

            IntPtr window = XDefaultRootWindow(display); // Modify this for specific windows
            XStoreName(display, window, title);
            Console.WriteLine($"Setting X11 window title to: {title}");
#endif
        }

        private static void SetMacOSWindowTitle(string title)
        {
#if UNITY_STANDALONE_OSX || UNITY_STANDALONE_OSX_ARM || UNITY_EDITOR_OSX || UNITY_EDITOR_OSX_ARM
            // Get the shared application instance (Cocoa App)
            IntPtr app = NSApplicationSharedApplication();

            // Get the current window (this part can be expanded for specific windows)
            IntPtr window = NSWindowTitle(app);

            // Set the title of the window
            NSWindowSetTitle(window, title);
            Console.WriteLine($"Setting macOS window title to: {title}");
#endif
        }

        private static bool IsWayland()
        {
            // Check for Wayland environment variable
            var waylandEnv = System.Environment.GetEnvironmentVariable("WAYLAND_DISPLAY");
            if (!string.IsNullOrEmpty(waylandEnv))
            {
                return true;
            }

            // Optionally check for XWayland environment variable if you are interested
            var xWaylandEnv = System.Environment.GetEnvironmentVariable("XWAYLAND_DISPLAY");
            return !string.IsNullOrEmpty(xWaylandEnv);
        }

        public static void StartCustomWindowManager()
        {
            // Placeholder for custom window manager logic if needed in the future.
        }
    }
}

