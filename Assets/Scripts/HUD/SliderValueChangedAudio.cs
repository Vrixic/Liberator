using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderValueChangedAudio : MonoBehaviour, IPointerUpHandler
{
    Slider slider;

    private void Start()
    {
        slider = GetComponent<Slider>();
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySoundAtVolume(PlayerPrefManager.Instance.sfxVolume/2, "TestSFX");
    }
}
