using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Drawing;

namespace Tauri
{
    public class Glow : Game
    {

        Thread _glowThread;
        private bool _turned;
        private bool _friendlyTeam;
        private bool _enemyTeam;

        private Color _color;

        private float _friendsAlpha;
        private float _friendsRed;
        private float _friendsGreen;
        private float _friendsBlue;

        private float _enemyAlpha;
        private float _enemyRed;
        private float _enemyGreen;
        private float _enemyBlue;

        public Glow()
        {
            TauriViewModel.AppClosing += AbortThread;
            TauriViewModel.SwitchFunction += SwitchFunction;
            TauriViewModel.SwitchTeamGlowVisibility += SwitchTeamGlowVisibility;
            TauriViewModel.SetGlowColor += SetGlowColor;
            
        }
        

        /// <summary>
        /// On app closing
        /// </summary>
        private void AbortThread()
        {
            if(_turned)
            {
                _glowThread.Abort();
            }            
        }


        /// <summary>
        /// Toggle Glow function
        /// </summary>
        /// <param name="value"></param>
        /// <param name="function"></param>
        private void SwitchFunction(bool value, TauriViewModel.Function function)
        {
            if(function == TauriViewModel.Function.Glow)
            {
                _turned = value;
                if(value)
                {
                    _glowThread = new Thread(GlowLoop);
                    _glowThread.Start();
                }
            }
        }


        /// <summary>
        /// Switching glow visibility on each team
        /// </summary>
        /// <param name="value"></param>
        /// <param name="team"></param>
        private void SwitchTeamGlowVisibility(bool value, TauriViewModel.Team team)
        {
            switch(team)
            {
                case TauriViewModel.Team.enemy:
                    _enemyTeam = value;
                    break;

                case TauriViewModel.Team.friends:
                    _friendlyTeam = value;
                    break;
            }
        }


        /// <summary>
        /// Setting glow colors
        /// </summary>
        /// <param name="value"></param>
        /// <param name="team"></param>
        private void SetGlowColor(string value, TauriViewModel.Team team)
        {
            _color = ColorTranslator.FromHtml(value);
            
            switch(team)
            {
                case TauriViewModel.Team.friends:
                    _friendsAlpha = _color.A;
                    _friendsRed = _color.R;
                    _friendsGreen = _color.G;
                    _friendsBlue = _color.B;
                    break;

                case TauriViewModel.Team.enemy:
                    _enemyAlpha = _color.A;
                    _enemyRed = _color.R;
                    _enemyGreen = _color.G;
                    _enemyBlue = _color.B;
                    break;
            }
        }


        /// <summary>
        /// Glow loop
        /// </summary>
        private void GlowLoop()
        {
            while (_turned)
            {
                int _glowPointer = Read(Injector.baseAddressClientDLL + m_dwGlowObject);
                int _localPlayer = Read(Injector.baseAddressClientDLL + m_dwLocalPlayer);
                int _myTeam = Read(_localPlayer + m_iTeamNum);

                for (int i = 0; i < 32; i++)
                {
                    int _currentPlayer = Read(Injector.baseAddressClientDLL + m_dwEntityList + i * 0x10);
                    int _currentPlayerDormant = Read(_currentPlayer + m_bDormant);
                    int _currentPlayerGlowIndex = Read(_currentPlayer + m_iGlowIndex);
                    int _currentPlayerTeam = Read(_currentPlayer + m_iTeamNum);

                    if (_currentPlayerDormant == 1 || _currentPlayerTeam == 0)
                        continue;

                    if (_myTeam == _currentPlayerTeam && _friendlyTeam)
                    {
                        WriteFloat(_friendsRed / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x4);
                        WriteFloat(_friendsGreen / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x8);
                        WriteFloat(_friendsBlue / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0xC);
                        WriteFloat(_friendsAlpha / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x10);
                        WriteBool(true, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x24);
                        WriteBool(false, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x25);
                    }

                    if (_myTeam != _currentPlayerTeam && _enemyTeam)
                    {
                        WriteFloat(_enemyRed / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x4);
                        WriteFloat(_enemyGreen / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x8);
                        WriteFloat(_enemyBlue / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0xC);
                        WriteFloat(_enemyAlpha / 255, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x10);
                        WriteBool(true, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x24);
                        WriteBool(false, _glowPointer + (_currentPlayerGlowIndex * 0x38) + 0x25);
                    }

                }
            }
        }

            

    }
}
