using System;
using System.Windows;
using Caliburn.Micro;

namespace Tauri
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            DisplayRootViewFor<TauriViewModel>();
        }

    }
}
