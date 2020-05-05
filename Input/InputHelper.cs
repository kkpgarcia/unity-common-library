using UnityEngine.EventSystems;

namespace Common.Input
{
    public static class InputHelper
    {
        public const int LeftMouseInputId = PointerInputModule.kMouseLeftId;
        public const int RightMouseInputId = PointerInputModule.kMouseRightId;
        public const int MiddleMouseInputId = PointerInputModule.kMouseMiddleId;
        public const int PenInputId = int.MinValue;
    }
}
