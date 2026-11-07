using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Mainframe.Scripts.Game
{
    public class Runner : MonoBehaviour
    {
        public enum State
        {
            Intermission,
            BossIntermission,
            StartMicrogame,
            Microgame,
            EndMicrogame,
            SpeedUp,
        }

        [SerializeField] private Camera camera;
        [SerializeField] private AudioListener listener;
        [SerializeField] private GameObject lights;

        [SerializeField] internal UnityEvent onMicrogameBegin;
        [SerializeField] internal UnityEvent onMicrogameFinished;
        [SerializeField] internal UnityEvent onIntermissionBegin;
        [SerializeField] internal UnityEvent<string> onSetInstruction;
        [SerializeField] internal UnityEvent<bool> onInstructionVisibility;
        [SerializeField] internal UnityEvent<bool> onRoundVisibility;
        [SerializeField] internal UnityEvent<bool> onOverlayVisibility;
        
        private State _state;
        private MicrogameCollection _collection;
        private readonly List<int> _availableMicrogames = new ();
        private Scene _gameScene;
        private Scene _microgameScene;

        private float _timer;
        private float _instructionTimer;
        private int _lives;
        private int _score;
        private int _speed;
        private int _difficulty;
        private int _currentMicrogameIndex;
        private Microgame.Microgame _currentMicrogame;
        private AsyncOperation _sceneLoadOperation;
        private bool _hasCheered;
        private bool _running;
        private bool _shouldSpeedUp;
        private bool _lastMicrogameSuccess;
        private bool _hasDisplayedInstruction;

        internal void Setup(MicrogameCollection collection)
        {
            _gameScene = SceneManager.GetActiveScene();
            _collection = collection;
            _availableMicrogames.Clear();

            _lives = 4;
            _score = 0;
            _currentMicrogameIndex = -1;
            _lastMicrogameSuccess = true;
            _state = EnterStateIntermission();
            _timer = 3f;
            _running = true;
            
            Time.timeScale = 0;
        }

        private void Update()
        {
            if (!_running)
                return;

            if (_state == State.Microgame)
                UpdateMicrogameTimer();
            else
                UpdateTimer();
            UpdateState();
        }

        private void UpdateTimer()
        {
            _timer = Mathf.Max(_timer - Time.unscaledDeltaTime * Mathf.Pow(2, _speed / 12.0f), 0);
        }
        
        private void UpdateMicrogameTimer()
        {
            _timer = Mathf.Min(_timer + Time.unscaledDeltaTime * Mathf.Pow(2, _speed / 12.0f), _currentMicrogame.GetDuration());
        }

        private void UpdateState()
        {
            _state = _state switch
            {
                State.Intermission => UpdateStateIntermission(),
                State.StartMicrogame => UpdateStateStartMicrogame(),
                State.Microgame => UpdateStateMicrogame(),
                State.EndMicrogame => UpdateStateEndMicrogame(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
        
        #region Intermission

        private State EnterStateIntermission()
        {
            _currentMicrogameIndex = PopAvailableMicrogame();
            LoadMicrogame(_currentMicrogameIndex);
            _timer = 2f;

            if (!_lastMicrogameSuccess)
            {
                _lives--;
                if (_lives <= 0)
                {
                    _speed = 0;
                    _difficulty = 0;
                    _lives = 4;
                    _score = 0;
                }
            }

            if (_shouldSpeedUp)
            {
                _speed = Mathf.Min(12, _speed + 1);
                _difficulty = Mathf.Min(5, _difficulty + 1);
                _shouldSpeedUp = false;
            }
                
            onIntermissionBegin?.Invoke();
            onInstructionVisibility?.Invoke(false);
            onRoundVisibility?.Invoke(true);
            
            return State.Intermission;
        }

        private State UpdateStateIntermission()
        {
            if (_timer > 0)
                return State.Intermission;

            if (_lives <= 0)
            {
                //TODO Loose game
            }

            ExitStateIntermission();
            return EnterStateStartMicrogame();
        }

        private void ExitStateIntermission()
        {
            _hasCheered = false;
        }
        
        #endregion
        
        #region StartMicrogame

        private State EnterStateStartMicrogame()
        {
            _timer = .5f;
            _hasDisplayedInstruction = false;
            
            return State.StartMicrogame;
        }

        private State UpdateStateStartMicrogame()
        {
            if (_sceneLoadOperation.isDone && !_hasDisplayedInstruction)
            {
                onInstructionVisibility?.Invoke(true);
                onRoundVisibility?.Invoke(false);
                onSetInstruction?.Invoke(_currentMicrogame.GetInstruction());
                _hasDisplayedInstruction = true;
            }
            
            if (_timer > 0 || !_sceneLoadOperation.isDone)
                return State.StartMicrogame;
            
            return EnterStateMicrogame();
        }
        
        #endregion
        
        #region Microgame

        private State EnterStateMicrogame()
        {
            _instructionTimer = .75f;
            SetMicrogameSceneActive();
            _currentMicrogame.Begin();
            _timer = 0;
            Time.timeScale = Mathf.Pow(2, _speed / 12.0f);
            onMicrogameBegin?.Invoke();
            onOverlayVisibility?.Invoke(!_currentMicrogame.IsBoss());
            return State.Microgame;
        }

        private State UpdateStateMicrogame()
        {
            if (_instructionTimer > 0)
            {
                _instructionTimer -= Time.deltaTime;
                if (_instructionTimer <= 0)
                {
                    onInstructionVisibility?.Invoke(false);
                    onRoundVisibility?.Invoke(false);
                }
            }

            if (_timer < _currentMicrogame.GetDuration() && !(_currentMicrogame.IsFinished() && IsInMicrogame())) 
                return State.Microgame;
            
            ExitStateMicrogame();
            return EnterStateEndMicrogame();
        }

        private void ExitStateMicrogame()
        {
            Time.timeScale = 0;
            _currentMicrogame.End();
            onOverlayVisibility?.Invoke(false);
            onInstructionVisibility?.Invoke(false);
            onRoundVisibility?.Invoke(false);
            SetGameSceneActive();
        }
        
        #endregion
        
        #region EndMicrogame

        private State EnterStateEndMicrogame()
        {
            _timer = 1f;
            _currentMicrogame.Cleanup();
            _lastMicrogameSuccess = _currentMicrogame.IsSuccess();
            
            if (_currentMicrogame.IsSuccess())
            {
                if (!_hasCheered)
                    OnWin();

                _score++;

                if (_score % _collection.speedUpStride == 0)
                    _shouldSpeedUp = true;
                
                //TODO request boss level beaten life gain
            }
            else
            {
                if (!_hasCheered)
                    OnLose();
            }

            UnloadMicrogame();
            onMicrogameFinished?.Invoke();

            return State.EndMicrogame;
        }

        private State UpdateStateEndMicrogame()
        {
            if (_timer > 0)
                return State.EndMicrogame;
            
            return EnterStateIntermission();
        }
        
        #endregion

        #region Utilities

        private void LoadMicrogame(int index)
        {
            _sceneLoadOperation = SceneManager.LoadSceneAsync(_collection.microgames[index].scene.BuildIndex, LoadSceneMode.Additive);
            Debug.Assert(_sceneLoadOperation != null);
            
            _sceneLoadOperation.completed += OnMicroGameLoaded;
        }

        private void UnloadMicrogame()
        {
            _sceneLoadOperation = SceneManager.UnloadSceneAsync(_microgameScene.buildIndex);
        }

        private void OnMicroGameLoaded(AsyncOperation operation)
        {
            _currentMicrogame = Microgame.Microgame.GetInstance();
            _currentMicrogame.Setup();
            _currentMicrogame.onWin.AddListener(OnWin);
            _currentMicrogame.onLose.AddListener(OnLose);
            _microgameScene = SceneManager.GetSceneByBuildIndex(_collection.microgames[_currentMicrogameIndex].scene.BuildIndex);
            operation.completed -= OnMicroGameLoaded;
            HandleAudioSources(_microgameScene);
        }

        internal static void HandleAudioSources(Scene microgameScene)
        {
            var audioSources = new HashSet<AudioSource>(FindObjectsByType<AudioSource>(FindObjectsInactive.Include));
            var audioPitchers = FindObjectsByType<AudioPitcher>(FindObjectsInactive.Include);

            foreach (var audioPitcher in audioPitchers)
            {
                foreach (var source in audioPitcher.sources)
                {
                    audioSources.Remove(source);
                }
            }

            var pitcherObject = new GameObject();
            SceneManager.MoveGameObjectToScene(pitcherObject, microgameScene);
            var pitcher = pitcherObject.AddComponent<AudioPitcher>();
            pitcher.sources = new List<AudioSource>(audioSources);
            pitcher.Pitch = 1;
        }

        private void OnWin()
        {
            //TODO win feedback
        }
        
        private void OnLose()
        {
            //TODO lose feedback
        }

        private void SetGameSceneActive()
        {
            SceneManager.SetActiveScene(_gameScene);
            camera.enabled = true;
            listener.enabled = true;
            lights.SetActive(true);
            DynamicGI.UpdateEnvironment();
        }

        private void SetMicrogameSceneActive()
        {
            SceneManager.SetActiveScene(_microgameScene);
            camera.enabled = false;
            listener.enabled = false;
            lights.SetActive(false);
            DynamicGI.UpdateEnvironment();
        }
        
        private int PopAvailableMicrogame()
        {
            if (_collection.microgames.Length <= 1)
                return 0;
            
            if (_availableMicrogames.Count == 0)
                PopulateAvailableMicrogames();
            
            int availableIndex;
            
            do
            {
                availableIndex = Random.Range(0, _availableMicrogames.Count);
            } while (_availableMicrogames[availableIndex] == _currentMicrogameIndex);

            var microgameIndex = _availableMicrogames[availableIndex];
            _availableMicrogames.RemoveAt(availableIndex);
            
            return microgameIndex;
        }

        private void PopulateAvailableMicrogames()
        {
            for (var i = 0; i < _collection.microgames.Length; i++)
                _availableMicrogames.Add(i);
        }
        
        #endregion
        
        #region Interface

        internal bool IsInMicrogame()
        {
            return _state == State.Microgame;
        }

        internal int GetDifficulty()
        {
            return _difficulty;
        }

        internal int GetSpeed()
        {
            return _speed;
        }

        internal void SetDifficulty(int difficulty)
        {
            _difficulty = difficulty;
        }

        internal void SetSpeed(int speed)
        {
            _speed = speed;
            _shouldSpeedUp = false;
        }

        internal State GetState()
        {
            return _state;
        }
        
        internal int GetScore()
        {
            return _score;
        }
        
        internal int GetLives()
        {
            return _lives;
        }

        internal bool HasWonLastMicrogame()
        {
            return _lastMicrogameSuccess;
        }
        
        internal void DisplayInstruction(string instruction)
        {
            if (!_currentMicrogame.IsBoss())
                return;
            
            _instructionTimer = 1f;
            onSetInstruction?.Invoke(instruction);
            onInstructionVisibility?.Invoke(true);
        }
        #endregion

        public float GetRemainingTime()
        {
            return _currentMicrogame ? _currentMicrogame.GetDuration() - _timer : float.PositiveInfinity;
        }
    }
}
