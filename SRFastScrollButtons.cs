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
        public static SRFastScrollButtons Instance; // { get; private set; }

        LoopScrollArrowController controllerLSAC;
        LoopScrollRect loopScrollLSR;
        FastScrollArrowController controllerFSAC;
        //int prevIndex = 0;

        // Reflection vars
        static Type typeLSAC = typeof(LoopScrollArrowController);
        static FieldInfo loopScrollFI = typeLSAC.GetField("loopScroll", BindingFlags.NonPublic | BindingFlags.Instance);

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

            if (mainMenuScenes.Contains(sceneName)) ButtonInit();
        }

        private void ButtonInit()
        {
            MelonLogger.Msg("SRFS: Initializing Buttons");

            try
            {
                // Locate the pre-existing buttons
                GameObject songSelectionGO = GameObject.Find("SongSelection");
                Transform scrollArrowsT = songSelectionGO.transform.Find("SelectionSongPanel/CentralPanel/Song Selection/VisibleWrap/Songs/Scroll Arrows");
                Transform bottomButtonT = scrollArrowsT.Find("Bottom");
                Transform topButtonT = scrollArrowsT.Find("Top");
                Transform upButtonT = scrollArrowsT.Find("Up");
                Transform downButtonT = scrollArrowsT.Find("Down");

                // vars for repositioning the buttons
                float lateralOffset = 0.7f;
                Vector3 newPosition;
                Vector3 originalScale;

                // Keep the ScrollArrowController and LoopScrollRect for later
                controllerLSAC = scrollArrowsT.GetComponent<LoopScrollArrowController>();
                controllerFSAC = scrollArrowsT.gameObject.AddComponent<FastScrollArrowController>();

                // vars to manipulate button events
                VRTK_InteractableObject_UnityEvents events;
                VRTK_InteractableObject_UnityEvents persistentEvents;

                //===============================================================================================
                // Clone the bottomButton into fastDownButton
                fastDownButtonGO = GameObject.Instantiate(bottomButtonT.gameObject);
                fastDownButtonGO.transform.name = "FastDown";
                fastDownButtonGO.transform.SetParent(scrollArrowsT);

                // Reposition fastDownButton
                newPosition = bottomButtonT.position + bottomButtonT.right * lateralOffset;
                fastDownButtonGO.transform.position = newPosition;
                fastDownButtonGO.transform.localScale = bottomButtonT.localScale;

                // get all event listeners
                events = fastDownButtonGO.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = fastDownButtonGO.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.FastScrollDownHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.FastScrollDownOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.FastScrollDown);

                //===============================================================================================

                //===============================================================================================
                // Clone the topButton into fastUpButton
                fastUpButtonGO = GameObject.Instantiate(topButtonT.gameObject);
                fastUpButtonGO.transform.name = "FastUp";
                fastUpButtonGO.transform.SetParent(scrollArrowsT);

                // Reposition fastDownButton
                newPosition = topButtonT.position + topButtonT.right * lateralOffset;
                fastUpButtonGO.transform.position = newPosition;
                fastUpButtonGO.transform.localScale = topButtonT.localScale;

                // get all event listeners
                events = fastUpButtonGO.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = fastUpButtonGO.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.FastScrollUpHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.FastScrollUpOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.FastScrollUp);
                //===============================================================================================

                //===============================================================================================
                // Get the echelon icons in the Bottom button
                GameObject downIcon0GO = bottomButtonT.Find("ICON").gameObject;
                GameObject downIcon1GO = bottomButtonT.Find("ICON (1)").gameObject;

                // Calculate the vertical space between them
                float downIconVertDiff = downIcon1GO.transform.position.y - downIcon0GO.transform.position.y;
                float downIconVertOffset = downIconVertDiff / 2.0f;

                // shift the icons to keep them more centered when the third one is added
                originalScale = downIcon0GO.transform.localScale;
                newPosition = downIcon0GO.transform.position;
                newPosition.y -= downIconVertOffset;
                downIcon0GO.transform.position = newPosition;
                downIcon0GO.transform.localScale = originalScale;
                originalScale = downIcon1GO.transform.localScale;
                newPosition = downIcon1GO.transform.position;
                newPosition.y -= downIconVertOffset;
                downIcon1GO.transform.position = newPosition;
                downIcon1GO.transform.localScale = originalScale;

                // Clone the outermost icon
                GameObject downIcon2GO = GameObject.Instantiate(downIcon1GO);
                downIcon2GO.transform.name = "ICON (2)";
                downIcon2GO.transform.SetParent(bottomButtonT);

                // move the icon out
                newPosition = downIcon1GO.transform.position;
                newPosition.y += downIconVertDiff;
                downIcon2GO.transform.position = newPosition;
                downIcon2GO.transform.localScale = downIcon1GO.transform.localScale;
                //===============================================================================================

                //===============================================================================================
                // Get the echelon icons in the Top button
                GameObject upIcon0GO = topButtonT.Find("ICON").gameObject;
                GameObject upIcon1GO = topButtonT.Find("ICON (1)").gameObject;

                // Calculate the vertical space between them
                float upIconVertDiff = upIcon1GO.transform.position.y - upIcon0GO.transform.position.y;
                float upIconVertOffset = upIconVertDiff / 2.0f;

                // shift the icons to keep them more centered when the third one is added
                originalScale = upIcon0GO.transform.localScale;
                newPosition = upIcon0GO.transform.position;
                newPosition.y += upIconVertOffset;
                upIcon0GO.transform.position = newPosition;
                upIcon0GO.transform.localScale = originalScale;
                originalScale = upIcon1GO.transform.localScale;
                newPosition = upIcon1GO.transform.position;
                newPosition.y += upIconVertOffset;
                upIcon1GO.transform.position = newPosition;
                upIcon1GO.transform.localScale = originalScale;

                // Clone the outermost icon
                GameObject upIcon2GO = GameObject.Instantiate(upIcon0GO);
                upIcon2GO.transform.name = "ICON (2)";
                upIcon2GO.transform.SetParent(topButtonT);

                // move the icon out
                newPosition = upIcon0GO.transform.position;
                newPosition.y -= upIconVertDiff;
                upIcon2GO.transform.position = newPosition;
                upIcon2GO.transform.localScale = upIcon0GO.transform.localScale;
                //===============================================================================================
                // activate objects
                fastUpButtonGO.SetActive(true);
                fastDownButtonGO.SetActive(true);
                //===============================================================================================

                //===============================================================================================
                // redirect Down button events to the FastScrollArrowController
                // get all event listeners
                events = downButtonT.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = downButtonT.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollDownHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollDownOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollDown);
                //===============================================================================================
                // redirect Up button events to the FastScrollArrowController
                // get all event listeners
                events = upButtonT.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = upButtonT.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollUpHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollUpOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollUp);
                //===============================================================================================
                // redirect Bottom button events to the FastScrollArrowController
                // get all event listeners
                events = bottomButtonT.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = bottomButtonT.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollBottomHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollBottomOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollBottom);
                //===============================================================================================
                // redirect Top button events to the FastScrollArrowController
                // get all event listeners
                events = topButtonT.GetComponent<VRTK_InteractableObject_UnityEvents>();
                persistentEvents = topButtonT.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();

                // remove pre-existing listeners
                events.OnTouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUntouch.RemoveAllListeners(); // 1 (non-persistent) listener to remove as of 20230602
                events.OnUse.RemoveAllListeners(); // no (non-persistent) listeners to remove as of 20230602

                // disable pre-existing persistent listeners
                for (int i = persistentEvents.OnTouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnTouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUntouch.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUntouch.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // add OnUse listener
                persistentEvents.OnTouch.AddListener(controllerFSAC.ScrollTopHover);
                persistentEvents.OnUntouch.AddListener(controllerFSAC.ScrollTopOut);
                persistentEvents.OnUse.AddListener(controllerFSAC.ScrollTop);
                //===============================================================================================
            }
            catch (NullReferenceException ex)
            {
                string stackTrace = ex.StackTrace;
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }

        }

    }
}
