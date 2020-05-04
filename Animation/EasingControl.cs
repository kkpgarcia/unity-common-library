using UnityEngine;
using System;
using System.Collections;

using TweenEquation = System.Func<float, float, float, float>;

namespace Common.Animation {
	public class EasingControl : MonoBehaviour 
	{
		#region Events
		public event EventHandler UpdateEvent;
		public event EventHandler StateChangeEvent;
		public event EventHandler CompletedEvent;
		public event EventHandler LoopedEvent;
		#endregion

		#region Enums
		public enum TimeType
		{
			Normal,
			Real,
			Fixed,
		};

		public enum PlayState
		{
			Stopped,
			Paused,
			Playing,
			Reversing,
		};

		public enum EndBehaviour
		{
			Constant,
			Reset,
		};

		public enum LoopType
		{
			Repeat,
			PingPong,
		};
		#endregion

		#region Properties
		public AnimationProperty Property = new AnimationProperty();

		public EasingControl.PlayState playState { get; private set; }
		public EasingControl.PlayState previousPlayState { get; private set; }

		public bool IsPlaying { get { return playState == PlayState.Playing || playState == PlayState.Reversing; }}
		public float startValue = 0.0f;
		public float endValue = 1.0f;

		public float currentTime { get; private set; }
		public float currentValue { get; private set; }
		public float currentOffset { get; private set; }
		public int loops { get; private set; }
		

		#endregion

		#region Query
		public EasingControl SetTimeType(TimeType type) {
			this.Property.timeType = type;
			return this;
		}

		public EasingControl SetLoopType(LoopType type) {
			this.Property.loopType = type;
			return this;
		}

		public EasingControl SetLoopCount(int loopCount) {
			this.Property.loopCount = loopCount;
			return this;
		}

		public EasingControl SetAnimationCurve(AnimationCurve curve) {
			this.Property.curve = curve;
			Property.useAnimationCurve = this.Property.curve != null;
			return this;
		}

		public EasingControl SetEndBehaviour(EndBehaviour behaviour) {
			this.Property.endBehaviour = behaviour;
			return this;
		}

		public EasingControl SetEquation(TweenEquation eq) {
			this.Property.equation = eq;
			return this;
		}

		public EasingControl SetDuration(float duration) {
			this.Property.duration = duration;
			return this;
		}

		public EasingControl SetOnLoopEvent(EventHandler action) {
			this.StateChangeEvent += action;
			return this;
		}

		public EasingControl SetOnChangeEvent(EventHandler action) {
			this.StateChangeEvent += action;
			return this;
		}

		public EasingControl SetOnUpdate(EventHandler action) {
			this.UpdateEvent += action;
			return this;
		}

		public EasingControl SetOnFinish(EventHandler action) {
			this.CompletedEvent += action;
			return this;
		}

		#endregion
		
		#region MonoBehaviour
		void OnEnable ()
		{
			Resume();
		}
		
		void OnDisable ()
		{
			Pause();
		}
		#endregion

		#region Public
		public void Play ()
		{
			SetPlayState(PlayState.Playing);
		}
		
		public void Reverse ()
		{
			SetPlayState(PlayState.Reversing);
		}
		
		public void Pause ()
		{
			if (IsPlaying)
				SetPlayState(PlayState.Paused);
		}
		
		public void Resume ()
		{
			if (playState == PlayState.Paused)
				SetPlayState(previousPlayState);
		}
		
		public void Stop ()
		{
			SetPlayState(PlayState.Stopped);
			previousPlayState = PlayState.Stopped;
			loops = 0;
			if (Property.endBehaviour == EndBehaviour.Reset)
				SeekToBeginning ();
		}
		
		public void SeekToTime (float time)
		{
			currentTime = Mathf.Clamp01(time / Property.duration);
			float newValue = (endValue - startValue) * currentTime + startValue;
			currentOffset = newValue - currentValue;
			currentValue = newValue;
			OnUpdate();
		}
		
		public void SeekToBeginning ()
		{
			SeekToTime(0.0f);
		}
		
		public void SeekToEnd ()
		{
			SeekToTime(Property.duration);
		}
		#endregion

		#region Protected
		protected virtual void OnUpdate ()
		{
			if (UpdateEvent != null)
				UpdateEvent(this, EventArgs.Empty);
		}

		protected virtual void OnLoop ()
		{
			if (LoopedEvent != null)
				LoopedEvent(this, EventArgs.Empty);
		}

		protected virtual void OnComplete ()
		{
			if (CompletedEvent != null)
				CompletedEvent(this, EventArgs.Empty);
		}

		protected virtual void OnStateChange ()
		{
			if (StateChangeEvent != null)
				StateChangeEvent(this, EventArgs.Empty);
		}
		#endregion

		#region Private
		void SetPlayState (PlayState target)
		{
			if (isActiveAndEnabled)
			{
				if (playState == target)
					return;
				
				previousPlayState = playState;
				playState = target;
				OnStateChange();
				StopCoroutine("Ticker");
				if (IsPlaying)
					StartCoroutine("Ticker");
			}
			else
			{
				previousPlayState = target;
				playState = PlayState.Paused;
			}
		}

		IEnumerator Ticker ()
		{
			while (true)
			{
				switch (Property.timeType)
				{
				case TimeType.Normal:
					yield return new WaitForEndOfFrame();
					Tick(Time.deltaTime);
					break;
				case TimeType.Real:
					yield return new WaitForEndOfFrame();
					Tick(Time.unscaledDeltaTime);
					break;
				default: // Fixed
					yield return new WaitForFixedUpdate();
					Tick(Time.fixedDeltaTime);
					break;
				}
			}
		}

		void Tick (float time)
		{
			bool finished = false;
			if (playState == PlayState.Playing)
			{
				currentTime = Mathf.Clamp01( currentTime + (time / Property.duration));
				finished = Mathf.Approximately(currentTime, 1.0f);
			}
			else // Reversing
			{
				currentTime = Mathf.Clamp01( currentTime - (time / Property.duration));
				finished = Mathf.Approximately(currentTime, 0.0f);
			}

			float equationValue = Property.useAnimationCurve ? Property.curve.Evaluate(currentTime) : Property.equation (0.0f, 1.0f, currentTime);

			float frameValue = (endValue - startValue) * equationValue + startValue;
			currentOffset = frameValue - currentValue;
			currentValue = frameValue;
			OnUpdate();
			
			if (finished)
			{
				++loops;
				if (Property.loopCount < 0 || Property.loopCount >= loops) 
				{
					if (Property.loopType == LoopType.Repeat) 
						SeekToBeginning();
					else // PingPong
						SetPlayState( playState == PlayState.Playing ? PlayState.Reversing : PlayState.Playing );

					OnLoop();
				} 
				else
				{
					OnComplete();
					Stop ();
				}
			}
		}
		#endregion
	}
}