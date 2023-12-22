using Main.Enumerators;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Main.Classes
{
    static class FileSystem
    {
        static public void NullData(ref Canvas canv, ref AdjacenceList list)
        {
            var @new = new AdjacenceList(list.Type).GetList;
            list.GetList = @new;
            canv.Children.Clear();
        }

        static public void Save(Canvas canv, GraphType type)
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "GraphCanvas"; // Default file name
            if (type == GraphType.Undirected)
            {
                dlg.DefaultExt = ".cogu"; // Default file extension
                dlg.Filter = "Файли неорієнтованого графа (*.cogu)|*.cogu|All files (*.*)|*.*";

            }
            else if (type == GraphType.Directed)
            {
                dlg.DefaultExt = ".cogd"; // Default file extension
                dlg.Filter = "Файли орієнтованого графа (*.cogd)|*.cogd|All files (*.*)|*.*";

            }
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();
            string filename = dlg.FileName;

            // Process save file dialog box results
            if (result == true)
            {
                FileStream fs = File.Open(dlg.FileName, FileMode.Create);
                XamlWriter.Save(canv, fs);
                fs.Close();
            }
        }

    }

    internal static class UnsafeNative
    {
        public const int WM_COPYDATA = 0x004A;

        public static string GetMessage(int message, IntPtr lParam)
        {
            if (message == UnsafeNative.WM_COPYDATA)
            {
                try
                {
                    var data = Marshal.PtrToStructure<UnsafeNative.COPYDATASTRUCT>(lParam);
                    var result = string.Copy(data.lpData);
                    return result;
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public static void SendMessage(IntPtr hwnd, string message)
        {
            var messageBytes = Encoding.Unicode.GetBytes(message); /* ANSII encoding */
            var data = new UnsafeNative.COPYDATASTRUCT
            {
                dwData = IntPtr.Zero,
                lpData = message,
                cbData = messageBytes.Length + 1 /* +1 because of \0 string termination */
            };

            if (UnsafeNative.SendMessage(hwnd, WM_COPYDATA, IntPtr.Zero, ref data) != 0)
                throw new Win32Exception(Marshal.GetLastWin32Error());
        }

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("User32.dll", EntryPoint = "SendMessage")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, ref COPYDATASTRUCT lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct COPYDATASTRUCT
        {
            public IntPtr dwData;
            public int cbData;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string lpData;
        }
    }

}
