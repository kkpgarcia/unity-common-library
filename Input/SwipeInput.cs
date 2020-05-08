using UnityEngine;

namespace Common.Input {
    public struct SwipeInput {
        public readonly int InputId;
        public readonly Vector2 CurrentPosition;
        public readonly Vector2 StartPosition;
        public readonly Vector2 PreviousPosition;
        public readonly Vector2 EndPosition;
        public readonly Vector2 SwipeDirection;
        public readonly float SwipeVelocity;
        public readonly float TravelDistance;
        public readonly double SwipeDuration;
        public readonly float SwipeSameness;
        
        internal SwipeInput(ActiveGesture gesture) : this() {
            InputId = gesture.InputId;
            StartPosition = gesture.StartPosition;
            PreviousPosition = gesture.PreviousPosition;
            EndPosition = gesture.EndPosition;
            SwipeDirection = (EndPosition - StartPosition).normalized;
            SwipeDuration = gesture.EndTime - gesture.StartTime;
            TravelDistance = gesture.TravelDistance;
            SwipeSameness = gesture.SwipeDirectionSameness;
            CurrentPosition = gesture.CurrentPosition;

            if (SwipeDuration > 0.0f)
                SwipeVelocity = (float)(TravelDistance / SwipeDuration);
            
        }
    }
}