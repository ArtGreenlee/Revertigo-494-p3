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

    private Vector3 spawnScale;

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
        spawnScale = transform.localScale;
        textMesh.color = color;
        StartCoroutine(growShrinkFade());
    }

    private void OnEnable()
    {
        transform.localScale = spawnScale;
        StartCoroutine(growShrinkFade());
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cameraTransform.position);
    }

    private IEnumerator growShrinkFade()
    {
        textMesh.color = color;
        StartCoroutine(destroyaftertime(2));
        float startScale = rectTransform.localScale.x;
        while (rectTransform.localScale.x < startScale + .4f)
        {
            float increase = 3f * Time.deltaTime;
            Vector3 newScale = new Vector3(rectTransform.localScale.x + increase, rectTransform.localScale.y + increase, rectTransform.localScale.z + increase);
            rectTransform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        startScale = rectTransform.localScale.x;
        while (rectTransform.localScale.x > startScale - .2f)
        {
            float increase = -2f * Time.deltaTime;
            Vector3 newScale = new Vector3(rectTransform.localScale.x + increase, rectTransform.localScale.y + increase, rectTransform.localScale.z + increase);
            rectTransform.localScale = newScale;
            yield return new WaitForEndOfFrame();
        }
        while (textMesh.color.a > .1f)
        {
            transform.Translate(transform.up * Time.deltaTime * 1);
            Color curColor = textMesh.color;
            curColor.a -= 1 * Time.deltaTime;
            textMesh.color = curColor;
            yield return new WaitForEndOfFrame();
        }
        gameObject.SetActive(false);
    }

    private IEnumerator destroyaftertime(float timer)
    {
        yield return new WaitForSeconds(timer);
        transform.localScale = spawnScale;
        gameObject.SetActive(false);
        StopAllCoroutines();
    }
}
