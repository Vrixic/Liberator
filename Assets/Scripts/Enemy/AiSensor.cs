using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AiSensor : MonoBehaviour
{
    public float distance = 23;
    public float raycastDistance = 23f;
    public float angle = 30;
    public float height = 1.0f;
    public float heightOffsetFromOrigin = 1.0f;
    public Color meshColor = Color.red;
    public int scanFrequency = 30;
    public LayerMask playerLayer;
    public LayerMask occlusionLayers;
    [SerializeField] Transform raycastOrigin;

    public List<GameObject> Objects
    {
        get
        {
            //removes all null objects in list.
            objects.RemoveAll(obj => !obj);
            return objects;
        }
    }
    private List<GameObject> objects = new List<GameObject>();

    Collider[] colliders = new Collider[2];
    Mesh mesh;

    int count;
    float scanInterval;
    float scanTimer;
    public bool playerInRange = false;
    private bool alreadyScanning = false;

    public bool playerFoundByRaycast = false;
    private bool alreadyIdleRaycasting = false;
    private bool alreadyRaycasting = false;

    // Start is called before the first frame update
    void Start()
    {
        scanInterval = 1.0f / scanFrequency;
        playerLayer = LayerMask.NameToLayer("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //scanTimer -= Time.deltaTime;
        //if(scanTimer < 0)
        //{
        //    scanTimer += scanInterval;
        //    Scan();
        //}
    }

    public bool Scan()
    {
        //scans within the enemy's range for a gameObject on the player layer(the player)
        count = Physics.OverlapSphereNonAlloc(transform.position, raycastDistance, colliders, playerLayer, QueryTriggerInteraction.Collide);

        //returns true if it found the player to be in range, false if not
        return count > 0;
    }

    public bool IsInsight()
    {
        //get the agents position in the world
        Vector3 origin = raycastOrigin.position;

        //look from the enemies midsection
        //origin.y *= 0.5f;

        //get player's position in the world from the gamemanager
        Vector3 dest = GameManager.Instance.playerTransform.position;

        //look for the player's head
        dest.y -= 0.2f;

        Vector3 direction = (dest - origin).normalized;
        //Debug.Log(direction);
        //checks if an object is within the height of the sensor        

        ////checks if an object is within the horizontal of the sensor
        //direction.y = 0;
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        //if the player is outside of the enemy FOV
        if (deltaAngle > angle)
        {
            return false;
        }

        //Debug.Log("Angle passed");

        //send the raycast from just in front of the enemy so they don't hit their own hitbox
        //origin += Vector3.ClampMagnitude(transform.forward, 0.5f);
        //dest.y = origin.y;

        //fire a raycast from the enemy to the player to see if anything obstructs the enemies view
        if (Physics.Raycast(origin, direction, out RaycastHit hit, raycastDistance))
        {
            //Debug.Log("Entered raycast line, " + hit.collider.tag);
            //Debug.DrawLine(origin, dest, Color.green, 1f);

            //if the player is hit by the raycast, the player is in sight
            if (hit.collider.gameObject.layer == playerLayer)
            {
                return true;
            }
        }

        //the raycast hit nothing or didn't hit the player
        return false;
    }
   
    public bool IsInsightWithAngleDistance()
    {
        //get the agents position in the world
        Vector3 origin = raycastOrigin.position;

        //get player's position in the world from the gamemanager
        Vector3 dest = GameManager.Instance.playerTransform.position;

        //look for the player's head
        dest.y -= 0.2f;

        Vector3 direction = (dest - origin).normalized;
        //checks if an object is within the height of the sensor

        ////checks if an object is within the horizontal of the sensor
        float deltaAngle = Vector3.Angle(direction, transform.forward);

        //if the player is outside of the enemy FOV
        if (deltaAngle > angle)
        {
            return false;
        }

        //send the raycast from just in front of the enemy so they don't hit their own hitbox
        //fire a raycast from the enemy to the player to see if anything obstructs the enemies view
        if (Physics.Raycast(origin, direction, out RaycastHit hit, distance))
        {
            //if the player is in range and inside the FOV, with no objects obstructing the enemies view
            if (hit.collider.gameObject.layer == playerLayer)
            {
                return true;
            }
        }

        return false;
    }

    public bool IsInsightAttackAndChase()
    {
        //get the agents position in the world
        Vector3 origin = raycastOrigin.position;

        //get player's position in the world from the gamemanager
        Vector3 dest = GameManager.Instance.playerTransform.position;

        //look for the player's midsection
        dest.y -= 0.2f;

        Vector3 direction = (dest - origin).normalized;

        //fire a raycast from the enemy to the player to see if anything obstructs the enemies view
        if (Physics.Raycast(origin, direction, out RaycastHit hit, raycastDistance))
        {
            //Debug.Log("Entered raycast line, " + hit.collider.tag);
            //Debug.DrawLine(origin, dest, Color.green, 1f);

            if (hit.collider.gameObject.layer == playerLayer)
            {
                return true;
            }
        }

        return false;
    }

    public void StartScan()
    {
        if(!alreadyScanning)
            StartCoroutine(IncrementalScan());
    }

    public void StopScan()
    {
        StopCoroutine(IncrementalScan());

        alreadyScanning = false;
    }

    private IEnumerator IncrementalScan()
    {
        alreadyScanning = true;

        while (true)
        {
            playerInRange = Scan();

            if (playerInRange) { StartIdleRaycast(); }
            else if (alreadyIdleRaycasting) { StopIdleRaycast(); }

            yield return new WaitForSeconds(0.3f);
        }
    }

    public void StartIdleRaycast()
    {
        if (!alreadyIdleRaycasting)
            StartCoroutine(IncrementalIdleRaycast());
    }

    public void StopIdleRaycast()
    {
        StopCoroutine(IncrementalIdleRaycast());

        alreadyIdleRaycasting = false;
    }

    private IEnumerator IncrementalIdleRaycast()
    {
        alreadyIdleRaycasting = true;

        while (true)
        {
            playerFoundByRaycast = IsInsightWithAngleDistance();
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void StartChaseAndAttackRaycast()
    {
        if (!alreadyRaycasting)
            StartCoroutine(IncrementalChaseAndAttackRaycast());
    }

    public void StopChaseAndAttackRaycast()
    {
        StopCoroutine(IncrementalChaseAndAttackRaycast());

        alreadyRaycasting = false;
    }

    public IEnumerator IncrementalChaseAndAttackRaycast()
    {
        alreadyRaycasting = true;

        while (true)
        {
            playerFoundByRaycast = IsInsightAttackAndChase();
            yield return new WaitForSeconds(0.1f);
        }
    }

    Mesh CreateWedgeMesh()
    {
        Mesh mesh = new Mesh();

        int segments = 10;
        //amount of triangles that make up the sensor wedge
        int numTriangles = (segments * 4) + 2 + 2;
        //amount of vertices, each triangle has 3
        int numVertices = numTriangles * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triangles = new int[numVertices];

        //defines the starting point of the wedge at characters origin.
        //Vector3 bottomCenter = new Vector3(0, -heightOffsetFromOrigin, 0);
        Vector3 bottomCenter = Vector3.zero;
        //defines bottom left with a vector at the negative angle and multiplying it by the agents forward vector and the distance scalar
        //same but with positive to the right
        Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
        Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

        //takes other three vectors and places them in the same spot, moving them up by the height scalar.
        Vector3 topCenter = bottomCenter + Vector3.up * height;
        Vector3 topLeft = bottomLeft + Vector3.up * height;
        Vector3 topRight = bottomRight + Vector3.up * height;


        int vert = 0;

        //left side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = bottomLeft;
        vertices[vert++] = topLeft;
        //loops on the left side to set the triangle from bottom center back to the bottom center
        vertices[vert++] = topLeft;
        vertices[vert++] = topCenter;
        vertices[vert++] = bottomCenter;

        //right side
        vertices[vert++] = bottomCenter;
        vertices[vert++] = topCenter;
        vertices[vert++] = topRight;
        //loops on the Right side to set the triangle from bottom center back to the bottom center
        vertices[vert++] = topRight;
        vertices[vert++] = bottomRight;
        vertices[vert++] = bottomCenter;

        //subdivides current wedges into other wedges so its a cone rather than a triangular prism
        float currentAngle = -angle;
        float deltaAngle = (angle * 2) / segments;
        for (int i = 0; i < segments; i++)
        {
            //redefines bottom left with a vector at the negative angle and multiplying it by the agents forward vector and the distance scalar
            //same but with positive to the right
            bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
            bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

            //redefines left and right vectors
            topLeft = bottomLeft + Vector3.up * height;
            topRight = bottomRight + Vector3.up * height;

            //far side
            vertices[vert++] = bottomLeft;
            vertices[vert++] = bottomRight;
            vertices[vert++] = topRight;
            //loops on the far side, going from bottom left vertex, to bottom right, to top right, top left, then back to bottom left.
            vertices[vert++] = topRight;
            vertices[vert++] = topLeft;
            vertices[vert++] = bottomLeft;


            //top of wedge
            //only loops through once, as only one triangle is needed to create top side.
            vertices[vert++] = topCenter;
            vertices[vert++] = topLeft;
            vertices[vert++] = topRight;

            //bottom of wedge
            //only loops through once, as only one triangle is needed to create bottom side.
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomLeft;
            currentAngle += deltaAngle;
        }



        for (int i = 0; i < numVertices; i++)
        {
            triangles[i] = i;
        }
        //sets all the vertices and triangles of the mesh in order that is listed above, then recalculates the norms to the updated triangles.
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        return mesh;
    }

    private void OnValidate()
    {
        //mesh = CreateWedgeMesh();
    }

    private void OnDrawGizmos()
    {
        //if (mesh)
        //{
        //    Gizmos.color = meshColor;
        //    //draws the sight mesh on the agent transform and position.
        //    Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
        //}
    }
}
