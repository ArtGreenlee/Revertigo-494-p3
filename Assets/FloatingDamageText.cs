using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    private TextMeshPro textMesh;
    private Transform cameraTransform;
    private RectTransform rectTransform;
    public Color color;

    public void setDamage(float damageIn)
    {
        textMesh.SetText(Mathf.Round(damageIn).ToString());
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        cameraTransform = Camera.main.transform;
        textMesh = GetComponent<TextMeshPro>();
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh.color = color;
        StartCoroutine(growShrinkFade());
        Vector3 randomOffset = Random.insideUnitCircle;
        transform.Translate((cameraTransform.position - transform.position).normalized + randomOffset);
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
        transform.Translate(transform.up * Time.deltaTime * 1.2f);
    }

    private IEnumerator growShrinkFade()
    {
        StartCoroutine(destroyaftertime(2));
        float startMagnitute = rectTransform.localScale.magnitude;
        while (rectTransform.localScale.magnitude < startMagnitute + .2f)
        {
            float increase = .5f * Time.deltaTime;
            Vector3 newScale = new Vector3(rectTransform.localScale.x + increase, rectTransform.localScale.y + increase, rectTransform.localScale.z + increase);
            rectTransform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(.1f);
        startMagnitute = rectTransform.localScale.magnitude;
        while (rectTransform.localScale.magnitude > startMagnitute - .3f)
        {
            Color curColor = textMesh.color;
            curColor.a -= 2 * Time.deltaTime;
            textMesh.color = curColor;
            float decrease = -.1f * Time.deltaTime;
            Vector3 newScale = new Vector3(rectTransform.localScale.x + decrease, rectTransform.localScale.y + decrease, rectTransform.localScale.z + decrease);
            rectTransform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    private IEnumerator destroyaftertime(float timer)
    {
        yield return new WaitForSeconds(timer);
        gameObject.SetActive(false);
    }
}
