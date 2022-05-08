using UnityEngine.EventSystems;

namespace Haptiq.Cloudberrie.Builder_2.InputManagement
{
	public class InputModule : StandaloneInputModule
	{
		public PointerEventData GetPointerData(int pointerId = kMouseLeftId)
		{
			PointerEventData pointerData;

			m_PointerData.TryGetValue(pointerId, out pointerData);

			if (pointerData == null) pointerData = new PointerEventData(EventSystem.current);

			return pointerData;
		}

		public bool IsPointerOverGameObject<T>(int pointerId = kMouseLeftId, bool withDerived = true) where T : BaseRaycaster
		{
			if (IsPointerOverGameObject(pointerId))
			{
				PointerEventData pointerEventData;
				if (m_PointerData.TryGetValue(pointerId, out pointerEventData))
				{
					if (withDerived)
					{
						return pointerEventData.pointerCurrentRaycast.module is T;
					}

					return pointerEventData.pointerCurrentRaycast.module.GetType() == typeof(T);
				}
			}

			return false;
		}
	}
}
