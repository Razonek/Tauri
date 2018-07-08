using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tauri
{
    public class ChangeWeapon : AllowedWeapon
    {


        private List<int> _scopeRifles = new List<int>(new int[] { 9, 40 });

        /// <summary>
        /// Change weapon and switch back if it is single shot sniper rifle
        /// </summary>
        public void ChangeWeaponIfScopeRifle()
        {
            foreach(int weapon in _scopeRifles)
            {
                if(weapon == GetWeaponID())
                {
                    KeyboardControl.ChangeWeapon();
                }
            }
        }

    }
}
