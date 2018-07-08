using System.Threading;

namespace Tauri
{
    public class NoFlash : Game
    {

        Thread _noFlashThread;
        private bool _turned;

        public NoFlash()
        {
            TauriViewModel.AppClosing += AbortThread;
            TauriViewModel.SwitchFunction += SwitchFunction;            
        }

        /// <summary>
        /// On closing app
        /// </summary>
        private void AbortThread()
        {
            if(_turned)
            {
                _noFlashThread.Abort();
            }            
        }

        /// <summary>
        /// Toggle NoFlash function
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        private void SwitchFunction(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.NoFlash)
            {
                _turned = value;
                if(value)
                {
                    _noFlashThread = new Thread(NoFlashLoop);
                    _noFlashThread.Start();
                }
            }
        }

        /// <summary>
        /// No flash
        /// </summary>
        private void NoFlashLoop()
        {
            while (_turned)
            {
                int _player = Read(Injector.baseAddressClientDLL + m_dwLocalPlayer);
                int _alpha = Read(_player + m_flFlashMaxAlpha);
                if (_alpha > 0)
                {
                    WriteFloat(0.0f, _player + m_flFlashMaxAlpha);
                }
                else
                {
                    Thread.Sleep(10);
                }
            }
        }

            




    }
}
