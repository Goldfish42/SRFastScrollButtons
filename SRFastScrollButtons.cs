using System;
using MelonLoader;
using System.Reflection;
using UnityEngine;
using System.IO;
using VRTK.UnityEventHelper;
using Synth.Utils;
using System.Collections.Generic;
//using SRModCore;
using UnityEngine.Events;
using Util.Navigation;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using SRFastScrollButtons.MonoBehaviors;

namespace SRFastScrollButtons
{
    public class SRFastScrollButtons : MelonMod
    {
        private FastScrollArrowController controllerFSAC;
        private GameObject fastDownButtonGO;
        private GameObject fastUpButtonGO;

        public override void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            var mainMenuScenes = new List<string>()
            {
                "01.The Room",
                "02.The Void",
                "03.Roof Top",
                "04.The Planet",
                "SongSelection"
            };
            base.OnSceneWasInitialized(buildIndex, sceneName);

            if (mainMenuScenes.Contains(sceneName)) Setup();
        }

        private void Setup()
        {
            MelonLogger.Msg("SRFS: Setting up...");

            try
            {
                // Locate the pre-existing buttons
                GameObject songSelectionGO = GameObject.Find("SongSelection");
                Transform scrollArrowsT = songSelectionGO.transform.Find("SelectionSongPanel/CentralPanel/Song Selection/VisibleWrap/Songs/Scroll Arrows");
                Transform bottomButtonT = scrollArrowsT.Find("Bottom");
                Transform topButtonT = scrollArrowsT.Find("Top");
                Transform upButtonT = scrollArrowsT.Find("Up");
                Transform downButtonT = scrollArrowsT.Find("Down");

                // Add the FastScrollArrowController to "Scroll Arrows" object
                controllerFSAC = scrollArrowsT.gameObject.AddComponent<FastScrollArrowController>();

                // vars to manipulate button events
                VRTK_InteractableObject_UnityEvents events = null;
                VRTK_InteractableObject_UnityEvents persistentEvents = null;

                // add FastDown button
                fastDownButtonGO = MakeButton(bottomButtonT.gameObject, "FastDown", scrollArrowsT);
                RemoveOrDisableEvents(fastDownButtonGO, ref events, ref persistentEvents);
                persistentEvents.OnTouch.AddListener(controllerFSAC.FastScrollDownHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.FastScrollDownOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.FastScrollDown);
                fastDownButtonGO.SetActive(true);

                // add FastUp button
                fastUpButtonGO = MakeButton(topButtonT.gameObject, "FastUp", scrollArrowsT);
                RemoveOrDisableEvents(fastUpButtonGO, ref events, ref persistentEvents);
                persistentEvents.OnTouch.AddListener(controllerFSAC.FastScrollUpHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.FastScrollUpOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.FastScrollUp);
                fastUpButtonGO.SetActive(true);

                // adjust the echelons on the Top and Bottom buttons
                AdjustEchelons(bottomButtonT); 
                AdjustEchelons(topButtonT); 

                // overwrite Down events
                RemoveOrDisableEvents(downButtonT.gameObject, ref events, ref persistentEvents);
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollDownHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollDownOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollDown);

                // overwrite Up events
                RemoveOrDisableEvents(upButtonT.gameObject, ref events, ref persistentEvents);
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollUpHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollUpOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollUp);
               
                MelonLogger.Msg("... finished setting up");
            }
            catch (NullReferenceException ex)
            {
                string stackTrace = ex.StackTrace;
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }

        }

        private GameObject MakeButton(GameObject _buttonToClone, string _buttonName, Transform _parent)
        { 
            // vars for repositioning the button
            float lateralOffset = 0.7f;
            Vector3 newPosition;

            // Clone the topButton into fastUpButton
            GameObject newButton = GameObject.Instantiate(_buttonToClone);
            newButton.transform.name = _buttonName;
            newButton.transform.SetParent(_parent);

            // Reposition fastDownButton
            newPosition = _buttonToClone.transform.position + _buttonToClone.transform.right * lateralOffset;
            newButton.transform.position = newPosition;
            newButton.transform.localScale = _buttonToClone.transform.localScale;

            return newButton;
        }

        private void RemoveOrDisableEvents(GameObject _button, ref VRTK_InteractableObject_UnityEvents _events, ref VRTK_InteractableObject_UnityEvents _persistentEvents)
        {
            // get all event listeners
            _events = _button.GetComponent<VRTK_InteractableObject_UnityEvents>();
            _persistentEvents = _button.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

            // remove pre-existing listeners
            _events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
            _events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
            _events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

            // disable pre-existing persistent listeners
            for (int i = _persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                _persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
            }
            for (int i = _persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                _persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
            }
            for (int i = _persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                _persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
            }
        }

        private void AdjustEchelons(Transform _button)
        {
            // Get the echelon icons in the button
            GameObject icon0GO = _button.Find("ICON").gameObject;
            GameObject icon1GO = _button.Find("ICON (1)").gameObject;

            // Calculate the vertical space between them
            float iconVertDiff = icon1GO.transform.position.y - icon0GO.transform.position.y;
            float iconVertOffset = iconVertDiff / 2.0f;            

            if (_button.name == "Bottom")
            {
                ShiftEchelon(icon0GO, iconVertOffset, false);
                ShiftEchelon(icon1GO, iconVertOffset, false);
                MakeEchelon(icon1GO, _button, iconVertDiff, false);
            }
            else if (_button.name == "Top")
            {
                ShiftEchelon(icon0GO, iconVertOffset, true);
                ShiftEchelon(icon1GO, iconVertOffset, true);
                MakeEchelon(icon0GO, _button, iconVertDiff, true);
            }
        }

        private void ShiftEchelon(GameObject _icon, float _vertOffset, bool _shiftUp)
        {
            Vector3 newPosition;
            Vector3 originalScale;

            originalScale = _icon.transform.localScale;
            newPosition = _icon.transform.position;

            if (_shiftUp)
            {
                newPosition.y += _vertOffset;
            }
            else // shift down
            {
                newPosition.y -= _vertOffset;
            }

            _icon.transform.position = newPosition;
            _icon.transform.localScale = originalScale;
        }

        private void MakeEchelon(GameObject _outerIcon, Transform _parent, float _vertDiff, bool _placeAbove)
        {
            Vector3 newPosition;

            // Clone the outermost icon
            GameObject icon2GO = GameObject.Instantiate(_outerIcon);
            icon2GO.transform.name = "ICON (2)";
            icon2GO.transform.SetParent(_parent);

            newPosition = _outerIcon.transform.position;

            if (_placeAbove)
            {
                newPosition.y -= _vertDiff;
            }
            else // place below
            {
                newPosition.y += _vertDiff;
            }

            icon2GO.transform.position = newPosition;
            icon2GO.transform.localScale = _outerIcon.transform.localScale;
        }

    }
}
