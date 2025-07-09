using MelonLoader;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniverseLib.Input;

namespace RushResetKey
{
    public class RushResetKey : MelonMod
    {
        internal static Game Game { get; private set; }
        internal static MainMenu MainMenu { get; private set; }
        private static bool Resetting = false;
        private static bool LockedOut = false;
        public override void OnLateInitializeMelon()
        {
            Game = Singleton<Game>.Instance;
            Game.OnInitializationComplete += OnInitComplete;
            Game.OnLevelLoadComplete += OnLevelLoadComplete;
            Settings.Register();
        }
        void OnInitComplete()
        {
            MainMenu = MainMenu.Instance();
        }
       
        public static void SigmaReset()
        {
            if (LevelRush.IsLevelRush() && Settings.Enabled.Value && !LockedOut)
            {
                MainMenu.PauseGameNoStateChange(false);
                Singleton<Audio>.Instance.StopMusic(0f);
                LevelRush.ClearLevelRushStats();
                LevelRush.PlayCurrentLevelRushMission();
                Resetting = true;
            }
        }
        public override void OnUpdate()
        {
            if (InputManager.GetKeyDown(Settings.ResetKey.Value) && !Resetting)
                SigmaReset();
            else
                Resetting = false;

        }

        private static void OnLevelLoadComplete()
        {
            if (SceneManager.GetActiveScene().name.Equals("Heaven_Environment"))
            {
                return;
            }

            LockedOut = (LevelRush.GetCurrentLevelRushLevelIndex() + 1) >= Settings.LevelLockout.Value && Settings.LevelLockout.Value != 0;


        }
        public static class Settings {
            public static MelonPreferences_Category Category;
            public static MelonPreferences_Entry<bool> Enabled;
            public static MelonPreferences_Entry<KeyCode> ResetKey;
            public static MelonPreferences_Entry<int> LevelLockout;

            public static void Register()
            {
                Category = MelonPreferences.CreateCategory("RushResetKey");
                Enabled = Category.CreateEntry("Enabled", true);
                ResetKey = Category.CreateEntry("Reset Key", KeyCode.Semicolon, description: "Pressing the assigned key will reset the current level rush.");
                LevelLockout = Category.CreateEntry("Level Lockout ", 0, description: "Disables Rush Reset once you reach level 'X' in the rush (Enter 0 to always allow).");
            }

        }
    }
}

