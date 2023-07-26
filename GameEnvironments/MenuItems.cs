using System.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Callbacks;
using UnityEngine;

namespace GameEnvironments
{
    [InitializeOnLoad]
    public static class MenuItems
    {
        private const string EnvironmentKey = "ENVIRONMENT_";
 
        private static NamedBuildTarget ActiveNamedBuildTarget
        {
            get
            {
                var target = EditorUserBuildSettings.activeBuildTarget;
                var group = BuildPipeline.GetBuildTargetGroup(target);
                return NamedBuildTarget.FromBuildTargetGroup(group);
            }
        }       

        static MenuItems()
        { 
            EditorApplication.delayCall += OnDelayReload;
            EditorApplication.delayCall += OnDelayLaunch;
            EditorApplication.quitting += OnApplicationQuitting;
        }

        private static void SetDefineSymbols(string symbols)
            => PlayerSettings.SetScriptingDefineSymbols(ActiveNamedBuildTarget, symbols);
        private static void SwitchDebug(string environment, string color)
            => Debug.Log($"Switched to <color={color}>{environment}</color> environment.");
        private static void DataCorrection()
        {
            Data.IsLaunched = false;
            Data.MenuItemsUpdated = false;
            Data.AllowSwitch = true;
            Data.BadQuitting = false;
        } 
        private static void SwitchEnvironment(string toEnvironmentDefine)
        {
            if (!Data.AllowSwitch || EditorApplication.isPlayingOrWillChangePlaymode) return;

            var defineSymbols = PlayerSettings.GetScriptingDefineSymbols(ActiveNamedBuildTarget);
            var newEnvironment = $"{EnvironmentKey}{toEnvironmentDefine}";
            if (!defineSymbols.Contains(EnvironmentKey))
            {
                SetDefineSymbols($"{defineSymbols};{newEnvironment}");
                OnSwitchEnvironment(toEnvironmentDefine);
                return;
            }

            if (defineSymbols.Contains(newEnvironment)) return;

            var defines = defineSymbols.Split(';');
            var currentEnvironment = defines.FirstOrDefault(x => x.Contains(EnvironmentKey));
            SetDefineSymbols(defineSymbols.Replace(currentEnvironment, newEnvironment));
            OnSwitchEnvironment(toEnvironmentDefine);
        }
        private static void ResetMenuCheckers()
        {
            Menu.SetChecked(Develop.Path, false);
            Menu.SetChecked(Testing.Path, false);
            Menu.SetChecked(Production.Path, false);
        }
        private static void UpdateMenuItems() 
        {
            if (Data.MenuItemsUpdated) return;
#if ENVIRONMENT_DEVELOP
            SetChecked(Develop.Path, true);
            SwitchDebug(Develop.Define, Develop.Color);
#elif ENVIRONMENT_TESTING
            SetChecked(Testing.Path, true);
            SwitchDebug(Testing.Define, Testing.Color);
#elif ENVIRONMENT_PRODUCTION
            SetChecked(Production.Path, true);
            SwitchDebug(Production.Define, Production.Color);
#endif
            Data.MenuItemsUpdated = true;
            Data.AllowSwitch = true;
            Data.BadQuitting = false;
        }
        private static void SetChecked(string path, bool value)
        {
            ResetMenuCheckers();
            Menu.SetChecked(path, value);
        }
        private static void OnSwitchEnvironment(string toEnvironmentDefine)
        {
            Data.CurrentEnvironment = toEnvironmentDefine;
            Data.MenuItemsUpdated = false;
            Data.AllowSwitch = false;
            Data.BadQuitting = true;
        }
        private static void OnApplicationQuitting()
        {
            Data.BadQuitting = false;
            Data.IsLaunched = false;
            Data.MenuItemsUpdated = false;
            Application.quitting -= OnApplicationQuitting;
        }
        private static void OnDelayLaunch()
        {
            if (Data.BadQuitting) DataCorrection();
            else if (Data.IsLaunched) return;

            Data.IsLaunched = true;
            SwitchEnvironment(Data.CurrentEnvironment);
            UpdateMenuItems();
        }
        private static void OnDelayReload()
        {
            if (!Data.IsLaunched || EditorApplication.isPlayingOrWillChangePlaymode) return;
            SwitchEnvironment(Data.CurrentEnvironment);
            UpdateMenuItems();
        }

        [MenuItem(Develop.Path)] private static void SetDevelopEnvironment() => SwitchEnvironment(Develop.Define);     
        [MenuItem(Testing.Path)] private static void SetTestingEnvironment() => SwitchEnvironment(Testing.Define);  
        [MenuItem(Production.Path)] private static void SetProductionEnvironment() => SwitchEnvironment(Production.Define);
    }
}