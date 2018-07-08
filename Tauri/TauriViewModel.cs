using System;
using Caliburn.Micro;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Threading;

namespace Tauri
{

    public delegate void CatchedKey(int vKey, string vKeyName);
    public delegate void AppClosing();
    public delegate void SwitchFunction(bool value, TauriViewModel.Function function);
    public delegate void SwitchTeamGlowVisibility(bool value, TauriViewModel.Team team);
    public delegate void SetGlowColor(string color, TauriViewModel.Team team);
    public delegate void SetAllowedWeapon(bool value, TauriViewModel.Weapon weaponType);
    public delegate void SetTriggerbotReactionTime(int time);
    public delegate void SetBunnyhopKey(int value);

    public class TauriViewModel : Screen
    {

        /// <summary>
        /// List of names grid visibility
        /// </summary>
        private List<string> _gridList = new List<string>(new string[] 
        {   "generalGridVisibility",
            "triggerbotGridVisibility",
            "glowGridVisibility",
            "otherGridVisibility",
            "settingsGridVisibility"
        });


        /// <summary>
        /// List of menu togglebuttons "IsChecked" property
        /// </summary>
        private List<string> _toggleMenuList = new List<string>(new string[]
        {
            "isCheckedGeneralMenuButton",
            "isCheckedTriggerbotMenuButton",
            "isCheckedGlowMenuButton",
            "isCheckedOtherMenuButton",
            "isCheckedSettingsMenuButton"
        });

        /// <summary>
        /// List of hotkey setter buttons
        /// </summary>
        private List<string> _hotkeySetButtons = new List<string>(new string[]
        {
            "isCheckedTriggerbotHotkeySet",
            "isCheckedBunnyhopHotkeySet",
            "isCheckedGlowHotkeySet",
            "isCheckedNoFlashHotkeySet"
        });


        
        

        private int _triggerbotHotkey;
        private int _glowHotkey;
        private int _noFlashHotkey;

        private int _bunnyhopHotkey; 
        private int bunnyhopHotkey
        {
            get { return _bunnyhopHotkey; }
            set
            {
                _bunnyhopHotkey = value;
                SetBunnyhopKey(value);
            }
        }   
            

        public static AppClosing AppClosing;
        public static SwitchFunction SwitchFunction;
        public static SwitchTeamGlowVisibility SwitchTeamGlowVisibility;
        public static SetGlowColor SetGlowColor;
        public static SetAllowedWeapon SetAllowedWeapon;
        public static SetTriggerbotReactionTime SetTriggerbotReactionTime;
        public static SetBunnyhopKey SetBunnyhopKey;


        DispatcherTimer _hotkeyDetectionTimer;

        Injector Injector;
        Game Game;
        Bunnyhop Bunnyhop;
        NoFlash NoFlash;
        Glow Glow;
        Triggerbot Triggerbot;
        ReloadControl ReloadControl;

        private bool _settingHotkey { get; set; }


        /// <summary>
        /// ViewModel Constructor
        /// </summary>
        public TauriViewModel()
        {
            this.DisplayName = "Tauri";
            
            Injector.InjectionStatus += SetInjectionStatus;
            TauriView.SendKey += CatchPressedKey;

            Injector = new Injector();
            Game = new Game();
            Bunnyhop = new Bunnyhop();
            NoFlash = new NoFlash();
            Glow = new Glow();
            Triggerbot = new Triggerbot();
            ReloadControl = new ReloadControl();

            SetAllTurnedOff();
            GeneralMenuButton();

            _hotkeyDetectionTimer = new DispatcherTimer();
            _hotkeyDetectionTimer.Interval = new TimeSpan(0, 0, 0, 0, 30);
            _hotkeyDetectionTimer.Tick += new EventHandler(HotkeyDetection);
            _hotkeyDetectionTimer.Start();            

            
            Game.LoadOffsets();
            LoadSettings();
                                 

        }


