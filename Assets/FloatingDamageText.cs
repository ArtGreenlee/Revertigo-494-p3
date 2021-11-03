using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Transform cameraTransform;

    public void setDamage(float damageIn)
    {
        textMesh.SetText(damageIn.ToString());
    }

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        textMesh = GetComponent<TextMeshPro>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(cameraTransform);
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        /*var forward = camera.transform.forward;
        var right = camera.transform.right;
 
        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
 
        //this is the direction in the world space we want to move:
        var desiredMoveDirection = forward * verticalAxis + right * horizontalAxis;
 
        //now we can apply the movement:
        transform.Translate(desiredMoveDirection * speedMeUp * Time.deltaTime);
        */
        transform.Translate(transform.up * Time.deltaTime);
    }

    private IEnumerator growShrinkFade()
    {
        Vector3 startScale = transform.localScale;
        float startMagnitute = startScale.magnitude;
        while (startMagnitute < startMagnitute + .2f)
        {
            float increase = .1f * Time.deltaTime;
            startScale = new Vector3(startScale.x + increase, startScale.y + increase, startScale.z + increase);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(.1f);
        startMagnitute = startScale.magnitude;
        while (startMagnitute > startMagnitute - .5f)
        {
            float decrease = -.1f * Time.deltaTime;
            startScale = new Vector3(startScale.x + decrease, startScale.y + decrease, startScale.z + decrease);
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
