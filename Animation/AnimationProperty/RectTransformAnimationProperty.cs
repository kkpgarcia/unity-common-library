using UnityEngine;

namespace Common.Animation {
    [System.Serializable]
    public class RectTransformAnimationProperty : AnimationProperty {
        public Vector2 Offset = Vector2.zero;

        public RectTransformAnimationProperty() {}

        public RectTransformAnimationProperty(Vector2 value, bool simulate, Equation eq, EasingControl.TimeType timeType, EasingControl.EndBehaviour endBehaviour, 
                                EasingControl.LoopType loopType, float duration, int loopCount, AnimationCurve curve, bool useAnimationCurve)
                                : base(simulate, eq, timeType, endBehaviour, loopType, duration, loopCount, curve, useAnimationCurve) {

            this.Offset = value;
        }
    }    
}