        /// <summary>
        /// Killing all running threads when app is under closing
        /// </summary>
        /// <param name="close"></param>
        protected override void OnDeactivate(bool close)
        {
            AppClosing();
            SaveSettings();
            base.OnDeactivate(close);
        }

               

        #region Triggerbot Grid

        /// <summary>
        /// Value showed to user
        /// </summary>
        private string _triggerbotReactionTimeLabel;
        public string triggerbotReactionTimeLabel
        {
            get { return _triggerbotReactionTimeLabel; }
            private set
            {
                _triggerbotReactionTimeLabel = value;
                NotifyOfPropertyChange("triggerbotReactionTimeLabel");
            }
        }

        /// <summary>
        /// Value from slider
        /// </summary>
        private int _triggerbotReactionTimeSliderValue;
        public int triggerbotReactionTimeSliderValue
        {
            get { return _triggerbotReactionTimeSliderValue; }
            set
            {
                _triggerbotReactionTimeSliderValue = value;
                NotifyOfPropertyChange("triggerbotReactionTimeSliderValue");
                SetTriggerbotReactionTime((int)value);
                triggerbotReactionTimeLabel = value.ToString();
            }
        }

        #region Allowed weapons by user
        private bool _triggerbotPistolsUsage;
        public bool triggerbotPistolsUsage
        {
            get { return _triggerbotPistolsUsage; }
            set
            {
                _triggerbotPistolsUsage = value;
                SetAllowedWeapon(value, Weapon.Pistols);
                NotifyOfPropertyChange("triggerbotPistolsUsage");
            }
        }

        private bool _triggerbotShotgunsUsage;
        public bool triggerbotShotgunsUsage
        {
            get { return _triggerbotShotgunsUsage; }
            set
            {
                _triggerbotShotgunsUsage = value;
                SetAllowedWeapon(value, Weapon.Shotguns);
                NotifyOfPropertyChange("triggerbotShotgunsUsage");
            }
        }

        private bool _triggerbotSMGUsage;
        public bool triggerbotSMGUsage
        {
            get { return _triggerbotSMGUsage; }
            set
            {
                _triggerbotSMGUsage = value;
                SetAllowedWeapon(value, Weapon.SMGs);
                NotifyOfPropertyChange("triggerbotSMGUsage");
            }
        }

        private bool _triggerbotRiflesUsage;
        public bool triggerbotRiflesUsage
        {
            get { return _triggerbotRiflesUsage; }
            set
            {
                _triggerbotRiflesUsage = value;
                SetAllowedWeapon(value, Weapon.Rifles);
                NotifyOfPropertyChange("triggerbotRiflesUsage");
            }
        }

        private bool _triggerbotSniperRiflesUsage;
        public bool triggerbotSniperRiflesUsage
        {
            get { return _triggerbotSniperRiflesUsage; }
            set
            {
                _triggerbotSniperRiflesUsage = value;
                SetAllowedWeapon(value, Weapon.SniperRifles);
                NotifyOfPropertyChange("triggerbotSniperRiflesUsage");
            }
        }
        #endregion

        #endregion

        #region Glow Grid

        private bool _isCheckedFriendlyGlow;
        public bool isCheckedFriendlyGlow
        {
            get { return _isCheckedFriendlyGlow; }
            set
            {
                _isCheckedFriendlyGlow = value;
                NotifyOfPropertyChange("isCheckedFriendlyGlow");
                SwitchTeamGlowVisibility(value, Team.friends);
            }
        }

        private bool _isCheckedEnemyGlow;
        public bool isCheckedEnemyGlow
        {
            get { return _isCheckedEnemyGlow; }
            set
            {
                _isCheckedEnemyGlow = value;
                NotifyOfPropertyChange("isCheckedEnemyGlow");
                SwitchTeamGlowVisibility(value, Team.enemy);
            }
        }

