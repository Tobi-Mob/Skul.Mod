using BepInEx.Logging;
using Characters;
using Level;
using Services;
using Singletons;

namespace Skul.Mod
{
    public static class Helper
    {
        public static ManualLogSource Logger;
        public static LevelManager LevelManager => Singleton<Service>.Instance.levelManager;
        public static  FloatingTextSpawner TextSpawner => Singleton<Service>.Instance.floatingTextSpawner;

        public static Character Player => (LevelManager != null) ? LevelManager.player : null;
    }
}