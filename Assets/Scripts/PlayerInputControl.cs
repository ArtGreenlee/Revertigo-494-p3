using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputControl : MonoBehaviour
{

    public float moveSpeed;
    public float rotateSpeed;
    public Transform cameraTransform;
    public Vector3 currentLookPoint;
    public float movementBuffer;
    private TowerDisplay towerDisplay;
    private int layerMask;

    public static PlayerInputControl instance;
    public bool movementEnabled;
    // public bool enabled;

    Vector3 endPos;
    Quaternion endRot;
    Vector3 startPos;
    Quaternion startRot;
    int currentSide = 0;
    Vector3[] sidePositions = new[] { new Vector3(0f, 0f, -8), new Vector3(16f, 0f, 0f), new Vector3(0f, 0f, 8), new Vector3(-8, 0f, 0f), new Vector3(0f, 8, 0f), new Vector3(0f, -8, 0f) };
    Quaternion[] sideRotations = new[] { Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, -90, 0), Quaternion.Euler(0, 180, 0), Quaternion.Euler(0, 90, 0), Quaternion.Euler(90, 0, 0), Quaternion.Euler(-90, 0, 0) };
    int[,] nextSide = new int[6, 4] { { 1, 3, 5, 4 }, { 2, 0, 5, 4 }, { 3, 1, 5, 4 }, { 0, 2, 5, 4 }, { 1, 3, 0, 2 }, { 1, 3, 2, 0 } };

    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        movementEnabled = false;
        cameraTransform = Camera.main.transform;
        rb = GetComponent<Rigidbody>();
        layerMask = ~LayerMask.GetMask("Tower", "Player");
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    private IEnumerator snapToPosition(Vector3 position)
    {
        Debug.Log(position);
        while (transform.position != position)
        {
            transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * 1.5f);
            yield return new WaitForFixedUpdate();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            currentSide = nextSide[currentSide, 0];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StopAllCoroutines();
            StartCoroutine(snapToPosition(endPos));
        }
        else if (Input.GetKeyDown(KeyCode.D))
        {
            currentSide = nextSide[currentSide, 1];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StopAllCoroutines();
            StartCoroutine(snapToPosition(endPos));
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            currentSide = nextSide[currentSide, 2];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StopAllCoroutines();
            StartCoroutine(snapToPosition(endPos));
        }
        else if (Input.GetKeyDown(KeyCode.W))
        {
            currentSide = nextSide[currentSide, 3];
            endPos = sidePositions[currentSide];
            endRot = sideRotations[currentSide];
            StopAllCoroutines();
            StartCoroutine(snapToPosition(endPos));
        }
        // if (Input.GetKeyDown(KeyCode.Space))
        // {
        //     //movementEnabled = !movementEnabled;
        // }

        // if (Input.GetKeyDown(KeyCode.W))
        // {
        //     //Snap to the wall above you
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(0, 5, 0)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
        // else if (Input.GetKeyDown(KeyCode.A))
        // {
        //     //Snap to the wall above you
        //     Vector3 curSide = UtilityFunctions.getClosestSide(transform.position);
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(-5, 0 , 0)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
        // else if (Input.GetKeyDown(KeyCode.D))
        // {
        //     //Snap to the wall above you
        //     Vector3 curSide = UtilityFunctions.getClosestSide(transform.position);
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(5, 0, 0)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
        // else if (Input.GetKeyDown(KeyCode.S))
        // {
        //     //Snap to the wall above you
        //     Vector3 curSide = UtilityFunctions.getClosestSide(transform.position);
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(-5, 0, 0)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
        // else if (Input.GetKeyDown(KeyCode.X))
        // {
        //     //Snap to the wall above you
        //     Vector3 curSide = UtilityFunctions.getClosestSide(transform.position);
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(0, 0, -5)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
        // else if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     //Snap to the wall above you
        //     Vector3 curSide = UtilityFunctions.getClosestSide(transform.position);
        //     StopAllCoroutines();
        //     StartCoroutine(snapToPosition(new Vector3(0, 0, 5)));
        //     //if curside == forward, up is curside + (-1, 1, 0)
        // }
    }

    

    // Update is called once per frame
    void FixedUpdate()
    {
        RaycastHit hit;
        Vector2 mousePosition = Input.mousePosition;
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out hit, 1000, layerMask))
        {
            currentLookPoint = hit.point;
            rb.MoveRotation(Quaternion.Slerp(transform.rotation,
                               Quaternion.LookRotation(currentLookPoint - transform.position),
                               rotateSpeed * Time.deltaTime));
            
        }
        //if the player is far enough from the camera, push them back

        /*if ((transform.position - cameraTransform.position).sqrMagnitude > 100)
        {
            rb.AddForce(Vector3.zero - transform.position);
        }*/
        if (transform.position.sqrMagnitude < 10)
        {
            rb.AddForce(transform.position);
        }
        else if (transform.position.sqrMagnitude > 80)
        {
            rb.AddForce(transform.position * -2);
        }

        /*if (Input.GetKey(KeyCode.W))
        {
            rb.AddRelativeForce(Vector3.forward * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            rb.AddRelativeForce(Vector3.forward * moveSpeed * -1);
        }*/

        

        if (movementEnabled && !Input.GetMouseButton(1))
        {
            if (currentLookPoint.x > transform.position.x + movementBuffer)
            {
                rb.AddForce(Vector3.right * moveSpeed);
            }
            else if (currentLookPoint.x < transform.position.x - movementBuffer)
            {
                rb.AddForce(Vector3.left * moveSpeed);
            }
            if (currentLookPoint.y > transform.position.y + movementBuffer)
            {
                rb.AddForce(Vector3.up * moveSpeed);
            }
            else if (currentLookPoint.y < transform.position.y - movementBuffer)
            {
                rb.AddForce(Vector3.down * moveSpeed);
            }
            if (currentLookPoint.z > transform.position.z + movementBuffer)
            {
                rb.AddForce(Vector3.forward * moveSpeed);
            }
            else if (currentLookPoint.z < transform.position.z - movementBuffer)
            {
                rb.AddForce(Vector3.back * moveSpeed);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
        }

        



        /*if (Input.GetKey(KeyCode.A))
        {
            rb.AddRelativeForce(Vector3.left * moveSpeed );
        }
        else if (Input.GetKey(KeyCode.D))
        {
            rb.AddRelativeForce(Vector3.left * moveSpeed * -1);
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddRelativeForce(Vector3.up * moveSpeed);
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.AddRelativeForce(Vector3.down * moveSpeed);
        }*/

        

            /*if (Input.GetKey(KeyCode.UpArrow))
            {
                rb.AddRelativeTorque(Vector3.left * rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.DownArrow))
            {
                rb.AddRelativeTorque(Vector3.right * rotateSpeed);
            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                rb.AddRelativeTorque(Vector3.down * rotateSpeed);
            }
            else if (Input.GetKey(KeyCode.RightArrow))
            {
                rb.AddRelativeTorque(Vector3.up * rotateSpeed);
            }*/
        }
}