        private string _friendlyGlowColor;
        public string friendlyGlowColor
        {
            get { return _friendlyGlowColor; }
            set
            {
                _friendlyGlowColor = value;
                NotifyOfPropertyChange("friendlyGlowColor");
                SetGlowColor(value, Team.friends);

            }
        }

        private string _enemyGlowColor;
        public string enemyGlowColor
        {
            get { return _enemyGlowColor; }
            set
            {
                _enemyGlowColor = value;
                NotifyOfPropertyChange("enemyGlowColor");
                SetGlowColor(value, Team.enemy);
            }
        }

        #endregion

        #region Other Grid

        private bool _quickScopeFunctionality;
        public bool quickScopeFunctionality
        {
            get { return _quickScopeFunctionality; }
            set
            {
                _quickScopeFunctionality = value;
                SwitchFunction(value, Function.QuickScope);
                NotifyOfPropertyChange("quickScopeFunctionality");
            }
        }

        private bool _changeWeaponAtReload;
        public bool changeWeaponAtReload
        {
            get { return _changeWeaponAtReload; }
            set
            {
                _changeWeaponAtReload = value;
                SwitchFunction(value, Function.ReloadControl);
                NotifyOfPropertyChange("changeWeaponAtReload");
            }
        }


        #endregion

        #region Settings Grid

        #region SetHotkey Methods

        public void TriggerbotSetHotkey()
        {
            if(!isCheckedTriggerbotHotkeySet)
            {
                ResetHotkeyButtons(_hotkeySetButtons, Hotkey.isCheckedTriggerbotHotkeySet);
            }
            else
            {
                isCheckedTriggerbotHotkeySet = false;
            }
            
        }

        public void GlowSetHotkey()
        {
            if(!isCheckedGlowHotkeySet)
            {
                ResetHotkeyButtons(_hotkeySetButtons, Hotkey.isCheckedGlowHotkeySet);
            }
            else
            {
                isCheckedGlowHotkeySet = false;
            }
            
        }

        public void NoFlashSetHotkey()
        {
            if(!isCheckedNoFlashHotkeySet)
            {
                ResetHotkeyButtons(_hotkeySetButtons, Hotkey.isCheckedNoFlashHotkeySet);
            }
            else
            {
                isCheckedNoFlashHotkeySet = false;
            }
            
        }

        public void BunnyhopSetHotkey()
        {
            if(!isCheckedBunnyhopHotkeySet)
            {
                ResetHotkeyButtons(_hotkeySetButtons, Hotkey.isCheckedBunnyhopHotkeySet);
            }
            else
            {
                isCheckedBunnyhopHotkeySet = false;
            }
            
        }
        #endregion

        #region Properties

        private bool _notificationsInGame;
        public bool notificationsInGame
        {
            get { return _notificationsInGame; }
            set
            {
                _notificationsInGame = value;
                NotifyOfPropertyChange("notificationsInGame");
            }
        }

        private string _triggerbotHotkeyName;
        public string triggerbotHotkeyName
        {
            get { return _triggerbotHotkeyName; }
            private set
            {
                _triggerbotHotkeyName = value;
                NotifyOfPropertyChange("triggerbotHotkeyName");
            }
        }

        private string _glowHotkeyName;
        public string glowHotkeyName
        {
            get { return _glowHotkeyName; }
            private set
            {
                _glowHotkeyName = value;
                NotifyOfPropertyChange("glowHotkeyName");
            }
        }

        private string _noFlashHotkeyName;
        public string noFlashHotkeyName
        {
            get { return _noFlashHotkeyName; }
            private set
            {
                _noFlashHotkeyName = value;
                NotifyOfPropertyChange("noFlashHotkeyName");
            }
        }

        private string _bunnyhopHotkeyName;
        public string bunnyhopHotkeyName
        {
            get { return _bunnyhopHotkeyName; }
            private set
            {
                _bunnyhopHotkeyName = value;
                NotifyOfPropertyChange("bunnyhopHotkeyName");
            }
        }

