using System.Runtime.InteropServices;
using System.Text;

#region WindowsImports
// Native Windows actions imports (user32.dll)
[DllImport("user32.dll")]
static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, uint dwExtraInfo);

[DllImport("user32.dll")]
static extern short GetAsyncKeyState(int vKey);

[DllImport("user32.dll", CharSet = CharSet.Auto)]
static extern uint MapVirtualKey(uint uCode, uint uMapType);

[DllImport("user32.dll", CharSet = CharSet.Auto)]
static extern int GetKeyNameText(int lParam, StringBuilder lpString, int nSize);
#endregion

#region MouseEvents
// Mouse event actions
const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
const uint MOUSEEVENTF_LEFTUP = 0x0004;
#endregion

// Microsoft Virtual-Key Codes: https://learn.microsoft.com/en-us/windows/win32/inputdev/virtual-key-codes
const int SELECTED_KEY = 0x75; // 0x75 = F6
const uint MAPVK_VK_TO_VSC = 0x00;

bool isActive = false;
int clickInterval = 100; // Interval in ms between each click (100ms = 10 clics/sec)

Console.Title = "Basic Autoclicker";
Console.WriteLine("=== Basic Autoclicker ===");
Console.WriteLine($"Click [{GetKeyName(SELECTED_KEY)}] to ENABLE / DISABLE.");
Console.WriteLine($"Actual interval : {clickInterval}ms");
Console.WriteLine("---------------------------------------------");

while (true)
{
    if ((GetAsyncKeyState(SELECTED_KEY) & 0x8000) != 0)
    {
        isActive = !isActive; // Switching the autoclicker state

        Console.ForegroundColor = isActive ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"[STATUS] Basic Autoclicker: {(isActive ? "ENABLED" : "DISABLED")}");
        Console.ResetColor();

        // Anti-spam: waits for the user to release the key to prevent looping.
        while ((GetAsyncKeyState(SELECTED_KEY) & 0x8000) != 0)
            Thread.Sleep(10);
    }

    // If the autoclicker is active, a click is simulated at the current cursor position
    if (isActive)
    {
        // A full click requires a down (pressed) and up (released) action
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);

        // Pause between each click
        Thread.Sleep(clickInterval);
    }
    else
        Thread.Sleep(20); // Slight pause to prevent the processor from running at 100% when idle
}

#region Functions
string GetKeyName(uint virtualKeyCode)
{
    // Mapping the virtual key code to a scan code
    uint scanCode = MapVirtualKey(virtualKeyCode, MAPVK_VK_TO_VSC);
    if (scanCode == 0)
        return "Unknown";

    int lParam = (int)(scanCode << 16);

    // Getting the string name
    StringBuilder buffer = new StringBuilder(128);
    int result = GetKeyNameText(lParam, buffer, buffer.Capacity);

    return result > 0 ? buffer.ToString() : "Unknown";
}
#endregion