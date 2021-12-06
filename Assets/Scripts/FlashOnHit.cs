using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashOnHit : MonoBehaviour
{

    public Material flashMaterial;
    public Material originalMaterial;
    private MeshRenderer meshRenderer;
    public float flashDuration;
    private bool isFlashing = false;

    private Coroutine flashRoutineInstance = null;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    public void flash()
    {
        if (flashRoutineInstance != null)
        {
            StopCoroutine(flashRoutineInstance);
            
        }
        flashRoutineInstance = StartCoroutine(flashRoutine());
    }

    private IEnumerator flashRoutine()
    {
        meshRenderer.material = flashMaterial;
        yield return new WaitForSeconds(flashDuration);
        meshRenderer.material = originalMaterial;
        flashRoutineInstance = null;

    }
}
