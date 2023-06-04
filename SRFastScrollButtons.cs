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
using SRFastScrollButtons.Wrappers;

namespace SRFastScrollButtons
{
    public class SRFastScrollButtons : MelonMod
    {
        public static SRFastScrollButtons Instance; // { get; private set; }

        private LSACWrapper loopScrollArrowControllerW;

        LoopScrollArrowController controllerLSAC;
        LoopScrollRect loopScrollLSR;
        Animator btnAnimator;

        // Reflection vars
        static Type typeLSAC = typeof(LoopScrollArrowController);
        static MethodInfo DoScrollMI = typeLSAC.GetMethod("DoScroll", BindingFlags.NonPublic | BindingFlags.Instance);
        static object[] args = { true };
        static FieldInfo loopScrollFI = typeLSAC.GetField("loopScroll", BindingFlags.NonPublic | BindingFlags.Instance);

        private int fastStepFactor = 10;
        private int fastStepCount = 0;

        private GameObject fastDownButtonGO;
        private GameObject fastUpButtonGO;

        //private SRLogger logger;
        private UnityAction<object, VRTK.InteractableObjectEventArgs> onScrollDownFastUse;
        private UnityAction<object, VRTK.InteractableObjectEventArgs> onScrollUpFastUse;

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
                // Locate the pre-existing buttons to clone
                GameObject songSelectionGO = GameObject.Find("SongSelection");
                Transform scrollArrowsT = songSelectionGO.transform.Find("SelectionSongPanel/CentralPanel/Song Selection/VisibleWrap/Songs/Scroll Arrows");
                Transform bottomButtonT = scrollArrowsT.Find("Bottom");
                Transform topButtonT = scrollArrowsT.Find("Top");

                // vars for repositioning the buttons and their colliders
                float lateralOffset = 0.7f;
                Vector3 newPosition;
                Vector3 origScale;
                BoxCollider newCollider;
                BoxCollider origCollider;


                // Add the new LSACWrapper which will eventually replace the LoopScrollArrowController
                GameObject scrollArrowsGO = scrollArrowsT.gameObject;
                scrollArrowsGO.AddComponent<LSACWrapper>();// Delete the original LoopScrollArrowController component and replace with the LSACWrapper instance

                // Delete the original LoopScrollArrowController component
                //controllerLSAC = scrollArrowsT.GetComponent<LoopScrollArrowController>();
                //Component.Destroy(controllerLSAC); // doing this deletes the LoopScrollArrowController inside the LSACWrapper as well!

                // get a reference to the new LSACWrapper
                //loopScrollArrowControllerW = new LSACWrapper(controllerLSAC); // save native LSAC in wrapper
                loopScrollArrowControllerW = scrollArrowsGO.GetComponent<LSACWrapper>();

                // Keep the ScrollArrowController and LoopScrollRect for later
                //controllerLSAC = scrollArrowsT.GetComponent<LoopScrollArrowController>();


                //loopScrollLSR = (LoopScrollRect)loopScrollFI.GetValue(controllerLSAC);
                //if (loopScrollLSR.name != "Songs Scroll") {
                //    loopScrollLSR = null;
                //    throw new Exception("Wrong LoopScrollRect found!");
                //}

                // define the number of songs that each button press will skip
                //fastStepCount = controllerLSAC.StepCount * fastStepFactor;

                // vars to manipulate button events
                VRTK_InteractableObject_UnityEvents eventsVIOUE;
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

                // remove pre-existing listeners from fastDownButton
                eventsVIOUE = fastDownButtonGO.GetComponent<VRTK_InteractableObject_UnityEvents>();
                eventsVIOUE.OnUse.RemoveAllListeners(); // no non-persistent listeners to remove as of 20230602
                persistentEvents = fastDownButtonGO.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // activate fastDownButton and add listener
                fastDownButtonGO.SetActive(true);  // is this necessary?
                persistentEvents.OnUse.AddListener(loopScrollArrowControllerW.ScrollDownFast);

                // Rework the button animator
                //btnAnimator = fastDownButtonGO.GetComponent<Animator>();