        private bool _isCheckedTriggerbotHotkeySet;
        public bool isCheckedTriggerbotHotkeySet
        {
            get { return _isCheckedTriggerbotHotkeySet; }
            private set
            {                
                _isCheckedTriggerbotHotkeySet = value;
                NotifyOfPropertyChange("isCheckedTriggerbotHotkeySet");
            }
        }

        private bool _isCheckedGlowHotkeySet;
        public bool isCheckedGlowHotkeySet
        {
            get { return _isCheckedGlowHotkeySet; }
            private set
            {                
                _isCheckedGlowHotkeySet = value;
                NotifyOfPropertyChange("isCheckedGlowHotkeySet");
            }
        }

        private bool _isCheckedNoFlashHotkeySet;
        public bool isCheckedNoFlashHotkeySet
        {
            get { return _isCheckedNoFlashHotkeySet; }
            private set
            {                
                _isCheckedNoFlashHotkeySet = value;
                NotifyOfPropertyChange("isCheckedNoFlashHotkeySet");
            }
        }

        private bool _isCheckedBunnyhopHotkeySet;
        public bool isCheckedBunnyhopHotkeySet
        {
            get { return _isCheckedBunnyhopHotkeySet; }
            private set
            {                
                _isCheckedBunnyhopHotkeySet = value;
                NotifyOfPropertyChange("isCheckedBunnyhopHotkeySet");
            }
        }

        #endregion

        #endregion

        #region Menu buttons

        /// <summary>
        /// Reaction for button click
        /// </summary>
        public void GeneralMenuButton()
        {
            ChangeContent(_gridList, Grid.generalGridVisibility);
            SetMenuButton(_toggleMenuList, MenuToggleButtons.isCheckedGeneralMenuButton);
        }

        /// <summary>
        /// Reaction for button click
        /// </summary>
        public void TriggerbotMenuButton()
        {
            ChangeContent(_gridList, Grid.triggerbotGridVisibility);
            SetMenuButton(_toggleMenuList, MenuToggleButtons.isCheckedTriggerbotMenuButton);
        }

        /// <summary>
        /// Reaction for button click
        /// </summary>
        public void GlowMenuButton()
        {
            ChangeContent(_gridList, Grid.glowGridVisibility);
            SetMenuButton(_toggleMenuList, MenuToggleButtons.isCheckedGlowMenuButton);
        }

        /// <summary>
        /// Reaction for button click
        /// </summary>
        public void OtherMenuButton()
        {
            ChangeContent(_gridList, Grid.otherGridVisibility);
            SetMenuButton(_toggleMenuList, MenuToggleButtons.isCheckedOtherMenuButton);
        }

        /// <summary>
        /// Reaction for button click
        /// </summary>
        public void SettingsMenuButton()
        {
            ChangeContent(_gridList, Grid.settingsGridVisibility);
            SetMenuButton(_toggleMenuList, MenuToggleButtons.isCheckedSettingsMenuButton);
        }

        #endregion

        #region General grid buttons

        public void TriggerbotSwitch()
        {
            isCheckedTriggerbotSwitch = !isCheckedTriggerbotSwitch;
            SwitchFunction(isCheckedTriggerbotSwitch, Function.Triggerbot);
        }

        public void GlowSwitch()
        {
            isCheckedGlowSwitch = !isCheckedGlowSwitch;
            SwitchFunction(isCheckedGlowSwitch, Function.Glow);
        }

        public void BunnyhopSwitch()
        {
            isCheckedBunnyhopSwitch = !isCheckedBunnyhopSwitch;
            SwitchFunction(isCheckedBunnyhopSwitch, Function.Bunnyhop);
        }

        public void NoFlashSwitch()
        {
            isCheckedNoFlashSwitch = !isCheckedNoFlashSwitch;
            SwitchFunction(isCheckedNoFlashSwitch, Function.NoFlash);
        }

        #endregion

        #region General grid bindings

