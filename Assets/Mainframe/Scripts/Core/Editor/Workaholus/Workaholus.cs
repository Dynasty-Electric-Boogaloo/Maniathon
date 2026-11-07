#if UNITY_EDITOR
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Eflatun.SceneReference;
using Mainframe.Scripts.Microgame;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Assemblies;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;
using Random = System.Random;

namespace Mainframe.Scripts.Core.Editor.Workaholus
{
    public class Workaholus : EditorWindow
    {
        private static Texture _workaholusTexture;
        private static string _microgameName;
        private static string _remoteURL;
        private static bool _hasGithub;
        private static bool _shouldPush;
        private static bool _waitingRestart;

        private class MagicContext
        {
            public string microgameName;
            public string microgameFolder;
            public string remoteURL;
            public bool git;
            public Scene scene;
            public Camera camera;
            public AudioListener listener;
            public MicrogameInfo info;
        }
        
        [MenuItem("Workaholus/Create new Microgame...")]
        public static void ShowExample()
        {
            Workaholus wnd = GetWindow<Workaholus>();
            wnd.minSize = new Vector2(256, 400);
            wnd.titleContent = new GUIContent("Workaholus");
        }

        private void OnGUI()
        {
            if (!_workaholusTexture)
                _workaholusTexture = Resources.Load<Texture>("Workaholus");
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Box(_workaholusTexture, GUILayout.Width(256), GUILayout.Height(256));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("Tell the mighty Workaholus...");
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("...your project name", "The base name of your yet to be created Microgame."), GUILayout.ExpandWidth(false));
            var field = GUILayout.TextField(_microgameName);
            _microgameName = Regex.Replace(field ?? "", "([^A-Za-z0-9_])", "");
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("...do you want a git repository?", "Check if you want Workaholus to automatically create a git repository in your Microgame folder"), GUILayout.ExpandWidth(false));
            _hasGithub = GUILayout.Toggle(_hasGithub, new GUIContent("", "Check if you want Workaholus to automatically create a git repository in your Microgame folder"));
            GUILayout.EndHorizontal();

