using MelonLoader;
using UnityEngine;
using UniverseLib.Input;

namespace RushResetKey
{
    public class RushResetKey : MelonMod
    {
        internal static Game Game { get; private set; }
        internal static MainMenu MainMenu { get; private set; }
        internal static bool Resetting { get; private set; } = false;

        public override void OnLateInitializeMelon()
        {
            Game = Singleton<Game>.Instance;
            Game.OnInitializationComplete += OnInitComplete;
            Settings.Register();

        }

        void OnInitComplete()
        {
            MainMenu = MainMenu.Instance();
        }

        public static void SigmaReset()
        {
            if (LevelRush.IsLevelRush() && !RushResetKey.Resetting && Settings.Enabled.Value)
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
            if (InputManager.GetKeyDown(Settings.ResetKey.Value))
                SigmaReset();
            if (InputManager.GetKeyUp(UnityEngine.KeyCode.Semicolon))
                Resetting = false;

        }
        public static class Settings {
            public static MelonPreferences_Category Category;
            public static MelonPreferences_Entry<bool> Enabled;
            public static MelonPreferences_Entry<KeyCode> ResetKey;

            public static void Register()
            {
                Category = MelonPreferences.CreateCategory("RushResetKey");
                Enabled = Category.CreateEntry("Enabled", true);
                ResetKey = Category.CreateEntry("Reset Key", KeyCode.Semicolon, description: "Pressing the assigned key will reset the current level rush.");
            }

        }
    }
}

