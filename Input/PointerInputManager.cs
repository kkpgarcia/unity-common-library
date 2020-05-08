using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Common.Input {
    public class PointerInputManager : MonoBehaviour {
        public event Action<PointerInput, double> Pressed;
        public event Action<PointerInput, double> Dragged;
        public event Action<PointerInput, double> Released;

        private bool m_Dragging;
        private PointerControls m_Controls;

        [SerializeField]
        private bool m_UseMouse;
        [SerializeField]
        private bool m_UsePen;
        [SerializeField]
        private bool m_UseTouch;

        protected virtual void Awake() {
            m_Controls = new PointerControls();

            //This needs to regester on the pointer control itself.
            //Temporary solution for registering binding composites.
            InputSystem.RegisterBindingComposite<PointerInputComposite>();
            
            m_Controls.pointer.point.performed += OnAction;
            m_Controls.pointer.point.canceled += OnAction;

            SyncBindingMask();
        }

        protected virtual void OnEnable()
        {
            m_Controls?.Enable();
        }

        protected virtual void OnDisable()
        {
            m_Controls?.Disable();
        }

        protected void OnAction(InputAction.CallbackContext context) {
            InputControl control = context.control;
            InputDevice device = control.device;

            bool isMouseInput = device is Mouse;
            bool isPenInput = !isMouseInput && device is Pen;

            PointerInput drag = context.ReadValue<PointerInput>();

            if(isMouseInput)
                drag.InputId = InputHelper.LeftMouseInputId;
            else if (isPenInput)
                drag.InputId = InputHelper.PenInputId;

            if(drag.Contact && !m_Dragging) {
                Pressed?.Invoke(drag, context.time);
                m_Dragging = true;
            } else if (drag.Contact && m_Dragging) {
                Dragged?.Invoke(drag, context.time);
            } else {
                Released?.Invoke(drag, context.time);
                m_Dragging = false;
            }
        }

        private void SyncBindingMask() {
            if(m_Controls == null)
                return;

            if(m_UseMouse && m_UsePen && m_UseTouch) {
                m_Controls.bindingMask = null;
                return;
            }

            m_Controls.bindingMask = InputBinding.MaskByGroups(new[] {
                m_UseMouse ? "Mouse" : null,
                m_UsePen ? "Pen" : null,
                m_UseTouch ? "Touch" : null
            });
        }

        private void OnValidate() {
            SyncBindingMask();
        }
    }
}