using UnityEngine;

namespace Common.Animation {
    [System.Serializable]
    public class TransformAnimationProperty : AnimationProperty {
        public Vector3 Vector3Value = Vector3.zero;
        public TransformAnimationProperty() : base() {}
        public TransformAnimationProperty(Vector3 value, bool simulate, Equation eq, EasingControl.TimeType timeType, EasingControl.EndBehaviour endBehaviour, 
                                EasingControl.LoopType loopType, float duration, int loopCount, AnimationCurve curve, bool useAnimationCurve)
                                : base(simulate, eq, timeType, endBehaviour, loopType, duration, loopCount, curve, useAnimationCurve) {

            this.Vector3Value = value;
        }
    }    
}