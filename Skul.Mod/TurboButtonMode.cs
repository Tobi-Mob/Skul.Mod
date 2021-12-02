using System.Collections.Generic;
using System.Reflection;
using Characters.Actions;
using Services;
using Singletons;

namespace Skul.Mod
{
    public static class TurboButtonMode
    {
        private static FieldInfo _inputMethodField = typeof(Action).GetField("_inputMethod", BindingFlags.Instance | BindingFlags.NonPublic);
        
        public static void ToggleTurbo(int index)
        {
            // find player action for the different skulls.
            // There is usually one action for each equipped skull.
            // The first found actions are usually for the currently active skull
            // The second found actions are usually for the currently inactive skull
            List<Action> basicAttacks = new List<Action>(2);
            List<Action> jumpAttacks  = new List<Action>(2);

            foreach (var action in Helper.Player.actions)
            {
                switch (action.type)
                {
                    case Action.Type.BasicAttack:
                        basicAttacks.Add(action);
                        break;
                    case Action.Type.JumpAttack:
                        jumpAttacks.Add(action);
                        break;
                }
            }

            // these are the protected enum values for the input types
            const int TryStartIsPressed = 0;
            const int TryStartWasPressed = 1;

            // check if the turbo is currently active
            int currentInputMethod = (int)_inputMethodField.GetValue(basicAttacks[index]);

            string text;
            
            if (currentInputMethod == TryStartWasPressed)
            {
                // Activate turbo
                // DoAction while the button is held down
                text = "enabled";
                _inputMethodField.SetValue(basicAttacks[index], TryStartIsPressed);
                _inputMethodField.SetValue(jumpAttacks[index], TryStartIsPressed);
            }
            else
            {
                // Disable turbo
                // DoAction if the button is pressed
                text = "disabled";
                _inputMethodField.SetValue(basicAttacks[index], TryStartWasPressed);
                _inputMethodField.SetValue(jumpAttacks[index], TryStartWasPressed);
            }

            // Display message on screen
            switch (index)
            {
                case 0: Helper.TextSpawner.SpawnBuff($"Turbo {text} for active skull", Helper.Player.transform.position);
                    break;
                case 1: Helper.TextSpawner.SpawnBuff($"Turbo {text} for secondary skull", Helper.Player.transform.position);
                    break;
                default: Helper.TextSpawner.SpawnBuff($"Turbo {text} for skull {index + 1}", Helper.LevelManager.player.transform.position);
                    break;
            }
        }
    }
}