        #region Buttons content
        private string _triggerbotSwitchContent;
        public string triggerbotSwitchContent
        {
            get { return _triggerbotSwitchContent; }
            private set
            {
                _triggerbotSwitchContent = value;
                NotifyOfPropertyChange("triggerbotSwitchContent");
            }
        }

        private string _glowSwitchContent;
        public string glowSwitchContent
        {
            get { return _glowSwitchContent; }
            private set
            {
                _glowSwitchContent = value;
                NotifyOfPropertyChange("glowSwitchContent");
            }
        }

        private string _bunnyhopSwitchContent;
        public string bunnyhopSwitchContent
        {
            get { return _bunnyhopSwitchContent; }
            private set
            {
                _bunnyhopSwitchContent = value;
                NotifyOfPropertyChange("bunnyhopSwitchContent");
            }
        }

        private string _noFlashSwitchContent;
        public string noFlashSwitchContent
        {
            get { return _noFlashSwitchContent; }
            private set
            {
                _noFlashSwitchContent = value;
                NotifyOfPropertyChange("noFlashSwitchContent");
            }
        }
        #endregion

        #region Buttons IsChecked
        private bool _isCheckedTriggerbotSwitch;
        public bool isCheckedTriggerbotSwitch
        {
            get { return _isCheckedTriggerbotSwitch; }
            private set
            {
                _isCheckedTriggerbotSwitch = value;
                NotifyOfPropertyChange("isCheckedTriggerbotSwitch");
                if(value)
                { 
                    triggerbotSwitchContent = "On";
                }
                else
                {
                    triggerbotSwitchContent = "Off";
                }
            }
        }

        private bool _isCheckedGlowSwitch;
        public bool isCheckedGlowSwitch
        {
            get { return _isCheckedGlowSwitch; }
            private set
            {
                _isCheckedGlowSwitch = value;
                NotifyOfPropertyChange("isCheckedGlowSwitch");
                if(value)
                {
                    glowSwitchContent = "On";
                }
                else
                {
                    glowSwitchContent = "Off";
                }
            }
        }

        private bool _isCheckedBunnyhopSwitch;
        public bool isCheckedBunnyhopSwitch
        {
            get { return _isCheckedBunnyhopSwitch; }
            private set
            {
                _isCheckedBunnyhopSwitch = value;
                NotifyOfPropertyChange("isCheckedBunnyhopSwitch");
                if(value)
                {
                    bunnyhopSwitchContent = "On";
                }
                else
                {
                    bunnyhopSwitchContent = "Off";
                }
            }
        }

        private bool _isCheckedNoFlashSwitch;
        public bool isCheckedNoFlashSwitch
        {
            get { return _isCheckedNoFlashSwitch; }
            private set
            {
                _isCheckedNoFlashSwitch = value;
                NotifyOfPropertyChange("isCheckedNoFlashSwitch");
                if(value)
                {
                    noFlashSwitchContent = "On";
                }
                else
                {
                    noFlashSwitchContent = "Off";
                }
            }
        }
        #endregion

        private string _injectionStatus;
        public string injectionStatus
        {
            get { return _injectionStatus; }
            private set
            {
                _injectionStatus = value;
                NotifyOfPropertyChange("injectionStatus");
            }
        }



        #endregion

        #region Grid visibility

        private Visibility _generalGridVisibility;
        public Visibility generalGridVisibility
        {
            get { return _generalGridVisibility; }
            private set
            {
                _generalGridVisibility = value;
                NotifyOfPropertyChange("generalGridVisibility");
            }
        }


        private Visibility _triggerbotGridVisibility;
        public Visibility triggerbotGridVisibility
        {
            get { return _triggerbotGridVisibility; }
            private set
            {
                _triggerbotGridVisibility = value;
                NotifyOfPropertyChange("triggerbotGridVisibility");
            }
        }

        private Visibility _glowGridVisibility;
        public Visibility glowGridVisibility
        {
            get { return _glowGridVisibility; }
            private set
            {
                _glowGridVisibility = value;
                NotifyOfPropertyChange("glowGridVisibility");
            }
        }

