#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Toolbars;
using UnityEngine;

namespace Mainframe.Scripts.Game
{
    public static class ManiathonEditorButtons
    {
        private static Game _game;

        static ManiathonEditorButtons()
        {
            Game.EnableTestPlay = PlayerPrefs.GetInt("MANIATHON_ENABLE_TEST_PLAY", 0) != 0;
            Game.OverrideSettings = PlayerPrefs.GetInt("MANIATHON_OVERRIDE_SETTINGS", 0) != 0;
            Game.TestPlayDifficulty = PlayerPrefs.GetInt("MANIATHON_TEST_PLAY_DIFFICULTY", 0);
            Game.TestPlaySpeed = PlayerPrefs.GetInt("MANIATHON_TEST_PLAY_SPEED", 0);
        }
    
        [MainToolbarElement("Maniathon/Test Play", defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 0)]
        public static MainToolbarElement TestPlayButton()
        {
            var icon = EditorGUIUtility.IconContent("d_playLoopOff").image as Texture2D;
            var content = new MainToolbarContent(icon);
            var element = new MainToolbarToggle(content, Game.EnableTestPlay, (value) =>
            {
                Game.EnableTestPlay = value;
                PlayerPrefs.SetInt("MANIATHON_ENABLE_TEST_PLAY", Game.EnableTestPlay ? 1 : 0);
                PlayerPrefs.Save();
            })
            {
                displayed = true,
            };
            return element;
        }
    
        [MainToolbarElement("Maniathon/Override Settings", defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 0)]
        public static MainToolbarElement Override()
        {
            var icon = EditorGUIUtility.IconContent("d_Settings").image as Texture2D;
            var content = new MainToolbarContent(icon);
            var element = new MainToolbarToggle(content, Game.OverrideSettings, (value) =>
            {
                Game.OverrideSettings = value;
                MainToolbar.Refresh("Maniathon/Difficulty");
                MainToolbar.Refresh("Maniathon/Speed");
                PlayerPrefs.SetInt("MANIATHON_OVERRIDE_SETTINGS", Game.OverrideSettings ? 1 : 0);
                PlayerPrefs.Save();
            })
            {
                displayed = true,
            };
            return element;
        }
    
        [MainToolbarElement("Maniathon/Difficulty", defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 1)]
        public static MainToolbarElement Difficulty()
        {
            var icon = EditorGUIUtility.IconContent("d_UnityEditor.GameView").image as Texture2D;
            var content = new MainToolbarContent(icon);
            var element = new MainToolbarSlider(content, Game.TestPlayDifficulty, 0, 5, (value) =>
            {
                Game.TestPlayDifficulty = (int)value;
                PlayerPrefs.SetInt("MANIATHON_TEST_PLAY_DIFFICULTY", Game.TestPlayDifficulty);
                PlayerPrefs.Save();
            }, true)
            {
                enabled = Game.OverrideSettings
            };
            return element;
        }
    
        [MainToolbarElement("Maniathon/Speed", defaultDockPosition = MainToolbarDockPosition.Middle, defaultDockIndex = 2)]
        public static MainToolbarElement Speed()
        {
            var icon = EditorGUIUtility.IconContent("UnityEditor.AnimationWindow").image as Texture2D;
            var content = new MainToolbarContent(icon);
            var element = new MainToolbarSlider(content, Game.TestPlaySpeed, 0, 12, (value) =>
            {
                Game.TestPlaySpeed = (int)value;
                PlayerPrefs.SetInt("MANIATHON_TEST_PLAY_SPEED", Game.TestPlaySpeed);
                PlayerPrefs.Save();
            }, true)
            {
                enabled = Game.OverrideSettings
            };
            return element;
        }
    }
}
#endif