using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UIEventTrigger : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler, ISelectHandler, ISubmitHandler {
    [SerializeField] private AudioData selectSFX;

    [SerializeField] private AudioData submitSFX;

    public void OnPointerEnter(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(selectSFX);
    }

    public void OnPointerDown(PointerEventData eventData) {
        AudioManager.Instance.PlaySFX(submitSFX);
    }

    public void OnSelect(BaseEventData eventData) {
        AudioManager.Instance.PlaySFX(selectSFX);
    }

    public void OnSubmit(BaseEventData eventData) {
        AudioManager.Instance.PlaySFX(submitSFX);
    }
}