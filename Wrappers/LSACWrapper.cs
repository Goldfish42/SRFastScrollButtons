using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Util.Navigation;

namespace SRFastScrollButtons.Wrappers
{
    public class LSACWrapper : LoopScrollArrowController
    {
        private LoopScrollArrowController nativeLSAC;

        // Private Fields of the native class
        private LoopScrollRect loopScroll { get; set; }
        private int prevIndex { get; set; }

        // New Fields for the Wrapper
        private GameObject fastDownGameObject;
        private GameObject fastUpGameObject;
        private Animator fastDownAnimator;
        private Animator fastUpAnimator;

        private readonly int fastStepFactor = 10;
        private int fastStepCount = 0;

        // Reflection vars
        private static Type typeLSAC = typeof(LoopScrollArrowController);
        private static MethodInfo DoScrollMI;
        private static FieldInfo loopScrollFI;
        private static object[] argTrue = { true };
        private static FieldInfo prevIndexFI;

        private void Awake()
        {
            try
            {
                nativeLSAC = GetComponent<LoopScrollArrowController>();

                // Setup reflection vars
                DoScrollMI = typeLSAC.GetMethod("DoScroll", BindingFlags.NonPublic | BindingFlags.Instance);
                loopScrollFI = typeLSAC.GetField("loopScroll", BindingFlags.NonPublic | BindingFlags.Instance);
                prevIndexFI = typeLSAC.GetField("prevIndex", BindingFlags.NonPublic | BindingFlags.Instance);

                // Setup private native vars
                loopScroll = (LoopScrollRect)loopScrollFI.GetValue(nativeLSAC);
                prevIndex = (int)prevIndexFI.GetValue(nativeLSAC);

                // SANITY CHECK
                if (loopScroll.name != "Songs Scroll")
                {
                    loopScroll = null;
                    throw new Exception("Wrong LoopScrollRect found!");
                }

                // define the number of songs that each button press will skip
                fastStepCount = nativeLSAC.StepCount * fastStepFactor;
            }
            catch (NullReferenceException ex)
            {
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }
        }

        //public LSACWrapper(LoopScrollArrowController _nativeLSAC)
        //{
        //    try
        //    {
        //        nativeLSAC = _nativeLSAC;

        //        // Setup reflection vars
        //        DoScrollMI = typeLSAC.GetMethod("DoScroll", BindingFlags.NonPublic | BindingFlags.Instance);
        //        loopScrollFI = typeLSAC.GetField("loopScroll", BindingFlags.NonPublic | BindingFlags.Instance);
        //        prevIndexFI = typeLSAC.GetField("prevIndex", BindingFlags.NonPublic | BindingFlags.Instance);

        //        // Setup private native vars
        //        loopScroll = (LoopScrollRect)loopScrollFI.GetValue(nativeLSAC);
        //        prevIndex = (int)prevIndexFI.GetValue(nativeLSAC);

        //        // define the number of songs that each button press will skip
        //        fastStepCount = nativeLSAC.StepCount * fastStepFactor;
        //    }
        //    catch (NullReferenceException ex)
        //    {
        //        MelonLogger.Msg("Null reference exception: " + ex.Message);
        //        MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
        //    }
        //}

        public void ScrollUpFast(object sender, VRTK.InteractableObjectEventArgs e)
        {
            try
            {
                if (nativeLSAC.CurrentIndex - fastStepCount > 0)
                {
                    prevIndex = CurrentIndex;
                    prevIndexFI.SetValue(nativeLSAC, CurrentIndex);
                    nativeLSAC.CurrentIndex -= fastStepCount;
                }
                else
                {
                    prevIndex = CurrentIndex;
                    prevIndexFI.SetValue(nativeLSAC, CurrentIndex);
                    nativeLSAC.CurrentIndex = 0;
                }
                DoScrollMI.Invoke(nativeLSAC, argTrue);
            }
            catch (NullReferenceException ex)
            {
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }
        }

        public void ScrollDownFast(object sender, VRTK.InteractableObjectEventArgs e)
        {
            try
            {
                if (nativeLSAC.CurrentIndex + fastStepCount < loopScroll.totalCount)
                {
                    prevIndex = CurrentIndex;
                    prevIndexFI.SetValue(nativeLSAC, CurrentIndex);
                    nativeLSAC.CurrentIndex += fastStepCount;
                }
                else
                {
                    prevIndex = CurrentIndex;
                    prevIndexFI.SetValue(nativeLSAC, CurrentIndex);
                    nativeLSAC.CurrentIndex = loopScroll.totalCount - 1;
                }
                DoScrollMI.Invoke(nativeLSAC, argTrue);
            }
            catch (NullReferenceException ex)
            {
                MelonLogger.Msg("Null reference exception: " + ex.Message);
                MelonLogger.Msg("Stack Trace: " + ex.StackTrace);
            }
        }
    }
}
