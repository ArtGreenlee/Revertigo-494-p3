using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLowHealth : MonoBehaviour
{
    [SerializeField] GameObject healthBar;
    public float flashValue = 0.25f;
    public float flashThresh = 0.25f;


    Graphic img;
    Color flash1;
    Color flash2;

    Slider slider;
    float lastFlash;

    // Start is called before the first frame update
    void Start()
    {
        img = healthBar.GetComponent<Graphic>();
        flash1 = img.color;
        flash2 = Color.blue;
        lastFlash = 0f;

        slider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(slider.value < flashValue){
            if(Time.time - lastFlash >= flashThresh){
                if(img.color == flash1){
                    img.color = flash2;
                }
                else{
                    img.color = flash1;
                }

                lastFlash = Time.time;
            }
        }
    }
}
