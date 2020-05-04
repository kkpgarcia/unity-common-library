using UnityEngine;
using TweenEquation = System.Func<float, float, float, float>;

namespace Common.Animation {
    [System.Serializable]
    public class AnimationProperty {
        public EasingControl.TimeType timeType = EasingControl.TimeType.Normal;
		public EasingControl.EndBehaviour endBehaviour = EasingControl.EndBehaviour.Constant;
		public EasingControl.LoopType loopType = EasingControl.LoopType.Repeat;
        public float duration = 1.0f;
        public int loopCount = 0;
        public AnimationCurve curve;
		public bool useAnimationCurve = false;
		public TweenEquation equation = EasingEquations.Linear;
    }
}