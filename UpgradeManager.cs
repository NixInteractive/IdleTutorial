using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script is what makes our upgrade work. Attach it to any buttons that will be used for upgrades.
public class UpgradeManager : MonoBehaviour
{
    public GameManager GM; //GameManager reference

    //Upgrade value and tier
    public float value;
    public float valueTier;

    //Upgrade cost and tier
    public float cost;
    public float costTier;

    //Times upgrade has been purchased
    private float quantity;

    //Base upgrade cost
    [SerializeField] private float baseCost;

    //Text references to display cost and value to the player
    public Text costDisp;
    public Text valueDisp;

    //Available upgrade types
    public enum UpgradeTypes
    {
        MPC,
        MPS
    }

    //This upgrade's type
    public UpgradeTypes upgradeType;

    // Start is called before the first frame update
    public void Start()
    {
        cost = baseCost; //Set the upgrade cost to the base upgrade cost
    }

    // Update is called once per frame
    public void Update()
    {
        //Clamp the tiers
        valueTier = Mathf.Clamp(valueTier, 0, GM.suffixes.Length);
        costTier = Mathf.Clamp(costTier, 0, GM.suffixes.Length);

        //Set our cost display
        costDisp.text = "Cost: " + GM.CurrencyText(cost, costTier);

        //Set our value display based on the upgrade type
        if (upgradeType == UpgradeTypes.MPC)
        {
            valueDisp.text = "MPC: +" + GM.CurrencyText(value, valueTier);
        }
        else if (upgradeType == UpgradeTypes.MPS)
        {
            valueDisp.text = "MPS: +" + GM.CurrencyText(value, valueTier);
        }
    }

    //The Upgrade function
    public void OnUpgradeClick()
    {
        float oldValue = 0; //Previous upgrade value

        //Set the old value based on upgrade type
        if(upgradeType == UpgradeTypes.MPC)
        {
            oldValue = GM.mpc;
        }
        else
        {
            oldValue = GM.mps;
        }

        //Call the OnUpgrad emethod from our GameManager script
        GM.OnUpgrade(costTier, cost, upgradeType, valueTier, value);

        //Check old value vs current value to see if upgrade was successful
        if(upgradeType == UpgradeTypes.MPC)
        {
            if(oldValue != GM.mpc)
            {
                quantity++;
            }
        }
        else
        {
            if(oldValue != GM.mps)
            {
                quantity++;
            }
        }

        //Adjust the cost
        cost = baseCost * Mathf.Pow(1.05f, quantity);

        Debug.Log(quantity);
    }
}
