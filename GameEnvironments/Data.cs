using System;
using UnityEditor;

namespace GameEnvironments
{
    [Serializable]
    public sealed class Data
    {
        private const string CurrentEnvironmentKey = "environment.currentEnvironment";
        private const string IsLaunchedKey         = "environment.isLaunched";
        private const string MenuItemsUpdatedKey   = "environment.menuItemsUpdated";
        private const string AllowSwitchKey        = "environment.allowSwitch";
        private const string BadQuittingKey        = "environment.badQuitting";

        public static string CurrentEnvironment
        { 
            get => EditorPrefs.GetString(CurrentEnvironmentKey, Develop.Define);
            set => EditorPrefs.SetString(CurrentEnvironmentKey, value);
        }
        public static bool MenuItemsUpdated
        { 
            get => EditorPrefs.GetBool(MenuItemsUpdatedKey);
            set => EditorPrefs.SetBool(MenuItemsUpdatedKey, value);
        }
        public static bool AllowSwitch
        { 
            get => EditorPrefs.GetBool(AllowSwitchKey);
            set => EditorPrefs.SetBool(AllowSwitchKey, value);
        }
        public static bool BadQuitting
        { 
            get => EditorPrefs.GetBool(BadQuittingKey);
            set => EditorPrefs.SetBool(BadQuittingKey, value);
        }
        public static bool IsLaunched
        { 
            get => EditorPrefs.GetBool(IsLaunchedKey);
            set => EditorPrefs.SetBool(IsLaunchedKey, value);
        }
    }   
}