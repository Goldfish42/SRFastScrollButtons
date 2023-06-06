using MelonLoader;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Util.Navigation;
using VRTK;

namespace SRFastScrollButtons.MonoBehaviors
{
    public class FastScrollArrowController : MonoBehaviour
    {
        private LoopScrollArrowController LSAC;

        // Token: 0x06001BD2 RID: 7122 RVA: 0x000A0990 File Offset: 0x0009EB90
        private void Awake()
        {
            LSAC = GetComponent<LoopScrollArrowController>();
            GameObject songsScroll = gameObject.transform.parent.Find("SongsList/Canvas/Songs Scroll").gameObject;

            // check for unwanted nulls
            if (LSAC == null) { MelonLogger.Msg("LSAC is null!"); }
            if (songsScroll == null) { MelonLogger.Msg("songsScroll is null!"); }

            upArrowGameObject = gameObject.transform.Find("Up").gameObject;
            downArrowGameObject = gameObject.transform.Find("Down").gameObject;
            topArrowGameObject = gameObject.transform.Find("Top").gameObject;
            bottomArrowGameObject = gameObject.transform.Find("Bottom").gameObject;
            fastUpArrowGameObject = gameObject.transform.Find("FastUp").gameObject;
            fastDownArrowGameObject = gameObject.transform.Find("FastDown").gameObject;

            this.upAnimator = this.upArrowGameObject.GetComponentInChildren<Animator>();
            this.downAnimator = this.downArrowGameObject.GetComponentInChildren<Animator>();

            // Defaults
            CurrentIndex = Game_InfoProvider.SongSelected.songIndex; // had to use CE to trace where this originated from!
            loopScroll = songsScroll.GetComponent<LoopVerticalScrollRect>();
            loopScroll.totalCount = 999999999; // hack to prevent DoScroll from resetting currentIndex back to 0 the first time it's executed
            fadeArrowsAutomatically = true;
            ScrollEasing = LSAC.ScrollEasing;
            UseTouchapdController = false; // it's TRUE in the LSAC, so I'm disabling here since I'm no longer attempting to override controller behavior
            fastStepCount = fastStepFactor * stepCount;
            //MelonLogger.Msg("stepCount = " + stepCount);
            //MelonLogger.Msg("fastStepFactor = " + fastStepFactor);
            //MelonLogger.Msg("fastStepCount = " + fastStepCount);
            //MelonLogger.Msg("currentIndex = " + currentIndex);
            //MelonLogger.Msg("loopScroll.totalCount = " + loopScroll.totalCount);

            // Original code
            if (this.upAnimator == null || this.downAnimator == null || fastUpAnimator == null || fastDownAnimator == null)
            {
                this.fadeArrowsAutomatically = false;
            }
            if (this.topArrowGameObject)
            {
                this.topAnimator = this.topArrowGameObject.GetComponentInChildren<Animator>();
            }
            if (this.bottomArrowGameObject)
            {
                this.bottomAnimator = this.bottomArrowGameObject.GetComponentInChildren<Animator>();
            }

            // SRFS
            if (fastUpArrowGameObject)
            {
                fastUpAnimator = fastUpArrowGameObject.GetComponentInChildren<Animator>();
            }
            if (fastDownArrowGameObject)
            {
                fastDownAnimator = fastDownArrowGameObject.GetComponentInChildren<Animator>();
            }

            // Original code
            //this.SetControllersEvents(); // this caused the controller thumbstick controls to become glitchy

            // check for unwanted nulls
            if (loopScroll == null ) { MelonLogger.Msg("loopScroll is null!"); }
            if (upArrowGameObject == null) { MelonLogger.Msg("upArrowGameObject is null!"); }
            if (downArrowGameObject == null) { MelonLogger.Msg("downArrowGameObject is null!"); }
            if (topArrowGameObject == null) { MelonLogger.Msg("topArrowGameObject is null!"); }
            if (bottomArrowGameObject == null) { MelonLogger.Msg("bottomArrowGameObject is null!"); }
            if (fastUpArrowGameObject == null) { MelonLogger.Msg("fastUpArrowGameObject is null!"); }
            if (fastDownArrowGameObject == null) { MelonLogger.Msg("fastDownArrowGameObject is null!"); }
            if (ScrollEasing == null) { MelonLogger.Msg("ScrollEasing is null!"); }
            if (upAnimator == null) { MelonLogger.Msg("upAnimator is null!"); }
            if (downAnimator == null) { MelonLogger.Msg("downAnimator is null!"); }
            if (topAnimator == null) { MelonLogger.Msg("topAnimator is null!"); }
            if (bottomAnimator == null) { MelonLogger.Msg("bottomAnimator is null!"); }
            if (fastUpAnimator == null) { MelonLogger.Msg("fastUpAnimator is null!"); }
            if (fastDownAnimator == null) { MelonLogger.Msg("fastDownAnimator is null!"); }
            if (rightEvents == null) { MelonLogger.Msg("rightEvents is null!"); }
            if (leftEvents == null) { MelonLogger.Msg("leftEvents is null!"); }
        }

