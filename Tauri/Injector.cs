using System.Diagnostics;
using System.Threading;

namespace Tauri
{

    public delegate void Status(string msg);

    public class Injector : Game
    {

        private bool _injected;
        private int _handle;
        public static int baseAddressClientDLL { get; private set; }
        public static int gameHandle { get; private set; }

        public static Status InjectionStatus;

        Thread _injectorThread;
        
        /// <summary>
        /// Class constructor
        /// </summary>
        public Injector()
        {
            TauriViewModel.AppClosing += FinishThread;
            _injectorThread = new Thread(Inject);
            _injectorThread.Start();
            InjectionStatus("Game not detected");
        }


        /// <summary>
        /// Abort injector thread when app is closing
        /// </summary>
        private void FinishThread()
        {
            _injectorThread.Abort();
        }

        /// <summary>
        /// Inject into game
        /// </summary>
        private void Inject()
        {
            while(true)
            {
                if(!_injected)
                {
                    foreach(Process process in Process.GetProcesses())
                    {
                        if(process.ProcessName.Equals("csgo"))
                        {
                            _handle = OpenProcess(2035711, false, process.Id);
                            Thread.Sleep(10000);
                            foreach(ProcessModule module in process.Modules)
                            {
                                if(module.ModuleName.Equals("client.dll"))
                                {
                                    baseAddressClientDLL = (int)module.BaseAddress;
                                    gameHandle = _handle;
                                    _injected = true;
                                    InjectionStatus("Tauri is injected into game");
                                }
                            }
                        }
                    }
                }

                else
                {
                    Process[] _proc = Process.GetProcessesByName("csgo");
                    if(_proc.Length == 0)
                    {
                        _injected = false;
                        InjectionStatus("Game not detected");
                    }                        
                }

                Thread.Sleep(1000);
            }            
        }






    }
}
