using System.Collections.Generic;
using System.Threading;

namespace Tauri
{
    public class ReloadControl : Game
    {
        

        private int _lastAmmo;
        private int _lastWeapon;
        private bool _turned;

        Thread _reloadControlLoop;

        AllowedWeapon AllowedWeapon;


        public ReloadControl()
        {
            TauriViewModel.AppClosing += AbortThread;
            TauriViewModel.SwitchFunction += SwitchFunction;
            AllowedWeapon = new AllowedWeapon();            
        }


        /// <summary>
        /// On app closing
        /// </summary>
        private void AbortThread()
        {
            if(_turned)
            {
                _reloadControlLoop.Abort();
            }
            
        }


        /// <summary>
        /// Switching on/off reload control in game
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        private void SwitchFunction(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.ReloadControl)
            {
                _turned = value;
                if(value)
                {
                    _reloadControlLoop = new Thread(ReloadControlLoop);
                    _reloadControlLoop.Start();
                }
            }
        }


        private int CheckCurrentAmmo()
        {
            return Read(AllowedWeapon.GetWeaponEntity() + m_iClip);
        }


        private bool IsAllowedWeapon(List<int> disallowedWeapons)
        {
            int _currentWeapon = AllowedWeapon.GetWeaponID();
            foreach(int disallowed in disallowedWeapons)
            {
                if(_currentWeapon == disallowed)
                {
                    return false;
                }
            }
            return true;
        }



        private void ReloadControlLoop()
        {
            while (_turned)
            {
                if (IsAllowedWeapon(AllowedWeapon.shotgunsList))
                {
                    int _currentWeapon = AllowedWeapon.GetWeaponID();
                    int _currentAmmo = CheckCurrentAmmo();

                    if (_currentWeapon == _lastWeapon && _currentAmmo > _lastAmmo)
                    {
                        KeyboardControl.ChangeWeapon();
                    }
                    _lastWeapon = AllowedWeapon.GetWeaponID();
                    _lastAmmo = CheckCurrentAmmo();
                }
                Thread.Sleep(10);
            }
        }






    }
}
