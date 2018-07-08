using System.Runtime.InteropServices;
using System.Threading;


namespace Tauri
{
    public static class KeyboardControl
    {

        [DllImport("user32.dll")]
        public static extern short GetAsyncKeyState(int vKey);

        [DllImport("user32.dll")]
        public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

        /// <summary>
        /// Change weapon by double clicking "Q"
        /// </summary>
        public static void ChangeWeapon()
        {
            keybd_event(0x51, 0x10, 0, 0);
            Thread.Sleep(20);
            keybd_event(0x51, 0x10, 0x02, 0);
            Thread.Sleep(200);
            keybd_event(0x51, 0x10, 0, 0);
            Thread.Sleep(20);
            keybd_event(0x51, 0x10, 0x02, 0);
            Thread.Sleep(1300);
        }
        
    }
}
