using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using BreakInfinity;
using System;

public class GameManager : MonoBehaviour
{
    public BigDouble money = 0f;
    public BigDouble startMoney;

    public TextMeshProUGUI moneyDisp;
    public TextMeshProUGUI offlineDisp;
    public TextMeshProUGUI offlineTimeDisp;

    public GameObject offlinePanel;

    private DataPersist DP;

    public Generator[] generators;

    // Start is called before the first frame update
    void Start()
    {
        DP = GetComponent<DataPersist>();

        LoadGame();
    }

    // Update is called once per frame
    void Update()
    {
        moneyDisp.text = "Money: " + SciNotToUSName(money);
        offlineDisp.text = SciNotToUSName(money - startMoney);
        SaveGame();
    }

    public void CloseOfflinePanel()
    {
        Destroy(offlinePanel);
    }

    private BigDouble CalculateOfflineTime()
    {
        BigDouble timeElapsed = DateTime.Now.Subtract(DP.GD.saveTime).TotalSeconds;
        BigDouble maxOfflineTime = new BigDouble(43200);

        if(timeElapsed > maxOfflineTime)
        {
            timeElapsed = maxOfflineTime;
        }

        if(timeElapsed < 0)
        {
            timeElapsed = 0;
        }

        return timeElapsed;
    }

    private void ProcessOfflineProduction(BigDouble timePassed)
    {
        if(timePassed < 5)
        {
            CloseOfflinePanel();
        }

        for(int i = generators.Length -1; i >= 0; i--)
        {
            if(generators[i].quantity > 0)
            {
                generators[i].time += timePassed;
            }
        }
        offlineTimeDisp.text = TimeSpan.FromSeconds(CalculateOfflineTime().ToDouble()).ToString(@"hh\:mm\:ss");
    }

    public string SciNotToUSName(BigDouble num)
    {
        string displayNumber = $"{(num.Mantissa * BigDouble.Pow(10, num.Exponent % 3)):G3} ";
        int prefixIndex = (int)BigDouble.Floor(BigDouble.Abs(num.Exponent)).ToDouble();
        string name = "";
        int prefixOffset = 0;

        if (num.Exponent < 33)
        {
            prefixIndex /= 3;
            name += $"{Prefixes.prefixes[prefixIndex]}";
            return displayNumber + name;
        }
        else
        {
            prefixIndex = (prefixIndex - 3) / 3;
            int tempPrefixIndex = prefixIndex;
            List<int> prefixList = new();
            for (int i = 0; i < prefixIndex.ToString().Length; i++)
            {
                int lastNum = tempPrefixIndex % 10;
                prefixList.Add(lastNum);
                tempPrefixIndex /= 10;
                name += Prefixes.allPrefixes[prefixList[i] + prefixOffset];
                prefixOffset += 10;
            }
            return $"{displayNumber} {name}";
        }
    }

    public void DeleteSave()
    {
        if (DP.TryLoad())
        {
            money = BigDouble.Zero;
            money += 11;
            generators[0].quantity = 0;
            generators[1].quantity = 0;
            generators[2].quantity = 0;
            SaveGame();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }

    private void LoadGame()
    {
        if (DP.TryLoad())
        {
            DP.LoadData();

            money = DP.GD.money;
            startMoney = money;
            generators[0].quantity = DP.GD.gen1;
            generators[1].quantity = DP.GD.gen2;
            generators[2].quantity = DP.GD.gen3;
            ProcessOfflineProduction(CalculateOfflineTime());
        }
        else
        {
            CloseOfflinePanel();
            money += 11;
            generators[0].quantity = 0;
            generators[1].quantity = 0;
            generators[2].quantity = 0;
            SaveGame();
        }
    }

    private void SaveGame()
    {
        DP.GD.money = money;
        DP.GD.gen1 = generators[0].quantity;
        DP.GD.gen2 = generators[1].quantity;
        DP.GD.gen3 = generators[2].quantity;

        DP.SaveData();
    }
}
