using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Tauri
{
    /// <summary>
    /// Interaction logic for TauriView.xaml
    /// </summary>
    public partial class TauriView : Window
    {
        public TauriView()
        {
            InitializeComponent();
            KeyDown += PressedKeyCatch;
        }

        public static CatchedKey SendKey;

        private void PressedKeyCatch(object sender, KeyEventArgs e)
        {
            SendKey(KeyInterop.VirtualKeyFromKey(e.Key), e.Key.ToString());
        }


    }
}
