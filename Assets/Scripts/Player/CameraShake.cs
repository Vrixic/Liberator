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

    //this value will move around the noise map following the peaks and valleys so that we can smoothly
    //transition between them
    private float sampleLocation = 0f;

    float GetFloat(float noiseSeed)
    {
        //returns a small positive or negative value that will randomly (but smoothly) shift back and forth over time
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
        //increase the amplitude of the noise(distance travelled) with the amount of overall trauma
        amplitude = trauma * amplitudeMultiplier + minimumAmplitude;

        //move through the noisemap with a nice decay curve by square rooting the trauma
        //picture this line as moving a ball across a bumpy surface, the more trauma, the faster we move the ball,
        //and thus we encounter more bumps over the same amount of time
        sampleLocation += Time.deltaTime * Mathf.Sqrt(trauma) * frequency;

        //reduce trauma over time, capping it at 0
        trauma = Mathf.Clamp01(trauma - Time.deltaTime);

        //get a new vector that is populated with a snapshot of perlin noise and scaled by the current amplitude
        Vector3 newRotation = GetVector3() * amplitude;

        //apply the rotation to a parent gameobject of the main camera to rotate the underlying camera
        transform.localRotation = Quaternion.Euler(newRotation);
    }
}
