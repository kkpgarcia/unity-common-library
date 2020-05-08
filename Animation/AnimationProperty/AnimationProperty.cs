using UnityEngine;
using TweenEquation = System.Func<float, float, float, float>;

namespace Common.Animation {
    [System.Serializable]
    public class AnimationProperty {
        public bool Simulate = true;
        public Equation TweenEquation = Equation.LINEAR;
        public EasingControl.TimeType timeType = EasingControl.TimeType.Normal;
		public EasingControl.EndBehaviour endBehaviour = EasingControl.EndBehaviour.Constant;
		public EasingControl.LoopType loopType = EasingControl.LoopType.Repeat;
        public float duration = 1.0f;
        public int loopCount = 0;
        public AnimationCurve curve;
		public bool useAnimationCurve = false;

        public AnimationProperty() {}

        public AnimationProperty(bool simulate, Equation eq, EasingControl.TimeType timeType, EasingControl.EndBehaviour endBehaviour, 
                                EasingControl.LoopType loopType, float duration, int loopCount, AnimationCurve curve, bool useAnimationCurve) {

            this.Simulate = simulate;
            this.TweenEquation = eq;
            this.timeType = timeType;
            this.endBehaviour = endBehaviour;
            this.loopCount = loopCount;
            this.loopType = loopType;
            this.duration = duration;
            this.curve = curve;
            this.useAnimationCurve = useAnimationCurve;
        }

        private TweenEquation m_equation;
		public TweenEquation equation {
            get {
                if(m_equation != null)
                    return m_equation;

                switch(this.TweenEquation) {
                    case Equation.SPRING:
                        return EasingEquations.Spring;
                    case Equation.EASE_IN_QUAD:
                        return EasingEquations.EaseInQuad;
                    case Equation.EASE_OUT_QUAD:
                        return EasingEquations.EaseOutQuad;
                    case Equation.EASE_IN_OUT_QUAD:
                        return EasingEquations.EaseInOutQuad;
                    case Equation.EASE_IN_CUBIC:
                        return EasingEquations.EaseInCubic;
                    case Equation.EASE_OUT_CUBIC:
                        return EasingEquations.EaseOutCubic;
                    case Equation.EASE_IN_OUT_CUBIC:
                        return EasingEquations.EaseInOutCubic;
                    case Equation.EASE_IN_QUART:
                        return EasingEquations.EaseInQuart;
                    case Equation.EASE_OUT_QUART:
                        return EasingEquations.EaseOutQuart;
                    case Equation.EASE_IN_OUT_QUART:
                        return EasingEquations.EaseInOutQuart;
                    case Equation.EASE_IN_QUINT:
                        return EasingEquations.EaseInQuint;
                    case Equation.EASE_OUT_QUINT:
                        return EasingEquations.EaseOutQuint;
                    case Equation.EASE_IN_OUT_QUINT:
                        return EasingEquations.EaseInOutQuint;
                    case Equation.EASE_IN_SINE:
                        return EasingEquations.EaseInSine;
                    case Equation.EASE_OUT_SINE:
                        return EasingEquations.EaseOutSine;
                    case Equation.EASE_IN_OUT_SINE:
                        return EasingEquations.EaseInOutSine;
                    case Equation.EASE_IN_EXPO:
                        return EasingEquations.EaseInExpo;
                    case Equation.EASE_OUT_EXPO:
                        return EasingEquations.EaseOutExpo;
                    case Equation.EASE_IN_OUT_EXPO:
                        return EasingEquations.EaseInOutExpo;
                    case Equation.EASE_IN_CIRC:
                        return EasingEquations.EaseInCirc;
                    case Equation.EASE_OUT_CIRC:
                        return EasingEquations.EaseOutCirc;
                    case Equation.EASE_IN_OUT_CIRC:
                        return EasingEquations.EaseInOutCirc;
                    case Equation.EASE_IN_BOUNCE:
                        return EasingEquations.EaseInBounce;
                    case Equation.EASE_OUT_BOUNCE:
                        return EasingEquations.EaseOutBounce;
                    case Equation.EASE_IN_OUT_BOUNCE:
                        return EasingEquations.EaseInOutBounce;
                    case Equation.EASE_IN_BACK:
                        return EasingEquations.EaseInBack;
                    case Equation.EASE_OUT_BACK:
                        return EasingEquations.EaseOutBack;
                    case Equation.EASE_IN_OUT_BACK:
                        return EasingEquations.EaseInOutBack;
                    case Equation.EASE_IN_ELASTIC:
                        return EasingEquations.EaseInElastic;
                    case Equation.EASE_OUT_ELASTIC:
                        return EasingEquations.EaseOutElastic;
                    case Equation.EASE_IN_OUT_ELASTIC:
                        return EasingEquations.EaseInOutElastic;
                    case Equation.LINEAR:
                    default:
                        return EasingEquations.Linear;
                }
            } set {
                m_equation = value;
            }
        }
        
    }
}