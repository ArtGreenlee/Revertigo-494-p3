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
    
    Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        movementEnabled = true;
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movementEnabled = !movementEnabled;
        }
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
