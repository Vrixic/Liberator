using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    private const float maxIndicatorDecayTime = 3.0f;
    private float currentIndicatorDecayTimer = maxIndicatorDecayTime;

    private CanvasGroup indicator = null;
    public CanvasGroup Indicator { get { indicator = GetComponent<CanvasGroup>(); return indicator; } }

    //component used to rotate a given indicator locally around the screen to face the damage source
    private RectTransform indicatorTransform = null;
    public RectTransform IndicatorTransform { get { indicatorTransform = GetComponent<RectTransform>(); return indicatorTransform; } }

    //transform of the gameObject that damaged the player(enemy, explodable, etc.)
    public Transform damageSource = null;
    private Transform player = null;

    //enumerator used to countdown a given damage indicator's lifetime
    private IEnumerator IE_Countdown = null;

    //unregister action that will be called when we need to get rid of a given indicator
    private Action unregister = null;

    private Quaternion damageSourceRotation = Quaternion.identity;
    private Vector3 damageSourcePosition = Vector3.zero;

    public void Register(Transform _damageSource, Transform _player, Action _unregister)
    {
        damageSource = _damageSource;
        player = _player;
        unregister = _unregister;

        StartTimer();
        StartCoroutine(RotateIndicatorToTarget());

    }

    //if the same damage source hits the player(again) while they already have an active damage indicator,
    //restart the decay time for that indicator
    public void Restart()
    {
        currentIndicatorDecayTimer = maxIndicatorDecayTime;
        StartTimer();
    }

    private void StartTimer()
    {
        if (IE_Countdown != null) { StopCoroutine(IE_Countdown); }
        IE_Countdown = Countdown();
        StartCoroutine(IE_Countdown);
    }

    private IEnumerator Countdown()
    {
        //Make the indicator visible by raising the opacity
        while (Indicator.alpha < 1.0f)
        {
            Indicator.alpha += 8 * Time.unscaledDeltaTime;
            yield return null;
        }
        //wait until it is time to decay the indicator
        while(currentIndicatorDecayTimer > 0)
        {
            currentIndicatorDecayTimer--;
            yield return new WaitForSecondsRealtime(1f);
        }
        //make the indicator invisible(decay) from the screen
        while(Indicator.alpha > 0.0f)
        {
            Indicator.alpha -= 2 * Time.unscaledDeltaTime;
            yield return null;
        }

        //call the unregister action to clean up this specific indicator
        unregister();

        //destroy this specific indicator
        Destroy(gameObject);
    }

    public void DestroyIndicator()
    {
        if (IE_Countdown != null) { StopCoroutine(IE_Countdown); }

        Destroy(gameObject);
    }

    IEnumerator RotateIndicatorToTarget()
    {
        while(enabled)
        {
            if(damageSource != null)
            {
                damageSourcePosition = damageSource.position;
                damageSourceRotation = damageSource.rotation;
            }

            //find the direction in the world from the player to the damage source
            Vector3 direction = player.position - damageSourcePosition;

            //get a target rotation from  that direction
            damageSourceRotation = Quaternion.LookRotation(direction);

            //convert it to the rotation we need for the 2D indicator(only z axis rotation needed)
            damageSourceRotation.z = -damageSourceRotation.y;
            damageSourceRotation.x = 0;
            damageSourceRotation.y = 0;

            Vector3 up = new Vector3(0, 0, player.eulerAngles.y);

            //rotate this indicator to indicate the location of the damage source on the screen
            IndicatorTransform.localRotation = damageSourceRotation * Quaternion.Euler(up);

            yield return null;
        }
    }
}
