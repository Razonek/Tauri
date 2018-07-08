using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Tauri
{
    public class Triggerbot : Game
    {


        AllowedWeapon Weapon;
        ChangeWeapon Change;
        QuickScope QuickScope;
        Thread _triggerbotLoopThread;
        private int _reactionTime;
        private bool _turned;


        public Triggerbot()
        {
            Weapon = new AllowedWeapon();
            Change = new ChangeWeapon();
            QuickScope = new QuickScope();
            TauriViewModel.AppClosing += AbortThread;
            TauriViewModel.SetTriggerbotReactionTime += SetReactionTime;
            TauriViewModel.SwitchFunction += SwitchTriggerbot;
            
        }

        /// <summary>
        /// On App closing
        /// </summary>
        private void AbortThread()
        {
            if(_turned)
            {
                _triggerbotLoopThread.Abort();
            }            
        }

        /// <summary>
        /// Set rt defined by user
        /// </summary>
        /// <param name="time"> Time in ms </param>
        private void SetReactionTime(int time)
        {
            _reactionTime = time;
        }

        /// <summary>
        /// Turn on/off triggerbot
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        private void SwitchTriggerbot(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.Triggerbot)
            {
                _turned = value;
                if(value)
                {
                    _triggerbotLoopThread = new Thread(TriggerbotLoop);
                    _triggerbotLoopThread.Start();
                }
            }
        }

        /// <summary>
        /// Detect target under crosshair
        /// </summary>
        /// <returns></returns>
        private bool DetectEnemy()
        {
            int _player = Read(Injector.baseAddressClientDLL + m_dwLocalPlayer);
            int _target = Read(_player + m_iCrossHairID);

            if(_target > 0 && _target <= 64)
            {
                int _targetAddress = Read(Injector.baseAddressClientDLL + m_dwEntityList + ((_target - 1) * 0x10));
                int _myTeam = Read(_player + m_iTeamNum);
                int _targetTeam = Read(_targetAddress + m_iTeamNum);
                int _hpValue = Read(_targetAddress + m_iHealth);

                if(_myTeam != _targetTeam && _hpValue > 0)
                {
                    return true;
                }
            }
            return false;
        }


        private void TriggerbotLoop()
        {
            while (_turned)
            {
                if (DetectEnemy() && Weapon.CheckWeaponIsAllowed())
                {
                    Thread.Sleep(_reactionTime);
                    QuickScope.MakeQuickScope();
                    MouseControl.Fire();
                    Change.ChangeWeaponIfScopeRifle();
                }
                else
                {
                    Thread.Sleep(1);
                }
            }


        }


    }
}
