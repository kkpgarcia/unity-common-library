using System;

using UnityEngine;

using Common.AI.Fsm;
using Common.CustomTime;

namespace Common.AI.Fsm.Action
{
    public class TimedFadeVolume : FsmActionAdapter
    {
        private AudioSource audioSource;
        private float volumeFrom;
        private float volumeTo;
        private float duration;
        private string finishEvent;

        private CountdownTimer timer;
        
        public TimedFadeVolume(FsmState owner, string timeReferenceName) : base(owner)
        {
            this.timer = new CountdownTimer(1, timeReferenceName);
        }       

        public void Init(AudioSource audioSource, float volumeFrom, float volumeTo, float duration, string finishEvent)
        {
            this.audioSource = audioSource;
            this.volumeFrom = volumeFrom;
            this.volumeTo = volumeTo;
            this.duration = duration;
            this.finishEvent = finishEvent;
        }

        public override void OnEnter()
        {
            this.timer.Reset(this.duration);
            this.audioSource.volume = this.volumeFrom;
        }

        public override void OnUpdate()
        {
            this.timer.Update();
            
            if(this.timer.HasElapsed())
            {
                Finish();
            }

            this.audioSource.volume = Mathf.Lerp(this.volumeFrom, this.volumeTo, this.timer.GetRatio());
        }

        private void Finish()
        {
            this.audioSource.volume = this.volumeTo;

            if (!string.IsNullOrEmpty(this.finishEvent))
            {
                GetOwner().SendEvent(this.finishEvent);
            }
        }
    }
}