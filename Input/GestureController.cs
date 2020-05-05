using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

namespace Common.Input {
    public class GestureController : MonoBehaviour {
        [SerializeField]
        private PointerInputManager InputManager;
        [SerializeField]
        private float MaxTapDuration = 0.2f;
        [SerializeField]
        private float MaxTapDrift = 0.5f;
        [SerializeField]
        private float MaxSwipeDuration = 0.5f;
        [SerializeField]
        private float MinSwipeDistance = 10.0f;
        [SerializeField]
        private float SwipeDirectionSamenessThreshold = 0.6f;

        private readonly Dictionary<int, ActiveGesture> ActiveGestures = new Dictionary<int, ActiveGesture>();
    
        public new event Action<SwipeInput> Pressed;
        public new event Action<SwipeInput> PotentiallySwiped;
        public event Action<SwipeInput> Swiped;
        public event Action<TapInput> Tapped;

        protected virtual void Awake() {
            InputManager.Pressed += OnPressed;
            InputManager.Dragged += OnDragged;
            InputManager.Released += OnReleased;
        }

        private bool IsValidSwipe(ref ActiveGesture gesture) {
            return gesture.TravelDistance >= MinSwipeDistance &&
                    (gesture.StartTime - gesture.EndTime) <= MaxSwipeDuration &&
                    gesture.SwipeDirectionSameness >= SwipeDirectionSamenessThreshold;
        }

         private bool IsValidTap(ref ActiveGesture gesture)
        {
            return gesture.TravelDistance <= MaxTapDrift &&
                (gesture.StartTime - gesture.EndTime) <= MaxTapDuration;
        }

        private void OnPressed(PointerInput input, double time) {
            Debug.Assert(!ActiveGestures.ContainsKey(input.InputId));

            var newGesture = new ActiveGesture(input.InputId, input.Position, time);
            ActiveGestures.Add(input.InputId, newGesture);

            Pressed?.Invoke(new SwipeInput(newGesture));
        }

        private void OnDragged(PointerInput input, double time) {
            if(!ActiveGestures.TryGetValue(input.InputId, out ActiveGesture existingGesture))
                return;
            
            existingGesture.SubmitPoint(input.Position, time);

            if(IsValidSwipe(ref existingGesture))
                PotentiallySwiped?.Invoke(new SwipeInput(existingGesture));
        }

        private void OnReleased(PointerInput input, double time) {
            if(!ActiveGestures.TryGetValue(input.InputId, out ActiveGesture existingGesture))
                return;
            
            ActiveGestures.Remove(input.InputId);
            existingGesture.SubmitPoint(input.Position, time);

            if(IsValidSwipe(ref existingGesture))
                Swiped?.Invoke(new SwipeInput(existingGesture));

            if(IsValidTap(ref existingGesture))
                Tapped?.Invoke(new TapInput(existingGesture));
        }
    }
}