        // Token: 0x06001BD3 RID: 7123 RVA: 0x000A0A24 File Offset: 0x0009EC24
        private void OnEnable()
        {
            this.DoScroll(false);
            SceneManager.sceneLoaded += this.OnSceneLoaded;
        }

        // Token: 0x06001BD4 RID: 7124 RVA: 0x000A0A40 File Offset: 0x0009EC40
        private void LateUpdate() // what calls this in the native LSAC?
        {
            if (this.hideArrowsAutomatically)
            {
                if (this.CurrentIndex + this.StepCount < this.loopScroll.totalCount)
                {
                    this.upArrowGameObject.SetActive(true);
                    if (this.bottomArrowGameObject)
                    {
                        this.bottomArrowGameObject.SetActive(true);
                    }
                    if (fastDownArrowGameObject)
                    {
                        fastDownArrowGameObject.SetActive(true);
                    }
                }
                else
                {
                    this.upArrowGameObject.SetActive(false);
                    if (this.bottomArrowGameObject)
                    {
                        this.bottomArrowGameObject.SetActive(false); // SRFS fixed this logic?
                    }
                    if (fastDownArrowGameObject)
                    {
                        fastDownArrowGameObject.SetActive(false);
                    }
                }

                if (this.CurrentIndex >= this.StepCount || (this.CurrentIndex > 0 && this.loopScroll.totalCount > this.stepCount))
                {
                    this.downArrowGameObject.SetActive(true);
                    if (this.topArrowGameObject)
                    {
                        this.topArrowGameObject.SetActive(true);
                    }
                    if (fastUpArrowGameObject)
                    {
                        fastUpArrowGameObject.SetActive(true);
                    }
                }
                else
                {
                    this.downArrowGameObject.SetActive(false);
                    if (this.topArrowGameObject)
                    {
                        this.topArrowGameObject.SetActive(false);
                    }
                    if (fastUpArrowGameObject)
                    {
                        fastUpArrowGameObject.SetActive(false);
                    }
                }
            }
            else if (this.fadeArrowsAutomatically)
            {
                if (this.CurrentIndex + this.StepCount < this.loopScroll.totalCount)
                {
                    if (this.upFaded)
                    {
                        this.upAnimator.SetTrigger("Normal");
                        if (this.bottomAnimator)
                        {
                            this.bottomAnimator.SetTrigger("Normal");
                        }
                        if (fastDownAnimator)
                        {
                            fastDownAnimator.SetTrigger("Normal");
                        }
                        this.upFaded = false;
                    }
                }
                else if (!this.upFaded)
                {
                    this.upAnimator.SetTrigger("Disabled");
                    if (this.bottomAnimator)
                    {
                        this.bottomAnimator.SetTrigger("Disabled");
                    }
                    if (fastDownAnimator)
                    {
                        fastDownAnimator.SetTrigger("Disabled");
                    }
                    this.upFaded = true;
                }
                if (this.CurrentIndex >= this.StepCount || (this.CurrentIndex > 0 && this.loopScroll.totalCount > this.stepCount))
                {
                    if (this.downFaded)
                    {
                        this.downAnimator.SetTrigger("Normal");
                        if (this.topAnimator)
                        {
                            this.topAnimator.SetTrigger("Normal");
                        }
                        if (fastUpAnimator)
                        {
                            fastUpAnimator.SetTrigger("Normal");
                        }
                        this.downFaded = false;
                    }
                }
                else if (!this.downFaded)
                {
                    this.downAnimator.SetTrigger("Disabled");
                    if (this.topAnimator)
                    {
                        this.topAnimator.SetTrigger("Disabled");
                    }
                    if (fastUpAnimator)
                    {
                        fastUpAnimator.SetTrigger("Disabled");
                    }
                    this.downFaded = true;
                }
            }
            if (this.WaitForTouchpadRest)
            {
                this.lastTouchpadInteraction += Time.deltaTime;
                if (this.lastTouchpadInteraction >= 0.5f)
                {
                    this.lastTouchpadInteraction = 0f;
                    this.WaitForTouchpadRest = false;
                }
            }
        }

