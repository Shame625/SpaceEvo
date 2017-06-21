using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ModuleContainer : MonoBehaviour {

	public List<GameObject> gameModulesList = new List<GameObject> ();
	public Dictionary<string,GameObject> gameModules = new Dictionary<string, GameObject> ();
	public Dictionary<string, totalModuleAndUpgrade> totalModulesAndUpgrades = new Dictionary<string, totalModuleAndUpgrade>();

    //Shop lists
    public Dictionary<string, GameObject> gameModuleForSale = new Dictionary<string, GameObject>();
    public List<GameObject> gameModuleForSaleList = new List<GameObject>();
    public List<string> upgradeForSale = new List<string>();

	void Awake()
	{
        //Generates lists, all modules ingame, modules for sale, upgrades for sale
		foreach (GameObject go in gameModulesList) {
			gameModules.Add (go.name, go);
			totalModulesAndUpgrades.Add (go.name, new totalModuleAndUpgrade (0, 0));

            if (go.name != "coreModule")
            {
                gameModuleForSale.Add(go.name, go);
                gameModuleForSaleList.Add(go);
            }
            upgradeForSale.Add(go.name);
		}
	}
}

public class totalModuleAndUpgrade{

	public int totalModule;
	public int totalUpgrade;

	public totalModuleAndUpgrade(int module,int upgrade)
	{
		totalModule = module;
		totalUpgrade = upgrade;
	}
	public void IncreaseValues(int module, int upgrade)
	{
		totalModule += module;
		totalUpgrade += upgrade;
	}
	public void IncreaseModuleCount(int module)
	{
		totalModule += module;
	}
	public void Reset()
	{
		totalModule = 0;
		totalUpgrade = 0;
	}
}