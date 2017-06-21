using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Shop : MonoBehaviour {
	
	UIManager UI;
	GameManager GM;

	List<string> keys;

	void Awake()
	{
		GM = GetComponent<GameManager> ();
		UI = GameObject.FindGameObjectWithTag ("UI").GetComponent<UIManager> ();
	}

	void Start()
	{
        keys = new List<string>(GM.moduleContainer.gameModuleForSale.Keys);
	}

    public void RegenerateBuildCost(string Name, bool condition)
    { 
        //if true regenerate upgrade
        if (condition)
        {
            UI.buyUpgradesButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCost(GM.GenerateNewCostUpgrade(Name));
        }
        else
            //if false regenerate module
            UI.buyModuleButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCost(GM.GenerateNewCostModule(Name));
        
    }

	public void BuyItem(string Name)
	{
        if (GM.Currency >= GM.currentModuleCost[Name])
        {
            GetComponent<Builder>().SelectItem(Name);
            GM.Currency -= GM.currentModuleCost[Name];

            //missing code to calcualte cost
            GM.player.ownedModules[Name]++;
            //Yolo way to cheat UI...
            UI.RefreshInventoryItem(Name, GM.player.ownedModules[Name]);
            GetComponent<SaveLoad>().SavePlayer();
            UI.buyModuleButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCurrentOwnedItems(GM.moduleContainer.totalModulesAndUpgrades[Name].totalModule);
            UI.buyModuleButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCost(GM.GenerateNewCostModule(Name));

            UI.DisplayPlayerCurrency(GM.Currency);
        }
        CheckIfCanBuyHighlightings();
	}

    public void BuyUpgrade(string Name)
    {
        if (GM.Currency >= GM.currentUpgradeCost[Name])
        {
            //uh oh check for max upgrade, only 1 case....
            if (GM.moduleContainer.totalModulesAndUpgrades[Name].totalUpgrade >= 10)
            {
                //Do nada
            }
            else
            {

                GM.Currency -= GM.currentUpgradeCost[Name];
                GM.player.ownedUpgrades[Name]++;
                GetComponent<SaveLoad>().SavePlayer();
                UI.buyUpgradesButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCurrentOwnedItems(GM.moduleContainer.totalModulesAndUpgrades[Name].totalUpgrade);
                UI.buyUpgradesButtons[Name].GetComponent<ShopItem_ModuleButton>().SetCost(GM.GenerateNewCostUpgrade(Name));
                //actually upgrade it
                GM.player.Upgrade(Name);
                //incase we get to max level
                if (GM.moduleContainer.totalModulesAndUpgrades[Name].totalUpgrade >= 10)
                {
                    UI.buyUpgradesButtons[Name].GetComponent<ShopItem_ModuleButton>().MaxUpgradeLevelReached();
                }

                if (Name == "coreModule")
                {
                    GetComponent<Builder>().RegenerateBuildingSpots();
                }
                GM.player.ChangeOccured(Name);
            }
            UI.DisplayPlayerCurrency(GM.Currency);
        }
        CheckIfCanBuyHighlightings();
    }

	public void DebugBuy()
	{
		foreach (string key in keys) 
		{
                BuyItem(key);
                BuyItem(key);
                BuyItem(key);
                BuyItem(key);
                BuyItem(key);
		}
	}

    public void GenerateUI()
    {
        GenerateShopItems_Modules();
        GenerateShopItems_Upgrades();
    }

    void GenerateShopItems_Modules()
    {
        List<ShopItem> myShopItems = new List<ShopItem>();
        foreach (GameObject go in GM.moduleContainer.gameModuleForSaleList)
        {
            myShopItems.Add(new ShopItem(go.name, GM.displayNames[go.name], GM.GenerateNewCostModule(go.name), go.GetComponent<Module>().icon, GM.moduleContainer.totalModulesAndUpgrades[go.name].totalModule));
        }

        GM.GetComponent<Builder>().UI.GenerateShopUI(myShopItems);
        UI.CheckIfYouCanBuy(GM.Currency, GM.currentModuleCost, false);
    }

    void GenerateShopItems_Upgrades()
    {
        List<ShopItem> myShopItems = new List<ShopItem>();
        foreach (string str in GM.moduleContainer.upgradeForSale)
        {
            myShopItems.Add(new ShopItem(str, GM.upgradedisplayNames[str], GM.GenerateNewCostUpgrade(str), GM.moduleContainer.gameModules[str].GetComponent<Module>().icon, GM.moduleContainer.totalModulesAndUpgrades[str].totalUpgrade));
        }
        GM.GetComponent<Builder>().UI.GenerateUpgradeUI(myShopItems);
        UI.CheckIfYouCanBuy(GM.Currency, GM.currentUpgradeCost, true);
    }

    void CheckIfCanBuyHighlightings()
    {
        UI.CheckIfYouCanBuy(GM.Currency, GM.currentModuleCost, false);
        UI.CheckIfYouCanBuy(GM.Currency, GM.currentUpgradeCost, true);
    }
}

public class ShopItem
{
    public string Name;
    public string DisplayName;
    public double baseCost;
    public Sprite ShopImage;
    public int totalModules;

    public ShopItem(string name, string displayName, double basecost, Sprite shopImage, int totalmodules)
    {
        Name = name;
        DisplayName = displayName;
        baseCost = basecost;
        ShopImage = shopImage;
        totalModules = totalmodules;
    }
}