                // Reposition BoxCollider
                origCollider = bottomButtonT.GetComponent<BoxCollider>();
                newCollider = fastDownButtonGO.GetComponent<BoxCollider>();
                newCollider.center = origCollider.center + (fastDownButtonGO.transform.position - bottomButtonT.position);
                newCollider.size = origCollider.size * fastDownButtonGO.transform.localScale.x / bottomButtonT.localScale.x;                

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

                // remove pre-existing listeners from fastDownButton
                eventsVIOUE = fastUpButtonGO.GetComponent<VRTK_InteractableObject_UnityEvents>();
                eventsVIOUE.OnUse.RemoveAllListeners(); // no non-persistent listeners to remove as of 20230602
                persistentEvents = fastUpButtonGO.GetComponentInChildren<VRTK_InteractableObject_UnityEvents>();
                for (int i = persistentEvents.OnUse.GetPersistentEventCount() - 1; i >= 0; i--)
                {
                    persistentEvents.OnUse.SetPersistentListenerState(i, UnityEventCallState.Off); // 1 persistent listener to remove as of 20230602 (ScrollBottom)
                }

                // activate fastDownButton and add listener
                fastUpButtonGO.SetActive(true);  // is this necessary?
                persistentEvents.OnUse.AddListener(loopScrollArrowControllerW.ScrollUpFast);
                //===============================================================================================

                //===============================================================================================
                // Get the echelon icons in the Bottom button
                GameObject downIcon0GO = bottomButtonT.Find("ICON").gameObject;
                GameObject downIcon1GO = bottomButtonT.Find("ICON (1)").gameObject;

                // Calculate the vertical space between them
                float downIconVertDiff = downIcon1GO.transform.position.y - downIcon0GO.transform.position.y;
                float downIconVertOffset = downIconVertDiff / 2.0f;

                // shift the icons to keep them more centered when the third one is added
                origScale = downIcon0GO.transform.localScale;
                newPosition = downIcon0GO.transform.position;
                newPosition.y -= downIconVertOffset;
                downIcon0GO.transform.position = newPosition;
                downIcon0GO.transform.localScale = origScale;
                origScale = downIcon1GO.transform.localScale;
                newPosition = downIcon1GO.transform.position;
                newPosition.y -= downIconVertOffset;
                downIcon1GO.transform.position = newPosition;
                downIcon1GO.transform.localScale = origScale;

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
                origScale = upIcon0GO.transform.localScale;
                newPosition = upIcon0GO.transform.position;
                newPosition.y += upIconVertOffset;
                upIcon0GO.transform.position = newPosition;
                upIcon0GO.transform.localScale = origScale;
                origScale = upIcon1GO.transform.localScale;
                newPosition = upIcon1GO.transform.position;
                newPosition.y += upIconVertOffset;
                upIcon1GO.transform.position = newPosition;
                upIcon1GO.transform.localScale = origScale;

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

                //===============================================================================================
                // Delete the original LoopScrollArrowController component and replace with the LSACWrapper instance
                //Component.Destroy(controllerLSAC);
                //===============================================================================================
            }
            catch (NullReferenceException ex)
            {
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }

        }

        //public void ScrollUpFast(object sender, VRTK.InteractableObjectEventArgs e)
        //{
        //    try
        //    {
        //        if (controllerLSAC.CurrentIndex - fastStepCount > 0)
        //        {
        //            controllerLSAC.CurrentIndex -= fastStepCount;
        //        }
        //        else
        //        {
        //            controllerLSAC.CurrentIndex = 0;
        //        }
        //        DoScrollMI.Invoke(controllerLSAC, args);
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        MelonLogger.Msg("Null reference exception: " + ex.Message);
        //        MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
        //    }
        //}

        //public void ScrollDownFast(object sender, VRTK.InteractableObjectEventArgs e)
        //{
        //    try
        //    {
        //        if (controllerLSAC.CurrentIndex + fastStepCount < loopScrollLSR.totalCount)
        //        {
        //            controllerLSAC.CurrentIndex += fastStepCount;
        //        } 
        //        else
        //        {
        //            controllerLSAC.CurrentIndex = loopScrollLSR.totalCount - 1;
        //        }
        //        DoScrollMI.Invoke(controllerLSAC, args);
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        MelonLogger.Msg("Null reference exception: " + ex.Message);
        //        MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
        //    }
        //}   
        
    }
}
