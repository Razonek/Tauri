using System.Collections.Generic;


namespace Tauri
{
    public class AllowedWeapon : Game
    {

        private bool _pistols;
        private bool _shotguns;
        private bool _SMGs;
        private bool _rifles;
        private bool _sniperRifles;

        private List<int> _pistolsList = new List<int>(new int[] { 1, 2, 3, 4, 30, 32, 36, 61, 63 });
        public List<int> shotgunsList = new List<int>(new int[] { 25, 27, 29, 35 });
        private List<int> _SMGsList = new List<int>(new int[] { 17, 19, 24, 26, 33, 34 });
        private List<int> _riflesList = new List<int>(new int[] { 7, 8, 10, 13, 16, 39, 60 });
        public List<int> sniperRiflesList = new List<int>(new int[] { 9, 11, 38, 40 });



        public AllowedWeapon()
        {
            TauriViewModel.SetAllowedWeapon += SetAllowedWeapon;
        }


        /// <summary>
        /// Check if equipped weapon is allowed to use in triggerbot
        /// </summary>
        /// <returns></returns>
        public bool CheckWeaponIsAllowed()
        {
            int _holdingWeapon = GetWeaponID();

            if (_pistols && _pistolsList.Contains(_holdingWeapon))
            {
                return true;
            }

            else if (_shotguns && shotgunsList.Contains(_holdingWeapon))
            {
                return true;
            }

            else if (_SMGs && _SMGsList.Contains(_holdingWeapon))
            {
                return true;
            }

            else if (_rifles && _riflesList.Contains(_holdingWeapon))
            {
                return true;
            }

            else if (_sniperRifles && sniperRiflesList.Contains(_holdingWeapon))
            {
                return true;
            }

            else
            {
                return false;
            }               
                        
        }


        /// <summary>
        /// Get actual hold weapon by player
        /// </summary>
        /// <returns></returns>
        public int GetWeaponID()
        {            
            return ReadShort(GetWeaponEntity() + m_iItemDefinitionIndex);
        }


        public int GetWeaponEntity()
        {
            int _player = Read(Injector.baseAddressClientDLL + m_dwLocalPlayer);
            int _active = Read(_player + m_hActiveWeapon);
            _active &= 0xFFF;
            int _weaponEntity = Read((m_dwEntityList + Injector.baseAddressClientDLL + _active * 0x10) - 0x10);
            return _weaponEntity;
        }

        /// <summary>
        /// Setting is kind of weapon allowed by user
        /// </summary>
        /// <param name="value"> Bool value </param>
        /// <param name="weapon"> Weapon type </param>
        private void SetAllowedWeapon(bool value, TauriViewModel.Weapon weapon)
        {
            switch(weapon)
            {
                case TauriViewModel.Weapon.Pistols:
                    _pistols = value;
                    break;

                case TauriViewModel.Weapon.Rifles:
                    _rifles = value;
                    break;

                case TauriViewModel.Weapon.Shotguns:
                    _shotguns = value;
                    break;

                case TauriViewModel.Weapon.SMGs:
                    _SMGs = value;
                    break;

                case TauriViewModel.Weapon.SniperRifles:
                    _sniperRifles = value;
                    break;             
            }
        }

    }
}
