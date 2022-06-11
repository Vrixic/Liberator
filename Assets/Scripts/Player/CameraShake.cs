using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    //This is the value that all impulse sources will add to. The higher this value gets the more camera shake is applied
    [Range(0, 1)] [SerializeField] private float trauma = 0f;
    [SerializeField] private float frequency = 18f;
    [SerializeField] private float amplitude = 0.6f;
    [SerializeField] private float minimumAmplitude = 0.2f;
    [SerializeField] private float amplitudeMultiplier = 3f;

    public float Trauma
    {
        get
        {
            return trauma;
        }
        set
        {
            trauma = Mathf.Clamp01(value);
        }
    }

    private float sampleLocation = 0f;

    float GetFloat(float noiseSeed)
    {
        //returns a small positive or negative value that will smoothly shift back and forth over time
        //subtracting 0.5f makes it so that the value will be centered around 0 with ability to return a value from -1 to 1
        return (Mathf.PerlinNoise(noiseSeed, sampleLocation) - 0.5f) * 2;
    }

    Vector3 GetVector3()
    {
        //generates a new noise value for x, y, and potentially z rotation
        return new Vector3(GetFloat(1), GetFloat(10), 0);
    }

    void Update()
    {
        amplitude = trauma * amplitudeMultiplier + 0.2f;

        //move through the noisemap with a nice decay curve by square rooting the trauma
        sampleLocation += Time.deltaTime * Mathf.Sqrt(trauma) * frequency;

        trauma = Mathf.Clamp01(trauma - Time.deltaTime);

        Vector3 newRotation = GetVector3() * amplitude;
        transform.localRotation = Quaternion.Euler(newRotation);
    }
}
