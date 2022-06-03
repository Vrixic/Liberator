using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DISystem : MonoBehaviour
{
    //prefab to instantiate when creating a new indicator
    [SerializeField] private DamageIndicator indicatorPrefab = null;

    //parent of the CanvasGroup with all of the indicators
    [SerializeField] private RectTransform indicatorParent = null;

    new Camera camera = null;
    Transform player = null;

    //collection of all active directional damage indicators in the scene
    private Dictionary<Transform, DamageIndicator> indicators = new Dictionary<Transform, DamageIndicator>();

    public static Action<Transform> createIndicator = delegate { };
    public static Func<Transform, bool> CheckIfObjectInSight = null;

    private void OnEnable()
    {
        //subscribe to the action and function
        createIndicator += Create;
        CheckIfObjectInSight += InSight;
    }

    private void OnDisable()
    {
        //unsubscribe to the action and function
        createIndicator -= Create;
        CheckIfObjectInSight -= InSight;
    }

    private void Start()
    {
        player = GameManager.Instance.playerTransform;
        camera = GameManager.Instance.mainCamera.GetComponent<Camera>();
    }

    void Create(Transform damageSource)
    {
        //check if there is already an indicator pointing to the damage source
        if (indicators.ContainsKey(damageSource))
        {
            //if so, reset the countdown on that indicator
            indicators[damageSource].Restart();
        }
        else
        {
            //create a new indicator
            DamageIndicator newIndicator = Instantiate(indicatorPrefab, indicatorParent);

            //register it so that the indicator can rotate to point to the damage source and start the decay countdown
            //the new action tells DamageIndicator to remove this damageSource from the dictionary whenever the decay timer expires
            newIndicator.Register(damageSource, player, new Action(() => { indicators.Remove(damageSource); }));

            //stash the indicator within the dictionary
            indicators.Add(damageSource, newIndicator);
        }
    }

    public void FindAndDestroyIndicator(Transform _damageSource)
    {
        if(indicators.ContainsKey(_damageSource))
        {
            indicators[_damageSource].DestroyIndicator();
            indicators.Remove(_damageSource);
        }
    }

    public void ClearAllIndicators()
    {
        //destroy each of the indicators in the dictionary
        foreach (KeyValuePair<Transform, DamageIndicator> damageSource in indicators)
        {
            damageSource.Value.DestroyIndicator();
        }

        //clear the dictionary
        indicators.Clear();
    }

    bool InSight(Transform damageSource)
    {
        Vector3 screenPoint = camera.WorldToViewportPoint(damageSource.position);
        return screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;
    }
}
