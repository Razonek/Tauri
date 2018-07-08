using System;
using System.Threading;
using System.Runtime.InteropServices;


namespace Tauri
{
    public static class MouseControl
    {

        [DllImport("user32.dll")]
        public static extern int mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, IntPtr dwExtraInfo);


        /// <summary>
        /// Make a single shot by pressing left mouse button
        /// </summary>
        public static void Fire()
        {
            mouse_event(0x002, 0, 0, 0, (IntPtr)0);
            Thread.Sleep(9);
            mouse_event(0x004, 0, 0, 0, (IntPtr)0);
            Thread.Sleep(9);
        }


        /// <summary>
        /// Zoom in sniper rifle
        /// </summary>
        public static void ZoomIn()
        {
            mouse_event(0x008, 0, 0, 0, (IntPtr)0);
            Thread.Sleep(10);
            mouse_event(0x010, 0, 0, 0, (IntPtr)0);
            Thread.Sleep(300);
        }

    }
}
