using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldStorage : MonoBehaviour
{
    public static GoldStorage instance;
    public float gold;
    public TextMeshProUGUI goldText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void changeGoldAmount(float amount)
    {
        gold += amount;
        goldText.text = "Gold " + gold.ToString();
    }
}
