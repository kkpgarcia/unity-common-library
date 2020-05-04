﻿using UnityEngine;
using System;
using System.Collections;

namespace Common.Tween
{
    public class EasingControl : MonoBehaviour {

        public event EventHandler updateEvent;
        public event EventHandler stateChangeEvent;
        public event EventHandler completedEvent;
        public event EventHandler loopedEvent;

        public enum TimeType { Normal, Real, Fixed, };
        public enum PlayState { Stopped, Paused, Playing, Reversing, };
        public enum EndBehaviour { Constant, Reset, };
        public enum LoopType { Repeat, PingPong, };

        public TimeType timeType = TimeType.Normal;
        public PlayState playState { get; private set; }
        public PlayState previousPlayState { get; private set; }
        public EndBehaviour endBehaviour = EndBehaviour.Constant;
        public LoopType loopType = LoopType.Repeat;
        public bool IsPlaying { get { return playState == PlayState.Playing || playState == PlayState.Reversing; }}

        public float startValue = 0.0f;
        public float endValue = 1.0f;
        public float duration = 1.0f;
        public int loopCount = 0;
        public Func<float, float, float, float> equation = EasingEquations.Linear;

        public float currentTime { get; private set; }
        public float currentValue { get; private set; }
        public float currentOffset { get; private set; }
        public int loops { get; private set; }

        void OnEnable ()
        {
            Resume();
            
        }

        void OnDisable()
        {
            Pause();
        }

        public void Play()
        {
            SetPlayState(PlayState.Playing);
        }

        public void Reverse()
        {
            SetPlayState(PlayState.Reversing);
        }

        public void Pause()
        {
            SetPlayState(PlayState.Paused);
        }

        public void Resume()
        {
            SetPlayState(previousPlayState);
        }

        public void Stop()
        {
            SetPlayState(PlayState.Stopped);
            loops = 0;
            if (endBehaviour == EndBehaviour.Reset)
                SeekToBeginning();
        }

        public void SeekToTime(float time)
        {
            currentTime = Mathf.Clamp01(time / duration);
            float newValue = (endValue - startValue) * currentTime + startValue;
            currentOffset = newValue - currentValue;
            currentValue = newValue;

            if (updateEvent != null)
                updateEvent(this, EventArgs.Empty);
        }

        public void SeekToBeginning()
        {
            SeekToTime(0.0f);
        }

        public void SeekToEnd()
        {
            SeekToTime(duration);
        }

        void SetPlayState(PlayState target)
        {
            if (playState == target)
                return;

            previousPlayState = playState;
            playState = target;

            if (stateChangeEvent != null)
                stateChangeEvent(this, EventArgs.Empty);

            StopCoroutine("Ticker");
            if (IsPlaying)
                StartCoroutine("Ticker");
        }

        IEnumerator Ticker()
        {
            while (true)
            {
                switch (timeType)
                {
                    case TimeType.Normal:
                        yield return new WaitForEndOfFrame();
                        Tick(Time.deltaTime);
                        break;
                    case TimeType.Real:
                        yield return new WaitForEndOfFrame();
                        Tick(Time.unscaledDeltaTime);
                        break;
                    default:
                        yield return new WaitForFixedUpdate();
                        Tick(Time.fixedDeltaTime);
                        break;
                }
            }
        }

        void Tick(float time)
        {
            bool finished = false;
            if (playState == PlayState.Playing)
            {
                currentTime = Mathf.Clamp01(currentTime + (time / duration));
                finished = Mathf.Approximately(currentTime, 1.0f);
            }
            else
            {
                currentTime = Mathf.Clamp01(currentTime - (time / duration));
                finished = Mathf.Approximately(currentTime, 0.0f);
            }

            float frameValue = (endValue - startValue) * equation(0.0f, 1.0f, currentTime) + startValue;
            currentOffset = frameValue - currentValue;
            currentValue = frameValue;

            if (updateEvent != null)
                updateEvent(this, EventArgs.Empty);

            if (finished)
            {
                ++loops;
                if (loopCount < 0 || loopCount >= loops)
                {
                    if (loopType == LoopType.Repeat)
                        SeekToBeginning();
                    else
                        SetPlayState(playState == PlayState.Playing ? PlayState.Reversing : PlayState.Playing);

                    if (loopedEvent != null)
                        loopedEvent(this, EventArgs.Empty);
                }
                else
                {
                    if (completedEvent != null)
                        completedEvent(this, EventArgs.Empty);

                    Stop();
                }
            }
        }
    }
}