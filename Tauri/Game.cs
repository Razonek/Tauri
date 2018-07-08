using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml.Serialization;

namespace Tauri
{
    public class Game
    {

        private static List<Offset> _offsets;
        public static int m_fAccuracyPenalty { get; private set; }
        public static int m_iWeaponID { get; private set; }
        public static int m_zoomLevel { get; private set; }
        public static int m_iTeamNum { get; private set; }
        public static int m_flFlashMaxAlpha { get; private set; }
        public static int m_flFlashDuration { get; private set; }
        public static int m_iGlowIndex { get; private set; }
        public static int m_iHealth { get; private set; }
        public static int m_hActiveWeapon { get; private set; }
        public static int m_iCrossHairID { get; private set; }
        public static int m_bDormant { get; private set; }
        public static int m_dwLocalPlayer { get; private set; }
        public static int m_dwEntityList { get; private set; }
        public static int m_dwGlowObject { get; private set; }
        public static int m_iClip { get; private set; }
        public static int m_fFlags { get; private set; }
        public static int m_iItemDefinitionIndex { get; private set; }



        [DllImport("kernel32.dll")]
        public static extern int OpenProcess(uint dwDesiredAccess, bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern int ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] buffer, int size, int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern int WriteProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, int lpNumberOfBytesWritten);
                

        /// <summary>
        /// Load and set offsets from xml file
        /// </summary>
        public void LoadOffsets()
        {
            _offsets = new List<Offset>();
            XmlSerializer _serializer = new XmlSerializer(typeof(Offsets));
            string _path = Path.Combine(Environment.CurrentDirectory, "Offsets.xml");
            StreamReader _sr = new StreamReader(_path);

            using (TextReader _reader = new StringReader(_sr.ReadToEnd()))
            {
                var _result = (Offsets)_serializer.Deserialize(_reader);
                for(int i=0; i<_result.Offset.Count;i++)
                {
                    _offsets.Add(_result.Offset.ElementAt(i));
                }
            }

            SetOffsets(_offsets);

        }


        /// <summary>
        /// Assign offset values 
        /// </summary>
        /// <param name="listToAssign"> List of offsets </param>
        private void SetOffsets(List<Offset> listToAssign)
        {
            foreach(Offset _offset in listToAssign)
            {
                int _offsetValue;
                try
                {
                    _offsetValue = Int32.Parse(_offset.value.Remove(0, 2), System.Globalization.NumberStyles.HexNumber);
                }
                catch (FormatException)
                {
                    _offsetValue = 0;
                    MessageBox.Show("Wrong value format in Offsets.xml" + Environment.NewLine + _offset.name);
                }      
                        
                GetType().GetProperty(_offset.name).SetValue(this, _offsetValue);
                
                
            }
        }



        /// <summary>
        /// Read game memory
        /// </summary>
        /// <param name="value"> Offset </param>
        /// <returns></returns>
        public int Read(int value)
        {
            byte[] _buffer = new byte[4];
            ReadProcessMemory(Injector.gameHandle, value, _buffer, 4, 0);
            return BitConverter.ToInt32(_buffer, 0);
        }

        public short ReadShort(int value)
        {
            byte[] _buffer = new byte[4];
            ReadProcessMemory(Injector.gameHandle, value, _buffer, 4, 0);
            return BitConverter.ToInt16(_buffer, 0);
        }


        /// <summary>
        /// Writting to process memory float value
        /// </summary>
        /// <param name="value"> Value to write </param>
        /// <param name="offset"> Place to write in memory </param>
         public void WriteFloat(float value, int offset)
        {
            byte[] _value = new byte[4];
            _value = BitConverter.GetBytes(value);
            WriteProcessMemory(Injector.gameHandle, offset, _value, 4, 0);
        }


        /// <summary>
        /// Writting to process memory bool value
        /// </summary>
        /// <param name="value"> Value to write </param>
        /// <param name="offset"> Place to write in memory </param>
        public void WriteBool(bool value, int offset)
        {
            var _value = BitConverter.GetBytes(value);
            WriteProcessMemory(Injector.gameHandle, offset, _value, _value.Length, 0);
        }


        public void WriteInt(int value, int offset)
        {
            var _value = BitConverter.GetBytes(value);
            WriteProcessMemory(Injector.gameHandle, offset, _value, _value.Length, 0);
        }

    }
}
