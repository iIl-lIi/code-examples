using System;

namespace GameEnvironments
{
    [Serializable]
    public sealed class Data
    {
        private const string FileFolder = "ProjectSettings/";
        private const string FileName = "environment";
        private static string FilePath = FileFolder + FileName;

        public string currentEnvironment = Develop.Define;
        public bool menuItemsUpdated;
        public bool isLaunched;
        public bool allowSwitch;
        public bool badQuitting;

        private Data ChangeCurrentEnvironment(string env)
        {
            currentEnvironment = env;
            return this;
        }
        private Data ChangeMenuItemsUpdated(bool miu)
        {
            menuItemsUpdated = miu;
            return this;
        }
        private Data ChangeIsLaunched(bool isl)
        {
            isLaunched = isl;
            return this;
        }
        private Data ChangeAllowSwitch(bool als)
        {
            allowSwitch = als;
            return this;
        }
        private Data ChangeBadQuitting(bool bqt)
        {
            badQuitting = bqt;
            return this;
        }
        
        private static Data Get() => JsonSaveSystem.Load<Data>(FilePath);
        private static void Set(Data data) => JsonSaveSystem.Save(data, FilePath);

        public static string CurrentEnvironment
        { 
            get => Get().currentEnvironment; 
            set => Set(Get().ChangeCurrentEnvironment(value));
        }
        public static bool MenuItemsUpdated
        { 
            get => Get().menuItemsUpdated; 
            set => Set(Get().ChangeMenuItemsUpdated(value));
        }
        public static bool IsLaunched
        { 
            get => Get().isLaunched; 
            set => Set(Get().ChangeIsLaunched(value));
        }
        public static bool AllowSwitch
        { 
            get => Get().allowSwitch; 
            set => Set(Get().ChangeAllowSwitch(value));
        }
        public static bool BadQuitting
        { 
            get => Get().badQuitting; 
            set => Set(Get().ChangeBadQuitting(value));
        }
    }   
}