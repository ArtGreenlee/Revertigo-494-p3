using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class GoldStorage : MonoBehaviour
{
    public static GoldStorage instance;
    public float gold;
    public TextMeshProUGUI goldText;
    public Button gemsPerRoundUpgradeButton;

    public List<float> gemsPerRoundUpgradeCost;
    private int gemsPerRoundUpgradeTier;

    private TowerInventory towerInventory;
    // Start is called before the first frame update
    void Start()
    {
        towerInventory = TowerInventory.instance;
        gemsPerRoundUpgradeTier = 0;
        goldText.text = "Gold " + gold.ToString();
        gemsPerRoundUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Increase Number of Gems Per Purchase.\n" +
           towerInventory.maxTowerInventory.ToString() + " -> " + (towerInventory.maxTowerInventory + 1).ToString() +
           " Cost: " + gemsPerRoundUpgradeCost[gemsPerRoundUpgradeTier];
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void upgradeGemsPerRound()
    {
        changeGoldAmount(-gemsPerRoundUpgradeCost[gemsPerRoundUpgradeTier]);
        towerInventory.maxTowerInventory++;
        gemsPerRoundUpgradeTier++;
        gemsPerRoundUpgradeButton.GetComponentInChildren<TextMeshProUGUI>().text = "Increase Number of Gems Per Purchase.\n" +
           towerInventory.maxTowerInventory.ToString() + " -> " + (towerInventory.maxTowerInventory + 1).ToString() +
           " Cost: " + gemsPerRoundUpgradeCost[gemsPerRoundUpgradeTier];
    }

    public void changeGoldAmount(float amount)
    {
        gold += amount;
        if (gold >= gemsPerRoundUpgradeCost[gemsPerRoundUpgradeTier])
        {
            gemsPerRoundUpgradeButton.interactable = true;
        }
        else
        {
            gemsPerRoundUpgradeButton.interactable = false;
        }
        goldText.text = "Gold " + gold.ToString();
    }
}
