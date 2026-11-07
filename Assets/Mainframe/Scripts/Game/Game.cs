using Eflatun.SceneReference;
using Mainframe.Scripts.Game.InputProviders;
using Mainframe.Scripts.Microgame;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Mainframe.Scripts.Game
{
    public class Game : MonoBehaviour
    {
        private static Game _instance;
        [SerializeField] private MicrogameCollection debugCollection;
        private Input _input;
        private Runner _runner;
        private bool _isDoppleganger;

        internal static bool EnableTestPlay = false;
        internal static bool OverrideSettings = false;
        internal static int TestPlayDifficulty = 0;
        internal static int TestPlaySpeed = 0;
        private static MicrogameInfo _testMicrogameInfo;
        private static MicrogameCollection _testCollection;

        public static Input Input => _instance?._input;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (EnableTestPlay)
            {
                _testMicrogameInfo = ScriptableObject.CreateInstance<MicrogameInfo>();
                _testMicrogameInfo.scene = SceneReference.FromScenePath(SceneManager.GetActiveScene().path);

                _testCollection = ScriptableObject.CreateInstance<MicrogameCollection>();
                _testCollection.microgames = new[] { _testMicrogameInfo };
                _testCollection.speedUpStride = 1;
                _testCollection.campaignPlayCount = 1000000;
                
                SceneManager.LoadScene("Game");
                return;
            }

            if (_instance != null)
                return;

            var gameObject = new GameObject("DoppleGAMEger");
            var game = gameObject.AddComponent<Game>();
            DontDestroyOnLoad(gameObject);
            
            Runner.HandleAudioSources(SceneManager.GetActiveScene());

            game._isDoppleganger = true;
        }

        private void Awake()
        {
            QualitySettings.vSyncCount = 1;
            Application.targetFrameRate = 60;
            
            if (_instance == null)
                _instance = this;

            _input = new InputSystemProvider();
            _input.Setup();

            _runner = GetComponent<Runner>();
        }

        private void Start()
        {
            if (EnableTestPlay)
                _runner.Setup(_testCollection);
            else if (debugCollection != null)
                _runner.Setup(debugCollection);
            
            if (_isDoppleganger)
            {
                if (Microgame.Microgame.GetInstance())
                    Microgame.Microgame.GetInstance().Begin();
            }
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
            
            _input.Cleanup();
        }

        private void Update()
        {
            _input.Update();

            if (_isDoppleganger)
            {
                Time.timeScale = Mathf.Pow(2, TestPlaySpeed / 12.0f);
            }
            else if (OverrideSettings)
            {
                _runner.SetDifficulty(TestPlayDifficulty);
                _runner.SetSpeed(TestPlaySpeed);
            }
        }

        public static bool IsInMicrogame()
        {
            if (!_instance || _instance._isDoppleganger)
                return true;
            
            return _instance._runner.IsInMicrogame();
        }

        public static int GetScore()
        {
            return _instance._runner.GetScore();
        }

        public static int GetLives()
        {
            return _instance._runner.GetLives();
        }

        public static bool HasWonLastMicrogame()
        {
            return _instance._runner.HasWonLastMicrogame();
        }

        public static int GetDifficulty()
        {
            if (!_instance || _instance._isDoppleganger)
                return OverrideSettings ? TestPlayDifficulty : 0;
            
            return _instance._runner.GetDifficulty();
        }

        public static int GetSpeed()
        {
            if (!_instance || _instance._isDoppleganger)
                return OverrideSettings ? TestPlaySpeed : 0;

            return _instance._runner.GetSpeed();
        }

        public static void DisplayInstruction(string instruction)
        {
            if (!_instance || _instance._isDoppleganger)
                return;

            _instance._runner.DisplayInstruction(instruction);
        }

        internal static Runner.State GetState()
        {
            if (!_instance || _instance._isDoppleganger)
                return Runner.State.Microgame;

            return _instance._runner.GetState();
        }

        internal static float GetRemainingTime()
        {
            if (!_instance || _instance._isDoppleganger)
                return float.PositiveInfinity;

            return _instance._runner.GetRemainingTime();
        }
    }
}