        private Visibility _otherGridVisibility;
        public Visibility otherGridVisibility
        {
            get { return _otherGridVisibility; }
            private set
            {
                _otherGridVisibility = value;
                NotifyOfPropertyChange("otherGridVisibility");
            }
        }

        private Visibility _settingsGridVisibility;
        public Visibility settingsGridVisibility
        {
            get { return _settingsGridVisibility; }
            private set
            {
                _settingsGridVisibility = value;
                if (value == Visibility.Visible)
                    _settingHotkey = true;
                else
                    _settingHotkey = false;
                NotifyOfPropertyChange("settingsGridVisibility");
            }
        }

        #endregion

        #region Menu togglebuttons IsChecked properties

        private bool _isCheckedGeneralMenuButton;
        public bool isCheckedGeneralMenuButton
        {
            get { return _isCheckedGeneralMenuButton; }
            private set
            {
                _isCheckedGeneralMenuButton = value;
                NotifyOfPropertyChange("isCheckedGeneralMenuButton");
            }
        }


        private bool _isCheckedTriggerbotMenuButton;
        public bool isCheckedTriggerbotMenuButton
        {
            get { return _isCheckedTriggerbotMenuButton; }
            private set
            {
                _isCheckedTriggerbotMenuButton = value;
                NotifyOfPropertyChange("isCheckedTriggerbotMenuButton");
            }
        }


        private bool _isCheckedGlowMenuButton;
        public bool isCheckedGlowMenuButton
        {
            get { return _isCheckedGlowMenuButton; }
            private set
            {
                _isCheckedGlowMenuButton = value;
                NotifyOfPropertyChange("isCheckedGlowMenuButton");
            }
        }


        private bool _isCheckedOtherMenuButton;
        public bool isCheckedOtherMenuButton
        {
            get { return _isCheckedOtherMenuButton; }
            private set
            {
                _isCheckedOtherMenuButton = value;
                NotifyOfPropertyChange("isCheckedOtherMenuButton");
            }
        }


        private bool _isCheckedSettingsMenuButton;
        public bool isCheckedSettingsMenuButton
        {
            get { return _isCheckedSettingsMenuButton; }
            private set
            {
                _isCheckedSettingsMenuButton = value;
                NotifyOfPropertyChange("isCheckedSettingsMenuButton");
            }
        }
        #endregion

        #region Enums

        /// <summary>
        /// Names of togglebuttons in menu
        /// </summary>
        private enum MenuToggleButtons
        {
            isCheckedGeneralMenuButton,
            isCheckedTriggerbotMenuButton,
            isCheckedGlowMenuButton,
            isCheckedOtherMenuButton,
            isCheckedSettingsMenuButton,
        }
        
        /// <summary>
        /// Names of grids visibility
        /// </summary>
        private enum Grid
        {
            generalGridVisibility,
            triggerbotGridVisibility,
            glowGridVisibility,
            otherGridVisibility,
            settingsGridVisibility,
        }

        /// <summary>
        /// Kind of hotkey allowed to change
        /// </summary>
        private enum Hotkey
        {
            isCheckedTriggerbotHotkeySet,
            isCheckedGlowHotkeySet,
            isCheckedNoFlashHotkeySet,
            isCheckedBunnyhopHotkeySet,
        }

        
        public enum Function
        {
            Glow,
            Triggerbot,
            NoFlash,
            Bunnyhop,
            ReloadControl,
            QuickScope,
        }

        public enum Team
        {
            friends,
            enemy,
        }

        public enum Weapon
        {
            Pistols,
            Shotguns,
            SniperRifles,
            Rifles,
            SMGs,
        }
        
        #endregion

        #region Content control

