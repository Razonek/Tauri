

namespace Tauri
{
    public class QuickScope : AllowedWeapon
    {


        private bool _turned;

        public QuickScope()
        {
            TauriViewModel.SwitchFunction += SwitchFunction;
        }


        private void SwitchFunction(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.QuickScope)
            {
                _turned = value;
            }
        }

        /// <summary>
        /// Make quick scope if sniper rifle isnt scoped.
        /// </summary>
        public void MakeQuickScope()
        {
            foreach(int weapon in sniperRiflesList)
            {
                if(GetWeaponID() == weapon && GetZoomLevel() == 0 && _turned)
                {
                    MouseControl.ZoomIn();
                }
            }
        }


        /// <summary>
        /// Get weapon zoom level
        /// </summary>
        /// <returns> Zoom level, 0= no scoped </returns>
        private int GetZoomLevel()
        {
            int _player = Read(m_dwLocalPlayer + Injector.baseAddressClientDLL);
            int _weapon = Read(_player + m_hActiveWeapon);
            _weapon &= 0xFFF;
            int _weaponEntity = Read(Injector.baseAddressClientDLL + m_dwEntityList + (_weapon - 1) * 0x10);
            int _zoomLevel = Read(_weaponEntity + m_zoomLevel);
            return _zoomLevel;
        }

        

    }
}
