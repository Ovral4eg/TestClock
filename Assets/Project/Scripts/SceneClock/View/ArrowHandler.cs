using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class ArrowHandler : MonoBehaviour, IDragHandler
{
    public event EventHandler<ArrowChangeArgs> OnChange;
    [SerializeField] private GameObject arrow;
    public void Enable()
    {
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = new Vector3(eventData.position.x, eventData.position.y, 0);
        Vector3 direction = mousePosition - arrow.transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Vector3 vector = new Vector3(0, 0, angle);
       

        OnChange?.Invoke(this, new ArrowChangeArgs { angle = vector.z});
    }
}