        /// <summary>
        /// Changing content by setting grids visibility
        /// </summary>
        /// <param name="gridsToReset"> List of all grids on one "field" </param>
        /// <param name="gridToShow"> Grid To show </param>
        private void ChangeContent(List<string> gridsToReset, Grid gridToShow)
        {
            
            foreach(string grid in gridsToReset)
            {
                GetType().GetProperty(grid).SetValue(this, Visibility.Hidden);                
            }

            GetType().GetProperty(gridToShow.ToString()).SetValue(this, Visibility.Visible);
                        
        }

        /// <summary>
        /// Setting highlight for pressed menu button (they are togglebuttons)
        /// </summary>
        /// <param name="buttonsToReset"> All menu buttons as string list </param>
        /// <param name="buttonToSet"> Button to highlight </param>
        private void SetMenuButton(List<string> buttonsToReset, MenuToggleButtons buttonToSet)
        {

            foreach(string button in buttonsToReset)
            {
                GetType().GetProperty(button).SetValue(this, false);
            }

            GetType().GetProperty(buttonToSet.ToString()).SetValue(this, true);

        }

        /// <summary>
        /// Setting off whole togglebuttons on startup for get valid binding names
        /// </summary>
        private void SetAllTurnedOff()
        {
            isCheckedBunnyhopSwitch = false;
            isCheckedGlowSwitch = false;
            isCheckedNoFlashSwitch = false;
            isCheckedTriggerbotSwitch = false;
        }

        /// <summary>
        /// Resetting hotkey buttons
        /// </summary>
        /// <param name="togglebuttonToReset"> List of all buttons to set false </param>
        private void ResetHotkeyButtons(List<string> togglebuttonToReset, Hotkey toSet)
        {            
            foreach (string button in togglebuttonToReset)
            {
                GetType().GetProperty(button).SetValue(this, false);
            }

            GetType().GetProperty(toSet.ToString()).SetValue(this, true);
        }
        #endregion

        /////////////////////////////////////////////////////////////

        #region Delegates

        
        /// <summary>
        /// Assign user defined hotkey to one of functionality 
        /// </summary>
        /// <param name="vKey"> virtual key</param>
        /// <param name="vKeyName"> key name </param>
        private void CatchPressedKey(int vKey, string vKeyName)
        {
            foreach (string button in _hotkeySetButtons)
            {               
                if (GetType().GetProperty(button).GetValue(this).Equals(true))
                {
                           
                    switch (button)
                    {
                        case "isCheckedBunnyhopHotkeySet":
                            bunnyhopHotkeyName = vKeyName;
                            bunnyhopHotkey = vKey;
                            break;

                        case "isCheckedTriggerbotHotkeySet":
                            triggerbotHotkeyName = vKeyName;
                            _triggerbotHotkey = vKey;
                            break;

                        case "isCheckedGlowHotkeySet":
                            glowHotkeyName = vKeyName;
                            _glowHotkey = vKey;
                            break;

                        case "isCheckedNoFlashHotkeySet":
                            noFlashHotkeyName = vKeyName;
                            _noFlashHotkey = vKey;
                            break;

                    }
                    
                    GetType().GetProperty(button).SetValue(this, false);
                    

                }
            }
        }


        /// <summary>
        /// Receiving and setting notification from Injector
        /// </summary>
        /// <param name="msg"> Message </param>
        private void SetInjectionStatus(string msg)
        {
            injectionStatus = msg;
        }

        #endregion

        #region Hotkey press checker

        /// <summary>
        /// Detecting if any hotkey is pressed to switch functionality
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HotkeyDetection(object sender, EventArgs e)
        {            
            if (KeyboardControl.GetAsyncKeyState(_triggerbotHotkey) != 0 && !_settingHotkey)
            {
                TriggerbotSwitch();
                Thread.Sleep(200);
            }

            else if (KeyboardControl.GetAsyncKeyState(_glowHotkey) != 0 && !_settingHotkey)
            {
                GlowSwitch();
                Thread.Sleep(200);
            }

            else if (KeyboardControl.GetAsyncKeyState(_noFlashHotkey) != 0 && !_settingHotkey)
            {
                NoFlashSwitch();
                Thread.Sleep(200);
            }

        }