            if (_hasGithub)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("...want me to push to an online repo?", "Check if you want Workaholus to automatically push your git repository to an online repository"), GUILayout.ExpandWidth(false));
                _shouldPush = GUILayout.Toggle(_shouldPush, new GUIContent("", "Check if you want Workaholus to automatically push your git repository to an online repository"));
                GUILayout.EndHorizontal();

                if (_shouldPush)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(new GUIContent("...your online repo's URL", "Paste here the link to your online github repository for Workaholus to push to"), GUILayout.ExpandWidth(false));
                    _remoteURL = GUILayout.TextField(_remoteURL);
                    GUILayout.EndHorizontal();

                    if (!Regex.IsMatch(_remoteURL ?? "", "^(([A-Za-z0-9]+@|http(|s)\\:\\/\\/)|(http(|s)\\:\\/\\/[A-Za-z0-9]+@))([A-Za-z0-9.]+(:\\d+)?)(?::|\\/)([\\d\\/\\w.-]+?)(\\.git){1}$"))
                    {
                        GUILayout.Label("Watch out! Your online repo URL isn't valid!");
                    }
                }
            }

            GUI.enabled = !string.IsNullOrEmpty(_microgameName);
            
            if (GUILayout.Button("Create!"))
            {
                CreateMicrogame();
            }

            if (IsMagicContextAvailable() && !_waitingRestart && !AssetDatabase.IsAssetImportWorkerProcess())
            {
                ContinueCreation();
            }
        }

        private void CreateMicrogame()
        {
            var context = new MagicContext()
            {
                microgameName = $"{GetRandomPrefix()}_{_microgameName}",
                remoteURL = _shouldPush ? _remoteURL : null,
                git = _hasGithub
            };
            
            CreateFolders(context);
            CreateScript(context);
            CreateAssemblyDefinition(context);
            BackupMagicContext(context);
            AssetDatabase.Refresh();
            _waitingRestart = true;
        }

        private void ContinueCreation()
        {
            var context = RestoreMagicContext();
            CreateScene(context);
            CreateScriptableObject(context);
            AssetDatabase.Refresh();
            Selection.SetActiveObjectWithContext(context.info, null);
            
            if (context.git)
                CreateRepository(context);

            DisposeMagicContext();
        }

        private void CreateFolders(MagicContext context)
        {
            context.microgameFolder = $"Assets/Microgames/{context.microgameName}";
            AssetDatabase.CreateFolder("Assets/Microgames", context.microgameName);
            AssetDatabase.CreateFolder(context.microgameFolder, "Scripts");
        }
        
        private void CreateScript(MagicContext context)
        {
            var scriptText = MicrogameScript.Replace("{MICROGAME}", context.microgameName);
            var scriptName = $"{context.microgameName}Microgame.cs";
            
            var scriptFile = File.Open($"{Application.dataPath}/../{context.microgameFolder}/Scripts/{scriptName}", FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(scriptFile);
            writer.Write(scriptText);
            writer.Close();
            scriptFile.Close();
        }
        
        private void CreateAssemblyDefinition(MagicContext context)
        {
            var assemblyDef = AssemblyDef.Replace("{MICROGAME}", context.microgameName);
            var assemblyName = $"{context.microgameName}.asmdef";

            var assemblyFile = File.Open($"{Application.dataPath}/../{context.microgameFolder}/{assemblyName}", FileMode.Create, FileAccess.Write);
            var writer = new StreamWriter(assemblyFile);
            writer.Write(assemblyDef);
            writer.Close();
            assemblyFile.Close();
        }

        private void CreateScene(MagicContext context)
        {
            context.scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            CreateSimpleCamera(context);
            CreateMicrogameManager(context);
            
            EditorSceneManager.SaveScene(context.scene, $"{context.microgameFolder}/{context.microgameName}.unity");
        }

        private void CreateScriptableObject(MagicContext context)
        {
            context.info = CreateInstance<MicrogameInfo>();
            context.info.scene = SceneReference.FromScenePath(context.scene.path);
            
            AssetDatabase.CreateAsset(context.info, $"{context.microgameFolder}/{context.microgameName}.asset");
            AssetDatabase.SaveAssets();

            var scenes = EditorBuildSettings.scenes;

            foreach (var scene in scenes)
            {
                if (scene.path == context.scene.path)
                    return;
            }

            Array.Resize(ref scenes, scenes.Length + 1);
            scenes[^1] = new EditorBuildSettingsScene(context.scene.path, true);
            EditorBuildSettings.scenes = scenes;
        }

        private void CreateRepository(MagicContext context)
        {
            File.Copy($"{Application.dataPath}/../.gitignore", $"{Application.dataPath}/../{context.microgameFolder}/.gitignore");
            
            var cmd = new Process();
            var info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.RedirectStandardInput = true;
            info.RedirectStandardOutput = false;
            info.RedirectStandardError = true;
            info.UseShellExecute = false;

            cmd.StartInfo = info;
            cmd.Start();
            cmd.ErrorDataReceived += (sender, args) => { Debug.Log(args.Data); };
            cmd.BeginErrorReadLine();

            using var w = cmd.StandardInput;
            
            if (!w.BaseStream.CanWrite)
                return;
            
            w.WriteLine($"cd {context.microgameFolder}");
                
            w.WriteLine("git init -b main");
            w.WriteLine("git add *");
            w.WriteLine("git commit -m \"Initial commit\"");

            if (!string.IsNullOrEmpty(context.remoteURL))
            {
                w.WriteLine($"git remote add origin {context.remoteURL}");
                w.WriteLine("git push -u origin main");
            }
        }

        private void CreateSimpleCamera(MagicContext context)
        {
            var cameraObject = new GameObject("Main Camera")
            {
                tag = "MainCamera",
                layer = 0
            };

            context.camera = cameraObject.AddComponent<Camera>();
            var camRect = new Rect(0.09166666666666666666666666666667f, 0.03888888888888888888888888888889f,
                0.809375f, 0.93703703703703703703703703703704f);
            context.camera.rect = camRect;
            context.camera.cullingMask = 0x7FFFFFFF;

            var cameraData = cameraObject.AddComponent<UniversalAdditionalCameraData>();
            cameraData.renderPostProcessing = true;
            
            context.listener = cameraObject.AddComponent<AudioListener>();
        }

        private void CreateMicrogameManager(MagicContext context)
        {
            Type baseType = null;
            Type microgameType = null;
            
            foreach (var assembly in CurrentAssemblies.GetLoadedAssemblies())
            {
                var tt = assembly.GetType($"Microgames.{context.microgameName}.{context.microgameName}Microgame");
                if (tt != null)
                    microgameType = tt;

                tt = assembly.GetType("Mainframe.Scripts.Microgame.Microgame");
                if (tt != null)
                    baseType = tt;
            }

            var microgameObject = new GameObject("Microgame");
            var microgame = microgameObject.AddComponent(microgameType);

            baseType.InvokeMember("camera", BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, microgame, new object[]{context.camera});
            baseType.InvokeMember("listener", BindingFlags.SetField | BindingFlags.SetProperty | BindingFlags.NonPublic | BindingFlags.Instance, null, microgame, new object[]{context.listener});
        }

        private void BackupMagicContext(MagicContext context)
        {
            var file = File.Open($"{Application.persistentDataPath}/workaholus", FileMode.Create);
            var writer = new StreamWriter(file);
            writer.WriteLine(context.microgameName);
            writer.WriteLine(context.microgameFolder);
            writer.WriteLine(context.remoteURL);
            writer.Write(context.git);
            
            writer.Close();
            file.Close();
        }

        private MagicContext RestoreMagicContext()
        {
            var context = new MagicContext();
            
            var file = File.Open($"{Application.persistentDataPath}/workaholus", FileMode.Open);
            var reader = new StreamReader(file);
            context.microgameName = reader.ReadLine();
            context.microgameFolder = reader.ReadLine();
            context.remoteURL = reader.ReadLine();
            context.git = reader.ReadLine() == "True";
            reader.Close();
            file.Close();
            
            return context;
        }

        private void DisposeMagicContext()
        {
            File.Delete($"{Application.persistentDataPath}/workaholus");
        }

        private bool IsMagicContextAvailable()
        {
            return File.Exists($"{Application.persistentDataPath}/workaholus");
        }

        private string GetRandomPrefix()
        {
            var random = new Random();
            return $"{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}{(char)('A' + random.Next(26))}";
        }

        private const string MicrogameScript = "namespace Microgames.{MICROGAME}\n{\n    public class {MICROGAME}Microgame : Mainframe.Scripts.Microgame.Microgame\n    {\n        public override string GetInstruction()\n        {\n            throw new System.NotImplementedException();\n        }\n\n        public override float GetDuration()\n        {\n            throw new System.NotImplementedException();\n        }\n\n        public override bool IsFinished()\n        {\n            throw new System.NotImplementedException();\n        }\n\n        public override bool IsSuccess()\n        {\n            throw new System.NotImplementedException();\n        }\n    }\n}";
        private const string AssemblyDef = "{\n    \"name\": \"{MICROGAME}\",\n    \"rootNamespace\": \"\",\n    \"references\": [\n        \"GUID:6dcdf78b5602a4d438a64b450ef9f5d4\",\n        \"GUID:792682f5a95ee1546a2f09d9df06b2d8\",\n        \"GUID:c93df30cbaea6e94f8f5b0fe2b2114a9\"\n    ],\n    \"includePlatforms\": [],\n    \"excludePlatforms\": [],\n    \"allowUnsafeCode\": false,\n    \"overrideReferences\": false,\n    \"precompiledReferences\": [],\n    \"autoReferenced\": true,\n    \"defineConstraints\": [],\n    \"versionDefines\": [],\n    \"noEngineReferences\": false\n}";
    }
}
#endif