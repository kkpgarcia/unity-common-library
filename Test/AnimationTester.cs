using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Common.Animation;

public class AnimationTester : MonoBehaviour
{
    public TransformAnimationProperty MoveAnimationProperty;
    public TransformAnimationProperty RotateAnimationProperty;
    public TransformAnimationProperty ScaleAnimationProperty;
    private void Start() {
        this.transform.MoveTo(MoveAnimationProperty);
        this.transform.RotateToLocal(RotateAnimationProperty);
        this.transform.ScaleTo(ScaleAnimationProperty);
    }
}