        #endregion

        #region Save Settings

        private void SaveSettings()
        {
            Properties.Settings.Default.AllowedPistols = triggerbotPistolsUsage;
            Properties.Settings.Default.AllowedShotguns = triggerbotShotgunsUsage;
            Properties.Settings.Default.AllowedSMGs = triggerbotSMGUsage;
            Properties.Settings.Default.AllowedRifles = triggerbotRiflesUsage;
            Properties.Settings.Default.AllowedSniperRifles = triggerbotSniperRiflesUsage;
            Properties.Settings.Default.ReactionTime = triggerbotReactionTimeSliderValue;

            Properties.Settings.Default.EnemyGlow = isCheckedEnemyGlow;
            Properties.Settings.Default.FriendGlow = isCheckedFriendlyGlow;
            Properties.Settings.Default.FriendGlowColor = friendlyGlowColor;
            Properties.Settings.Default.EnemyGlowColor = enemyGlowColor;

            Properties.Settings.Default.TriggerbotHotkeyName = triggerbotHotkeyName;
            Properties.Settings.Default.GlowHotkeyName = glowHotkeyName;
            Properties.Settings.Default.NoFlashHotkeyName = noFlashHotkeyName;
            Properties.Settings.Default.BunnyhopHotkeyName = bunnyhopHotkeyName;

            Properties.Settings.Default.TriggerbotHotkeyKey = _triggerbotHotkey;
            Properties.Settings.Default.GlowHotkeyKey = _glowHotkey;
            Properties.Settings.Default.BunnyhopHotkeyKey = bunnyhopHotkey;
            Properties.Settings.Default.NoFlashHotkeyKey = _noFlashHotkey;

            Properties.Settings.Default.ReloadWeapon = changeWeaponAtReload;
            Properties.Settings.Default.QuickScope = quickScopeFunctionality;

            Properties.Settings.Default.Save();
        }

        #endregion

        #region Load Settings

        private void LoadSettings()
        {
            triggerbotPistolsUsage = Properties.Settings.Default.AllowedPistols;
            triggerbotShotgunsUsage = Properties.Settings.Default.AllowedShotguns;
            triggerbotSMGUsage = Properties.Settings.Default.AllowedSMGs;
            triggerbotRiflesUsage = Properties.Settings.Default.AllowedRifles;
            triggerbotSniperRiflesUsage = Properties.Settings.Default.AllowedSniperRifles;
            triggerbotReactionTimeSliderValue = Properties.Settings.Default.ReactionTime;

            isCheckedEnemyGlow = Properties.Settings.Default.EnemyGlow;
            isCheckedFriendlyGlow = Properties.Settings.Default.FriendGlow;
            friendlyGlowColor = Properties.Settings.Default.FriendGlowColor;
            enemyGlowColor = Properties.Settings.Default.EnemyGlowColor;

            triggerbotHotkeyName = Properties.Settings.Default.TriggerbotHotkeyName;
            glowHotkeyName = Properties.Settings.Default.GlowHotkeyName;
            noFlashHotkeyName = Properties.Settings.Default.NoFlashHotkeyName;
            bunnyhopHotkeyName = Properties.Settings.Default.BunnyhopHotkeyName;

            _triggerbotHotkey = Properties.Settings.Default.TriggerbotHotkeyKey;
            _glowHotkey = Properties.Settings.Default.GlowHotkeyKey;
            _noFlashHotkey = Properties.Settings.Default.NoFlashHotkeyKey;
            bunnyhopHotkey = Properties.Settings.Default.BunnyhopHotkeyKey;

            quickScopeFunctionality = Properties.Settings.Default.QuickScope;
            changeWeaponAtReload = Properties.Settings.Default.ReloadWeapon;

        }

        #endregion
    }
}
