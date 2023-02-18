using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Yurowm.GameCore;

namespace _Yurowm.UltimateMatchThree.Features.Hand
{
    public class HandBooster : IMultipleUseBooster
    {
        public override bool ShowButton => true;

        public float swap_duration = 0.2f;
        Slot slot1;
        Slot slot2;
        RaycastHit2D hit;
        Vector2 startPoint = new Vector2();
        Slot pressedSlot = null;
        bool swaping = false;

        public override string FirstMessage() {
            return LocalizationAssistant.main[FirstMessageLocalizedKey()];
        }
    
        public override void Initialize() {
            slot1 = null;
            slot2 = null;
            base.Initialize();
        }

        public override IEnumerator Logic() {
            List<Slot> targets = Slot.allActive.Values.ToList();
            targets.ForEach(x => x.Highlight());

            ControlAssistant.main.ChangeMode(ControlAssistant.ControlMode.Regular);
            ControlAssistant.main.onControl += Control;

            while ((!slot1 && !slot2) && !IsCanceled())
                yield return new WaitForSeconds(0.1f);

            ControlAssistant.main.ChangeMode(ControlAssistant.ControlMode.Regular);
            ControlAssistant.main.onControl -= Control;
            targets.ForEach(x => x.Unlight());

            if (!IsCanceled())
            {
                yield return SwapByPlayerRoutine(slot1, slot2);
            
                SessionInfo.current.rule.matchDate++;
            }

            BoosterAssistant.main.boosterMode = null;
        }

        public override IEnumerator LogicOnGameStart()
        {
            yield return Logic();
        }

        private void Control(bool isBegan, bool isPress, bool IsOverUI, Vector2 point) {
            if (isBegan) {
                if (IsOverUI) return;
                startPoint = point;
                hit = Physics2D.Raycast(startPoint, Vector2.zero);
                if (!hit.transform) return;
                pressedSlot = hit.transform.GetComponent<Slot>();
            }
            if (isPress && pressedSlot) {
                if (IsOverUI) return;
                Vector2 move = point - startPoint;
                if (move.magnitude > Project.main.slot_offset / 2) {              
                    foreach (Side side in Utils.straightSides)
                        if (Vector2.Angle(move, side.X() * Vector2.right + side.Y() * Vector2.up) <= 45)
                            if (pressedSlot.chip)
                                SwapByPlayer(pressedSlot, side);
                    pressedSlot = null;
                }
            }
        }

        #region Swapping
        void SwapByPlayer(Slot slot, Side side)
        {
            if (slot && slot[side])
            {
                slot1 = slot;
                slot2 = slot[side];
            }
        }
        
        internal IEnumerator SwapByPlayerRoutine(Slot a, Slot b) {
            if (!SessionInfo.current.isPlaying) yield break;

            if (swaping) yield break; // If the process is already running
            if (!a || !b) yield break; // If one of the chips is missing
            if (a == b) yield break;
            if (!Slot.allActive.ContainsValue(a) || !Slot.allActive.ContainsValue(b)) yield break;
            if (!(a.GetCurrentContent() is IChip) || !(b.GetCurrentContent() is IChip)) yield break;

            IChip chipA = a.chip;
            IChip chipB = b.chip;

            if (!Slot.IsInteractive(chipA.slot) ||
                !Slot.IsInteractive(chipB.slot))
                yield break;

            var success = false;

            swaping = true;

            Vector3 posA = a.transform.position;
            Vector3 posB = b.transform.position;

            float progress = 0;
            float time = 0;

            Vector3 normal = a.x == b.x ? Vector3.right : Vector3.up;

            while (progress < swap_duration)
            {
                time = EasingFunctions.easeInOutQuad(progress / swap_duration);

                chipA.transform.position = Vector3.Lerp(posA, posB, time) + normal * Mathf.Sin(3.14f * time) * .2f;
                chipB.transform.position = Vector3.Lerp(posB, posA, time) - normal * Mathf.Sin(3.14f * time) * .2f;

                progress += Time.deltaTime;

                yield return 0;
            } 

            chipA.transform.position = posB;
            chipB.transform.position = posA;

            a.chip = chipB;
            b.chip = chipA;

            Project.onSwapSuccess.Invoke();
            SessionInfo.current.moveTimer.Move();
            SessionInfo.current.swapEvent++;

            swaping = false;
        }

        #endregion
    }
}