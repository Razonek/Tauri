using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tauri
{
    public class Bunnyhop : Game
    {
        Thread _bunnyhopThread;
        private bool _turned;
        private int _key;     

        public Bunnyhop()
        {
            TauriViewModel.AppClosing += AbortThread;
            TauriViewModel.SwitchFunction += SwitchFunction;
            TauriViewModel.SetBunnyhopKey += SetKey;
            

        }


        /// <summary>
        /// On closing app
        /// </summary>
        private void AbortThread()
        {
            if(_turned)
            {
                _bunnyhopThread.Abort();
            }            
        }

        /// <summary>
        /// Toggle Bunnyhop function
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        private void SwitchFunction(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.Bunnyhop)
            {
                _turned = value;
                if(value)
                {
                    _bunnyhopThread = new Thread(BunnyhopLoop);
                    _bunnyhopThread.Start();
                }
            }
        }
        
        /// <summary>
        /// Get virtual key code to assign hotkey
        /// </summary>
        /// <param name="key"> Virtual key code </param>
        private void SetKey(int key)
        {
            _key = key;
        }

        /// <summary>
        /// Get player position
        /// </summary>
        /// <returns>257 - on ground, 256 in air</returns>
        private int GetPositionStatement()
        {
            int _player = Read(Injector.baseAddressClientDLL + m_dwLocalPlayer);
            int _position = Read(_player + m_fFlags);
            return _position;
        }

        /// <summary>
        /// Bunnyhop
        /// </summary>
        private void BunnyhopLoop()
        {
            while (_turned)
            {
                if (KeyboardControl.GetAsyncKeyState(_key) != 0 && GetPositionStatement() == 257)
                {
                    KeyboardControl.keybd_event(0x20, 0x39, 0, 0);
                    Thread.Sleep(20);
                    KeyboardControl.keybd_event(0x20, 0x39, 0x02, 0);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }



    }
}
