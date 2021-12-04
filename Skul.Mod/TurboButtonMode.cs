using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Characters.Actions;
using Services;
using Singletons;
using UnityEngine;

namespace Skul.Mod
{
    public static class TurboButtonMode
    {
        private static Coroutine SetTurboCoroutineHandle = null;
        
        // these are the protected enum values for the input types
        const int TryStartIsPressed = 0;
        const int TryStartWasPressed = 1;
        
        /// <summary>
        /// This field controls if an action requires a Button-Press or a Button-Hold 
        /// </summary>
        private static readonly FieldInfo _inputMethodField = typeof(Action)
            .GetField("_inputMethod", BindingFlags.Instance | BindingFlags.NonPublic);
        
        /// <summary>
        /// Indicates if Turbo-Mode is currently active
        /// </summary>
        public static bool IsTurboActive { get; private set; }

        /// <summary>
        /// Toggles Turbo Mode.
        /// Don't forget to call <see cref="StartSetTurboCoroutine"/> to refresh this setting for new equipped skulls
        /// </summary>
        public static void ToggleTurbo()
        {
            IsTurboActive = !IsTurboActive;
            
            string text = IsTurboActive ? "enabled" : "disabled";

            Helper.TextSpawner.SpawnBuff($"Turbo {text}", Helper.Player.transform.position);
            
            // Also update the new state for the current skulls
            SetTurboOnCurrentSkulls();
        }
        
        /// <summary>
        /// Sets the configured Turbo-Mode on all currently equipped skulls
        /// </summary>
        public static void SetTurboOnCurrentSkulls()
        {
            int typeValue = IsTurboActive ? TryStartIsPressed : TryStartWasPressed;
            
            // find player action for the different skulls.
            // There is usually two actions for each equipped skull.

            // we only want to modify the BasicAttack and JumpAttach actions
            foreach (var action in Helper.Player.actions)
            {
                switch (action.type)
                {
                    case Action.Type.BasicAttack:
                    case Action.Type.JumpAttack:
                        _inputMethodField.SetValue(action, typeValue);
                        break;
                }
            }
        }

        #region Couroutine
        /// <summary>
        /// Calls <see cref="SetTurboOnCurrentSkulls"/> regulary
        /// </summary>
        public static void StartSetTurboCoroutine(MonoBehaviour anyObject)
        {
            if (SetTurboCoroutineHandle == null)
            {
                Helper.Logger.LogInfo("Turbo-Button coroutine starting");
                anyObject.StartCoroutine(SetTurboCoroutine());
            }
        }

        private static IEnumerator SetTurboCoroutine()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);
                
                if(Helper.IsInGame)
                    SetTurboOnCurrentSkulls();
            }
        }
        #endregion
    }
}