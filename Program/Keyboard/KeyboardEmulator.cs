using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace Keyboard
{
    public static class KeyboardEmulator
    {
        [DllImport("user32.dll")]
        static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
        
        [DllImport("user32.dll")]
        public static extern short VkKeyScan(char ch);

        const int KEYEVENTF_KEYUP = 0x0002;

        public static void PressKey(char c)
        {
            byte vk = (byte)VkKeyScan(c);
            keybd_event(vk, 0, 0, 0); // Key down
            keybd_event(vk, 0, KEYEVENTF_KEYUP, 0); // Key up
        }

        public static async Task HoldKey(char c, TimeSpan span, CancellationToken? cts = null)
        {
            byte vk = (byte)VkKeyScan(c);
            keybd_event(vk, 0, 0, 0); // Key down
            if (cts == null)
            {
                await Task.Delay(span);
            }
            else
            {
                await Task.Delay(span, (CancellationToken)cts);
            }
            keybd_event(vk, 0, KEYEVENTF_KEYUP, 0); // Key up
        }
    }
}