        public void ScrollDownHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollDownHover();
        }

        // Token: 0x06001BD5 RID: 7125 RVA: 0x000A0CBE File Offset: 0x0009EEBE
        public void ScrollDownHover()
        {
            if (!this.downFaded)
            {
                this.downAnimator.SetTrigger("Highlighted");
            }
        }

        public void ScrollDownOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollDownOut();
        }

        // Token: 0x06001BD6 RID: 7126 RVA: 0x000A0CD8 File Offset: 0x0009EED8
        public void ScrollDownOut()
        {
            if (!this.downFaded)
            {
                this.downAnimator.SetTrigger("Normal");
            }
        }

        public void ScrollUpHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollUpHover();
        }

        // Token: 0x06001BD7 RID: 7127 RVA: 0x000A0CF2 File Offset: 0x0009EEF2
        public void ScrollUpHover()
        {
            if (!this.upFaded)
            {
                this.upAnimator.SetTrigger("Highlighted");
            }
        }

        public void ScrollUpOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollUpOut();
        }

        // Token: 0x06001BD8 RID: 7128 RVA: 0x000A0D0C File Offset: 0x0009EF0C
        public void ScrollUpOut()
        {
            if (!this.upFaded)
            {
                this.upAnimator.SetTrigger("Normal");
            }
        }

        public void ScrollTopHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollTopHover();
        }

        // Token: 0x06001BD9 RID: 7129 RVA: 0x000A0D26 File Offset: 0x0009EF26
        public void ScrollTopHover()
        {
            if (!this.downFaded)
            {
                this.topAnimator.SetTrigger("Highlighted");
            }
        }

        public void ScrollTopOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollTopOut();
        }

        // Token: 0x06001BDA RID: 7130 RVA: 0x000A0D40 File Offset: 0x0009EF40
        public void ScrollTopOut()
        {
            if (!this.downFaded)
            {
                this.topAnimator.SetTrigger("Normal");
            }
        }

        public void ScrollBottomHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollBottomHover();
        }

        // Token: 0x06001BDB RID: 7131 RVA: 0x000A0D5A File Offset: 0x0009EF5A
        public void ScrollBottomHover()
        {
            if (!this.upFaded)
            {
                this.bottomAnimator.SetTrigger("Highlighted");
            }
        }

        public void ScrollBottomOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollBottomOut();
        }

        // Token: 0x06001BDC RID: 7132 RVA: 0x000A0D74 File Offset: 0x0009EF74
        public void ScrollBottomOut()
        {
            if (!this.upFaded)
            {
                this.bottomAnimator.SetTrigger("Normal");
            }
        }

        // SRFS
        public void FastScrollDownHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollDownHover();
        }

        // SRFS
        public void FastScrollDownHover()
        {
            if (!downFaded)
            {
                fastDownAnimator.SetTrigger("Highlighted");
            }
        }

        // SRFS
        public void FastScrollDownOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollDownOut();
        }

        // SRFS
        public void FastScrollDownOut()
        {
            if (!downFaded)
            {
                fastDownAnimator.SetTrigger("Normal");
            }
        }

        // SRFS
        public void FastScrollUpHover(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollUpHover();
        }

        // SRFS
        public void FastScrollUpHover()
        {
            if (!upFaded)
            {
                fastUpAnimator.SetTrigger("Highlighted");
            }
        }

        // SRFS
        public void FastScrollUpOut(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollUpOut();
        }

        // SRFS
        public void FastScrollUpOut()
        {
            if (!upFaded)
            {
                fastUpAnimator.SetTrigger("Normal");
            }
        }

        // Token: 0x06001BDD RID: 7133 RVA: 0x000A0D90 File Offset: 0x0009EF90
        private void DoScroll(bool quick = false)
        {
            if (CurrentIndex % this.StepCount > 0)
            {
                this.CurrentIndex += this.StepCount - this.CurrentIndex % this.StepCount;
            }
            this.CurrentIndex = Mathf.Clamp(this.CurrentIndex, 0, this.loopScroll.totalCount - 1);
            if (this.CurrentIndex == this.loopScroll.totalCount - 1)
            {
                this.CurrentIndex = this.loopScroll.totalCount - this.StepCount;
                if (this.CurrentIndex < 0)
                {
                    this.CurrentIndex = 0;
                }
            }
            if (quick)
            {
                this.loopScroll.RefillCells(this.CurrentIndex);
                return;
            }
            this.loopScroll.SrollToCell(this.CurrentIndex, this.scrollSpeed, this.scrollDrag);
        }

        public void ScrollUp(object sender, VRTK.InteractableObjectEventArgs e)        
        {            
            ScrollUp();
        }

        // Token: 0x06001BDE RID: 7134 RVA: 0x000A0E5C File Offset: 0x0009F05C
        public void ScrollUp()
        {
            if (this.CurrentIndex < this.loopScroll.totalCount)
            {
                this.prevIndex = this.CurrentIndex;
                this.CurrentIndex += this.StepCount;
                //this.DoScroll(false);
                DoScroll(true);
            }
        }

        public void ControllerScrollUp()
        {
            if (this.CurrentIndex < this.loopScroll.totalCount)
            {
                this.prevIndex = this.CurrentIndex;
                this.CurrentIndex += this.StepCount;
                this.DoScroll(false);
            }
        }

        public void ScrollDown(object sender, VRTK.InteractableObjectEventArgs e)
        {
            SrollDown();
        }

        // Token: 0x06001BDF RID: 7135 RVA: 0x000A0E97 File Offset: 0x0009F097
        public void SrollDown()
        {
            if (this.CurrentIndex > 0)
            {
                this.prevIndex = this.CurrentIndex;
                this.CurrentIndex -= this.StepCount;
                //this.DoScroll(false);
                DoScroll(true);
            }
        }
        
        public void ControllerScrollDown()
        {
            if (this.CurrentIndex < this.loopScroll.totalCount)
            {
                this.prevIndex = this.CurrentIndex;
                this.CurrentIndex += this.StepCount;
                this.DoScroll(false);
            }
        }

        public void ScrollTop(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollTop();
        }

        // Token: 0x06001BE0 RID: 7136 RVA: 0x000A0EC8 File Offset: 0x0009F0C8
        public void ScrollTop()
        {
            if (this.CurrentIndex > 0)
            {
                this.CurrentIndex = 0;
                this.DoScroll(true);
            }
        }

        public void ScrollBottom(object sender, VRTK.InteractableObjectEventArgs e)
        {
            ScrollBottom();
        }

        // Token: 0x06001BE1 RID: 7137 RVA: 0x000A0EE1 File Offset: 0x0009F0E1
        public void ScrollBottom()
        {
            if (this.CurrentIndex < this.loopScroll.totalCount)
            {
                this.CurrentIndex = this.loopScroll.totalCount - 1;
                this.DoScroll(true);
            }
        }

        // SRFS
        public void FastScrollUp(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollUp();
        }

        public void FastScrollUp()
        {
            if (CurrentIndex - fastStepCount > 0)
            {
                //prevIndex = currentIndex;
                CurrentIndex -= fastStepCount;
            }
            else
            {
                //prevIndex = currentIndex;
                CurrentIndex = 0;
            }
            DoScroll(true);
        }

        // SRFS
        public void FastScrollDown(object sender, VRTK.InteractableObjectEventArgs e)
        {
            FastScrollDown();
        }

        public void FastScrollDown()
        {
            if (CurrentIndex + fastStepCount < loopScroll.totalCount)
            {
                //prevIndex = currentIndex;
                CurrentIndex += fastStepCount;
            }
            else
            {
                //prevIndex = currentIndex;
                CurrentIndex = loopScroll.totalCount - 1;
            }
            DoScroll(true);
        }

        // Token: 0x1700046E RID: 1134
        // (get) Token: 0x06001BE2 RID: 7138 RVA: 0x000A0F10 File Offset: 0x0009F110
        // (set) Token: 0x06001BE3 RID: 7139 RVA: 0x000A0F18 File Offset: 0x0009F118
        public int CurrentIndex
        {
            get
            {
                currentIndex = LSAC.CurrentIndex; // SRFS
                return this.currentIndex;
            }
            set
            {
                this.currentIndex = value;
                LSAC.CurrentIndex = currentIndex; // SRFS
            }
        }

        // Token: 0x1700046F RID: 1135
        // (get) Token: 0x06001BE4 RID: 7140 RVA: 0x000A0F21 File Offset: 0x0009F121
        // (set) Token: 0x06001BE5 RID: 7141 RVA: 0x000A0F29 File Offset: 0x0009F129
        public int StepCount
        {
            get
            {
                return this.stepCount;
            }
            set
            {
                this.stepCount = value;
            }
        }

        // Token: 0x06001BE6 RID: 7142 RVA: 0x000A0F32 File Offset: 0x0009F132
        public void OnDisable()
        {
            SceneManager.sceneLoaded -= this.OnSceneLoaded;
        }

        // Token: 0x06001BE7 RID: 7143 RVA: 0x000A0F45 File Offset: 0x0009F145
        private void OnSceneLoaded(Scene scene, LoadSceneMode loadingMode)
        {
            this.SetControllersEvents();
        }

        // Token: 0x06001BE8 RID: 7144 RVA: 0x000A0F4D File Offset: 0x0009F14D
        private void OnDestroy()
        {
            this.RemoveControllerEvents();
        }

        // Token: 0x06001BE9 RID: 7145 RVA: 0x000A0F58 File Offset: 0x0009F158
        private void SetControllersEvents()
        {
            if (!this.UseTouchapdController)
            {
                return;
            }
            try
            {
                if (this.rightEvents == null || this.leftEvents == null)
                {
                    GameObject gameObject = GameObject.Find("RightController");
                    GameObject gameObject2 = GameObject.Find("LeftController");
                    this.RemoveControllerEvents();
                    this.rightEvents = (((gameObject != null) ? gameObject.GetComponent<VRTK_ControllerEvents>() : null) ?? null);
                    this.leftEvents = (((gameObject2 != null) ? gameObject2.GetComponent<VRTK_ControllerEvents>() : null) ?? null);
                    this.AddControllerEvents();
                }
            }
            catch
            {
            }
        }

        // Token: 0x06001BEA RID: 7146 RVA: 0x000A0FF0 File Offset: 0x0009F1F0
        private void AddControllerEvents()
        {
            if (this.rightEvents != null)
            {
                this.rightEvents.TouchpadAxisChanged += this.OnTouchPadAxisChanged;
            }
            if (this.leftEvents != null)
            {
                this.leftEvents.TouchpadAxisChanged += this.OnTouchPadAxisChanged;
            }
        }

        // Token: 0x06001BEB RID: 7147 RVA: 0x000A1048 File Offset: 0x0009F248
        private void RemoveControllerEvents()
        {
            if (this.rightEvents != null)
            {
                this.rightEvents.TouchpadAxisChanged -= this.OnTouchPadAxisChanged;
            }
            if (this.leftEvents != null)
            {
                this.leftEvents.TouchpadAxisChanged -= this.OnTouchPadAxisChanged;
            }
        }

        // Token: 0x06001BEC RID: 7148 RVA: 0x000A10A0 File Offset: 0x0009F2A0
        private void OnTouchPadAxisChanged(object sender, ControllerInteractionEventArgs e)
        {
            if (!base.gameObject.activeInHierarchy)
            {
                return;
            }
            Vector2 touchpadAxis = e.touchpadAxis;
            if (!this.WaitForTouchpadRest)
            {
                this.WaitForTouchpadRest = true;
                if (touchpadAxis.y > 0f)
                {
                    this.ControllerScrollDown();
                    return;
                }
                if (touchpadAxis.y < 0f)
                {
                    this.ControllerScrollUp();
                }
            }
        }

        // Token: 0x04001EDF RID: 7903
        [SerializeField]
        private LoopScrollRect loopScroll;

        // Token: 0x04001EE0 RID: 7904
        [SerializeField]
        private float scrollSpeed = 2000f;

        // Token: 0x04001EE1 RID: 7905
        [SerializeField]
        private float scrollDrag = 3500f;

        // Token: 0x04001EE2 RID: 7906
        [SerializeField]
        private int stepCount = 5;

        // SRFS
        [SerializeField]
        private int fastStepFactor = 10;

        // SRFS
        [SerializeField]
        private int fastStepCount = 0;

        // Token: 0x04001EE3 RID: 7907
        [SerializeField]
        private GameObject upArrowGameObject;

        // Token: 0x04001EE4 RID: 7908
        [SerializeField]
        private GameObject downArrowGameObject;

        // Token: 0x04001EE5 RID: 7909
        [SerializeField]
        private GameObject topArrowGameObject;

        // Token: 0x04001EE6 RID: 7910
        [SerializeField]
        private GameObject bottomArrowGameObject;

        // SRFS
        [SerializeField]
        private GameObject fastUpArrowGameObject;

        // SRFS
        [SerializeField]
        private GameObject fastDownArrowGameObject;

        // Token: 0x04001EE7 RID: 7911
        [SerializeField]
        public AnimationCurve ScrollEasing;

        // Token: 0x04001EE8 RID: 7912
        [SerializeField]
        public float scrollDuration = 1f;

        // Token: 0x04001EE9 RID: 7913
        [SerializeField]
        private bool hideArrowsAutomatically;

        // Token: 0x04001EEA RID: 7914
        [SerializeField]
        private bool fadeArrowsAutomatically;

        // Token: 0x04001EEB RID: 7915
        private bool upFaded;

        // Token: 0x04001EEC RID: 7916
        private bool downFaded;

        // Token: 0x04001EED RID: 7917
        private int currentIndex;

        // Token: 0x04001EEE RID: 7918
        private int prevIndex;

        // Token: 0x04001EEF RID: 7919
        private Animator upAnimator;

        // Token: 0x04001EF0 RID: 7920
        private Animator downAnimator;

        // Token: 0x04001EF1 RID: 7921
        private Animator topAnimator;

        // Token: 0x04001EF2 RID: 7922
        private Animator bottomAnimator;

        // SRFS
        private Animator fastUpAnimator;

        // SRFS
        private Animator fastDownAnimator;

        // Token: 0x04001EF3 RID: 7923
        private VRTK_ControllerEvents rightEvents;

        // Token: 0x04001EF4 RID: 7924
        private VRTK_ControllerEvents leftEvents;

        // Token: 0x04001EF5 RID: 7925
        [SerializeField]
        private bool UseTouchapdController;

        // Token: 0x04001EF6 RID: 7926
        private bool WaitForTouchpadRest;

        // Token: 0x04001EF7 RID: 7927
        private float lastTouchpadInteraction;

        // Token: 0x04001EF8 RID: 7928
        private const float TOUCHPAD_INTERVAL = 0.5f;
    }
}
