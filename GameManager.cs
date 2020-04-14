using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This script can be attached to any object in your scene. I created a dedicated manager object for it.
public class GameManager : MonoBehaviour
{
    public float money; //Player money
    public float mpc; //Player money per click
    public float mps; //Player money per second

    //Currency conversion tiers
    public float moneyTier = 1;
    public float mpcTier = 1;
    public float mpsTier = 1;

    public string[] suffixes; //Available suffixes for currency display

    //Display references so the player can see these values
    public Text moneyDisp; 
    public Text mpcDisp;
    public Text mpsDisp;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AutoTick()); //Starts our AutoTick Coroutine
    }

    // Update is called once per frame
    void Update()
    {

        //Assigns our GUI text to display the correct values
        moneyDisp.text = "Money: " + CurrencyText(money, moneyTier);
        mpcDisp.text = "Per Click: " + CurrencyText(mpc, mpcTier);
        mpsDisp.text = "Per Sec: " + CurrencyText(mps, mpsTier);

        //Adjust our tiers based on our values
        #region TierIncrementation
        if (money >= 1000)
        {
            money /= 1000;
            moneyTier++;
        }

        if (mpc >= 1000)
        {
            mpc /= 1000;
            mpcTier++;
        }

        if (mps >= 1000)
        {
            mps /= 1000;
            mpsTier++;
        }

        if (money <= 0)
        {
            money *= 1000;
            moneyTier--;
        }

        if (mpc <= 0)
        {
            mpc *= 1000;
            mpcTier--;
        }

        if (mps <= 0)
        {
            mps *= 1000;
            mpsTier--;
        }
        #endregion

        //Clamps all of our tiers and our current money.
        moneyTier = Mathf.Clamp(moneyTier, 1, suffixes.Length);
        mpcTier = Mathf.Clamp(mpcTier, 1, suffixes.Length);
        mpsTier = Mathf.Clamp(mpsTier, 1, suffixes.Length);
        money = Mathf.Clamp(money, 1, Mathf.Infinity);

    }

    //Creates the string that will be displayed to our player.
    public string CurrencyText(float currency, float currencyTier)
    {
        string currencyText = currency.ToString("#.00") + suffixes[(int)currencyTier - 1];

        return currencyText;
    }

    //The click for currency function
    public void OnButtonPress()
    {
        if (mpcTier > moneyTier)
        {
            money += mpc * Mathf.Pow(1000, mpcTier - moneyTier);
        }
        else if (mpcTier < moneyTier)
        {
            money += mpc / Mathf.Pow(1000, moneyTier - mpcTier);
        }
        else
        {
            money += mpc;
        }
    }

    //The money per second function
    public void IdleIncome()
    {
        if (mpsTier > moneyTier)
        {
            money += (mps * Mathf.Pow(1000, mpsTier - moneyTier)) * 0.01f;
        }
        else if (mpsTier < moneyTier)
        {
            money += (mps / Mathf.Pow(1000, moneyTier - mpsTier)) * 0.01f;
        }
        else
        {
            money += mps * 0.01f;
        }
    }

    //Removes money when an upgrade is purchased
    public void OnUpgrade(float costTier, float cost, UpgradeManager.UpgradeTypes type, float valueTier, float value)
    {
        if (moneyTier >= costTier)
        {
            if (moneyTier == costTier)
            {
                if (money >= cost)
                {
                    money -= cost;
                    ValueAdd(type, value, valueTier);
                }
            }
            else
            {
                money -= cost / Mathf.Pow(1000, moneyTier - costTier);
                ValueAdd(type, value, valueTier);
            }
        }
    }

    //Applies purchased upgrade
    public void ValueAdd(UpgradeManager.UpgradeTypes type, float value, float valueTier)
    {
        if (type == UpgradeManager.UpgradeTypes.MPC)
        {
            if (mpcTier == valueTier)
            {
                mpc += value;
            }
            else if (mpcTier < valueTier)
            {
                mpc += value * Mathf.Pow(1000, valueTier - mpcTier);
            }
            else
            {
                mpc += value / Mathf.Pow(1000, mpcTier - valueTier);
            }
        }

        if (type == UpgradeManager.UpgradeTypes.MPS)
        {
            if (mpsTier == valueTier)
            {
                mps += value;
            }
            else if (mpsTier < valueTier)
            {
                mps += value * Mathf.Pow(1000, valueTier - mpsTier);
            }
            else
            {
                mps += value / Mathf.Pow(1000, mpsTier - valueTier);
            }
        }
    }

    //Calls our IdleIncome Function once per second.
    IEnumerator AutoTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.01f);
            IdleIncome();
        }
    }
}
