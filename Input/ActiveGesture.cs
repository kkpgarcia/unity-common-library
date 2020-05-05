using UnityEngine;
using Common.Utils;

namespace Common.Input {
    internal sealed class ActiveGesture {
        public int InputId;
        public readonly double StartTime;
        public double EndTime;
        public readonly Vector2 StartPosition;
        public Vector2 PreviousPosition;
        public Vector2 EndPosition;
        public int Samples;
        public float SwipeDirectionSameness;
        public float TravelDistance;
        private Vector2 m_AccumulatedNormalized;
        
        public ActiveGesture(int inputId, Vector2 startPosition, double startTime) {
            InputId = inputId;
            EndTime = StartTime = startTime;
            EndPosition = StartPosition = startPosition;
            Samples = 1;
            SwipeDirectionSameness = 1;
            m_AccumulatedNormalized = Vector2.zero;
        }

        public void SubmitPoint(Vector2 position, double time) {
            Vector2 toNewPosition = position - EndPosition;
            float distanceMoved = toNewPosition.magnitude;

            EndTime = time;

            if(Comparison.TolerantEquals(distanceMoved, 0))
                return;

            toNewPosition /= distanceMoved;
            Samples++;

            Vector2 toNewEndPosition = (position - StartPosition).normalized;

            PreviousPosition = EndPosition;
            EndPosition = position;

            m_AccumulatedNormalized += toNewPosition;

            SwipeDirectionSameness = Vector2.Dot(toNewEndPosition, m_AccumulatedNormalized/(Samples-1));

            TravelDistance += distanceMoved;
        }
    }
}