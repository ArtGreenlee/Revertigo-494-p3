using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GoldStorage : MonoBehaviour
{
    public float gold;

    public static GoldStorage instance;
    public TextMeshPro goldText; 

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        gold = 0;
    }

    public void changeGoldAmount(float amount)
    {
        gold += amount;
        goldText.text = "Gold: " + gold.ToString();
    }

}
