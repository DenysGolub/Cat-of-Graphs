using Main.Enumerators;
using Main.TestingPart;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Security.AccessControl;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Security.Cryptography;
using System.Windows.Input;
using System.IO.Packaging;

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

            // Process save file dialog box results
            if (result == true)
            {
                FileStream fs = File.Open(dlg.FileName, FileMode.Create);
                XamlWriter.Save(canv, fs);
                fs.Close();
            }
        }

        static public void Save(List<Question> questions) //For testing files
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "TestFile";
            dlg.DefaultExt = "etf";
            dlg.Filter = "редагований тестовий файл (*.etf)|*.etf";
            
            // Default file name
            // Show save file dialog box
            Nullable<bool> result = dlg.ShowDialog();

            // Process save file dialog box results
            if (result == true)
            {
                string jsonString = JsonConvert.SerializeObject(questions);

                File.WriteAllText(dlg.FileName, jsonString);
                dlg.FileName = dlg.FileName.Replace("etf", "ctf");

                byte[] encryptedBytes = EncryptStringToBytes_Aes(jsonString);
                File.WriteAllBytes(dlg.FileName, encryptedBytes);

            }
        }

        static public List<Question> Load(bool isAssociate=false, string filepath="")
        {
            if(!isAssociate)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "редагований тестовий файл (*.etf)|*.etf";

                if (openFileDialog.ShowDialog() == true)
                {
                    ObservableCollection<Question> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(File.ReadAllText(openFileDialog.FileName));

                    return new List<Question>(deserializedProduct);
                }
            }
            else if(isAssociate)
            {
                ObservableCollection<Question> deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(File.ReadAllText(filepath));

                return new List<Question>(deserializedProduct);
            }
           
            return new List<Question>();
        }

        static public void Load(ref Canvas canv, Canvas savedCanvas, ref AdjacenceList list)
        {
            NullData(ref canv, ref list);
            while (savedCanvas.Children.Count > 0)
            {
                UIElement obj = savedCanvas.Children[0];
                savedCanvas.Children.Remove(obj);

                if (obj is Line l)
                {
                    l.Name.EdgesNames(out int f_node, out int s_node);
                    list.AddEdge(f_node, s_node);
                }
                else if (obj is Shape sh && !(obj is Ellipse))
                {
                    sh.Name.EdgesNames(out int f_node, out int s_node);
                    list.AddEdge(f_node, s_node);
                }
                else if (obj is Ellipse elip)
                {
                    list.AddNode(elip.Name.SingleNodeName());
                }
                canv.Children.Add(obj); // Add to canvas
            }
            canv.InvalidateVisual();
        }

        static public void LoadTest(bool isAssociated=false, string filepath = "")
        {
            ObservableCollection<Question> deserializedProduct = null;
            if (!isAssociated)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "контрольний тестовий файл (*.ctf)|*.ctf";

                if (openFileDialog.ShowDialog() == true)
                {
                    byte[] encryptedBytes = File.ReadAllBytes(openFileDialog.FileName);
                    // Decrypt the encrypted text
                    string decryptedText = DecryptStringFromBytes_Aes(encryptedBytes);
                    deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(decryptedText);
                }
            }
            else if (isAssociated)
            {

                byte[] encryptedBytes = File.ReadAllBytes(filepath);
                // Decrypt the encrypted text
                string decryptedText = DecryptStringFromBytes_Aes(encryptedBytes);

                deserializedProduct = JsonConvert.DeserializeObject<ObservableCollection<Question>>(decryptedText);
            }

            if(deserializedProduct==null)
            {
                return;
            }

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double score = 0;
            foreach (var quest in deserializedProduct)
            {
                if (quest.QuestionsType == QuestionsType.ToGraphFromAdjacenceMatrix)
                {
                    if (new TestingPart.QuestionsAnsweringWindows.ToGraphFromAdjMatrixWin(quest).ShowDialog() == true)
                    {
                        score += quest.Points;
                    }
                }
                else if (quest.QuestionsType == QuestionsType.ToAdjacenceMatrixFromGraph)
                {
                    if (new TestingPart.QuestionsAnsweringWindows.ToAdjMatrixFromGraphWin(quest).ShowDialog() == true)
                    {
                        score += quest.Points;
                    }
                }
                else if(quest.QuestionsType == QuestionsType.ToIncidenceMatrixFromGraph)
                {
                    if (new TestingPart.QuestionsAnsweringWindows.ToIncMatrixFromGraphWin(quest).ShowDialog() == true)
                    {
                        score += quest.Points;
                    }
                }

            }
            TimeSpan elapsedTime = stopwatch.Elapsed;

            int hours = elapsedTime.Hours;
            int minutes = elapsedTime.Minutes;
            int seconds = elapsedTime.Seconds;
            stopwatch.Stop();
            MessageBox.Show($"Набрана кількість балів: {score}\nЧас виконання: {hours:D2}:{minutes:D2}:{seconds:D2}", "Результати тестування");
        }

        private static byte[] EncryptStringToBytes_Aes(string plainText, string key="+r6r+zZj9Jnuquh6UROArP/tgDyivAfpU7cKNgiSAHA=")
        {
            byte[] encrypted;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV should be the same size as the block size
                aesAlg.Mode = CipherMode.CBC; // Set the mode to CBC
                aesAlg.Padding = PaddingMode.PKCS7; // Use PKCS7 padding

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        private static string DecryptStringFromBytes_Aes(byte[] cipherText, string key = "+r6r+zZj9Jnuquh6UROArP/tgDyivAfpU7cKNgiSAHA=")
        {
            string plaintext = null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Convert.FromBase64String(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8]; // IV should be the same size as the block size
                aesAlg.Mode = CipherMode.CBC; // Set the mode to CBC
                aesAlg.Padding = PaddingMode.PKCS7; // Use PKCS7 padding

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                {
                    plaintext = srDecrypt.ReadToEnd();
                }
            }
            return plaintext;
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
