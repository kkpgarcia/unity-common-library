using UnityEngine;
using System.Collections;
using System;

namespace Common.Tween
{
    public abstract class Tweener : MonoBehaviour {

        public static float DefaultDuration = 1f;
        public static Func<float, float, float, float> DefaultEquation = EasingEquations.EaseInOutQuad;

        public EasingControl easingControl;
        public bool destroyOnComplete = true;

        protected virtual void Awake ()
        {
            easingControl = gameObject.AddComponent<EasingControl>();
        }

        protected virtual void OnEnable ()
        {
            easingControl.updateEvent += OnUpdate;
            easingControl.completedEvent += OnComplete;
        }
    
        protected virtual void OnDisable ()
        {
            easingControl.updateEvent -= OnUpdate;
            easingControl.completedEvent -=  OnComplete;
        }

        protected virtual void OnDestroy ()
        {
            if (easingControl != null) {
                Destroy(easingControl);
            }
        }

        protected abstract void OnUpdate(object sender, EventArgs e);

        protected virtual void OnComplete (object sender, EventArgs e)
        {
            if(destroyOnComplete) {
                Destroy(this);
            }
        }
    }
}