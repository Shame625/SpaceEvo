using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    public Dictionary<string, string> displayNames = new Dictionary<string, string>();
    public Dictionary<string, double> modulebaseCosts = new Dictionary<string, double>();

    public Dictionary<string, string> upgradedisplayNames = new Dictionary<string, string>();
    public Dictionary<string, double> upgradebaseCosts = new Dictionary<string, double>();

    public Dictionary<string, double> currentModuleCost = new Dictionary<string, double>();
    public Dictionary<string, double> currentUpgradeCost = new Dictionary<string, double>();

    public GameObject buildingBackGround;
    public GameObject gameBackGround;

	public GameObject playerObjTemplate;

	public static float offsetX = 0.75f;
	public static float offsetY = 0.5f;

	public ModuleContainer moduleContainer;

	public GameObject Player;

	public Transform Everything;

	public Player player;
    public double Currency;

    public static Color[] LevelColors = {
                                    Color.gray, Color.white, new Color(0.678f, 1, 0.184f), Color.green, new Color(0.282f, 0.820f, 0.800f), new Color(0.255f, 0.412f, 0.882f),
                                    Color.cyan, new Color(0.482f, 0.408f, 0.933f), new Color(0.502f, 0, 0.502f), Color.yellow, new Color(1, 0.549f, 0), new Color(0, 0.502f, 0.502f)
                                 };

    public bool doingDmg_Hazard = false;

	void Awake()
    {
        PlayerPrefs.DeleteAll();
        //Load from file later, this is fine for now I guess
        RewriteMe();
        foreach (var pair in upgradebaseCosts)
        {
            currentUpgradeCost.Add(pair.Key, pair.Value);
        }

        foreach (var pair in modulebaseCosts)
        {
            currentModuleCost.Add(pair.Key, pair.Value);
        }

        moduleContainer = GetComponent<ModuleContainer> ();
	}

    void RewriteMe()
    {
        upgradebaseCosts.Add("coreModule", 1000);
        upgradebaseCosts.Add("bridgeModule", 200);
        upgradebaseCosts.Add("gunModule", 1000);
        upgradebaseCosts.Add("engineModule", 2000);
        upgradebaseCosts.Add("grinderModule", 5000);
        upgradebaseCosts.Add("hpModule", 10000);
        upgradebaseCosts.Add("magnetModule", 20000);
        upgradebaseCosts.Add("playergunModule", 5000);
        upgradebaseCosts.Add("shieldModule", 2500);
        upgradebaseCosts.Add("turningModule", 1000);

        upgradedisplayNames.Add("coreModule", "Core Module Upgrade");
        upgradedisplayNames.Add("bridgeModule", "Bridge Upgrade");
        upgradedisplayNames.Add("gunModule", "Auto Gun Upgrade");
        upgradedisplayNames.Add("engineModule", "Engine Upgrade");
        upgradedisplayNames.Add("grinderModule", "Grinder Upgrade");
        upgradedisplayNames.Add("hpModule", "HP Module Upgrade");
        upgradedisplayNames.Add("magnetModule", "Magnet Upgrade");
        upgradedisplayNames.Add("playergunModule", "Controlled Gun");
        upgradedisplayNames.Add("shieldModule", "Shield Expansion");
        upgradedisplayNames.Add("turningModule", "Truning Module");

        modulebaseCosts.Add("bridgeModule", 2);
        modulebaseCosts.Add("gunModule", 10);
        modulebaseCosts.Add("engineModule", 20);
        modulebaseCosts.Add("grinderModule", 5);
        modulebaseCosts.Add("hpModule", 100);
        modulebaseCosts.Add("magnetModule", 2000);
        modulebaseCosts.Add("playergunModule", 50);
        modulebaseCosts.Add("shieldModule", 25);
        modulebaseCosts.Add("turningModule", 100);

        displayNames.Add("bridgeModule", "Bridge");
        displayNames.Add("gunModule", "Auto Gun");
        displayNames.Add("engineModule", "Engine");
        displayNames.Add("grinderModule", "Grinder");
        displayNames.Add("hpModule", "HP Module");
        displayNames.Add("magnetModule", "Magnet");
        displayNames.Add("playergunModule", "Controlled Gun");
        displayNames.Add("shieldModule", "Shield Module");
        displayNames.Add("turningModule", "Truning Module");

    }

	void Start()
	{
        if (PlayerPrefs.HasKey("PlayerModules"))
        {
            if (PlayerPrefs.GetString("PlayerModules") != "")
            {
                GetComponent<SaveLoad>().LoadPlayer();
            }
        }
        else
        {
            GenerateStartingShip();
        }
        GetComponent<Shop>().GenerateUI();
        GetComponent<Builder>().UI.DisplayPlayerCurrency(Currency);
	}

	void Update()
	{
		if (Input.GetKeyDown (KeyCode.B)) {
			GetComponent<Builder>().Build ();
		}
		if (Input.GetKeyDown (KeyCode.F)) {
			GetComponent<Shop> ().DebugBuy();
		}

		if (Input.GetKeyDown (KeyCode.I)) {
			GetComponent<SaveLoad> ().SavePlayer ();
		}
		if (Input.GetKeyDown (KeyCode.O)) {
			GetComponent<SaveLoad> ().LoadPlayer ();
		}
	}

	void GenerateStartingShip()
	{
		//Loading Will Happen here
		Player = (GameObject)Instantiate(playerObjTemplate,Vector3.zero,Quaternion.identity);
		Player.name = "Player";

		player = Player.GetComponent<Player> ();
		//Player.transform.SetParent (Everything);
		//Construct Starting Ship
		//DeleteThis later, and only spawn core
        GameObject core = (GameObject)Instantiate(moduleContainer.gameModules["coreModule"], Vector2.zero, Quaternion.identity);
        core.name = moduleContainer.gameModules["coreModule"].name;
		core.transform.SetParent (Player.transform);
		player.playerModules.Add (core);
		float offset = -offsetX;
        
		foreach (GameObject go in moduleContainer.gameModulesList) {
			player.InitializeModulesAndUpgrades (go.name, 0, 0);
		}

        core.GetComponent<Module>().SetColor(LevelColors[player.ownedUpgrades[core.name]]);
		for (int i = 0; i <= 2; i++) 
		{
			GameObject go;
				if (i != 2) {
				go = (GameObject)Instantiate (moduleContainer.gameModules["engineModule"], new Vector2 (offset, -0.5f), Quaternion.identity);
				go.name = moduleContainer.gameModules["engineModule"].name;
					go.transform.SetParent (Player.transform);
				offset = offsetX;
				} else 
				{
				go = (GameObject)Instantiate (moduleContainer.gameModules["playergunModule"], new Vector2 (0, 1), Quaternion.identity);
                go.name = moduleContainer.gameModules["playergunModule"].name;
				go.transform.SetParent (Player.transform);
				}
			moduleContainer.totalModulesAndUpgrades [go.name].IncreaseModuleCount (1);
			player.playerModules.Add (go);
            go.GetComponent<Module>().SetColor(LevelColors[player.ownedUpgrades[go.name]]);
		}
        player.GetModules();
        player.LoadUpgrades();
        player.RecalculateHp();
        
		SetCameraTarget (Player.transform);

	}
		

	public void SpawnPlayer(List<PlayerOwnedModulesAndUpgrades> playerOwnedItems,List<PlayerBuildingBlocks> playerModules)
	{
		Player = (GameObject)Instantiate(playerObjTemplate,Vector3.zero,Quaternion.identity);
		Player.name = "Player";
		player = Player.GetComponent<Player> ();
		//Set player properties
		foreach (PlayerOwnedModulesAndUpgrades item in playerOwnedItems) 
		{
			player.InitializeModulesAndUpgrades (item.Name, item.ownedModules, item.ownedUpgrades);
			moduleContainer.totalModulesAndUpgrades [item.Name].IncreaseValues (item.ownedModules, item.ownedUpgrades);
		}

		//Spawn ship      
		foreach (PlayerBuildingBlocks block in playerModules) 
		{
			GameObject go;
			go = (GameObject)Instantiate (moduleContainer.gameModules[block.Name], block.Position, Quaternion.identity);		

            go.name = moduleContainer.gameModules[block.Name].name;

            //Set colors of modules
            go.GetComponent<Module>().SetColor(LevelColors[player.ownedUpgrades[go.name]]);

            go.transform.SetParent(Player.transform);
            moduleContainer.totalModulesAndUpgrades[block.Name].IncreaseModuleCount(1);
			player.playerModules.Add (go);
		}
        player.GetModules();
        player.LoadUpgrades();
        player.RecalculateHp();
        
		SetCameraTarget (Player.transform);
	}

	public void SetCameraTarget(Transform target)
	{
		Camera.main.GetComponent<SmoothCamera2D> ().target = target;
	}


	public void DebugPlayerMovementRotationLeft(bool condition)
	{
		Player.GetComponent<PlayerMovement> ().rotateLeft = condition;
	}
	public void DebugPlayerMovementRotationRight(bool condition)
	{
		Player.GetComponent<PlayerMovement> ().rotateRight = condition;
	}

	public void DebugPlayerMoveForward(bool condition)
	{
		Player.GetComponent<PlayerMovement> ().goForward = condition;
	}

	public void DebugPlayerMoveBackwards(bool condition)
	{
		Player.GetComponent<PlayerMovement> ().goBackwards = condition;
	}


    //maybe fix for a bug
    public void CalculateNewCosts()
    {
        foreach (var pair in moduleContainer.gameModuleForSale)
        {
            currentModuleCost[pair.Key] = GenerateNewCostModule(pair.Key);
            GetComponent<Shop>().RegenerateBuildCost(pair.Key, false);
        }
        foreach (var str in moduleContainer.upgradeForSale)
        {
            currentUpgradeCost[str] = GenerateNewCostUpgrade(str);
            GetComponent<Shop>().RegenerateBuildCost(str, true);
        }
    }

    public double GenerateNewCostModule(string Name)
    {
        if (player.ownedModules[Name] != 0)
        {
            double newCost = System.Math.Pow((double)modulebaseCosts[Name] * (double)player.ownedModules[Name], 1.5);
            newCost = (10 - newCost % 10) + newCost;
            currentModuleCost[Name] = System.Math.Round(newCost);
            return System.Math.Round(newCost);
        }
        return modulebaseCosts[Name];
    }
    public double GenerateNewCostUpgrade(string Name)
    {
        if (player.ownedUpgrades[Name] != 0)
        {
            double newCost = System.Math.Pow((double)upgradebaseCosts[Name] * (double)player.ownedUpgrades[Name], 1.6f);
            newCost = (10 - newCost % 10) + newCost;
            SetNewColors(Name);
            return System.Math.Round(newCost);
        }
        return upgradebaseCosts[Name];
    }

    public void SetNewColors(string Name)
    {
        foreach (GameObject go in player.playerModules)
        {
            if (go.name == Name)
            {
                go.GetComponent<Module>().SetColor(LevelColors[player.ownedUpgrades[go.name]]);
            }
        }
    }

    public void BackgroundSwitch(bool condition)
    {
        if (condition)
        {
            buildingBackGround.SetActive(true);
            gameBackGround.SetActive(false);
        }
        else
        {
            buildingBackGround.SetActive(false);
            gameBackGround.SetActive(true);
        }
    }

    public void PlayerInHazard(bool condition)
    {
        if (condition && !doingDmg_Hazard && player.currentHP>=0)
        {
            InvokeRepeating("DoHazardDmg", 1.0f, 1.0f);
            doingDmg_Hazard = true;
        }
        else if(!condition && doingDmg_Hazard && player.currentHP>=0)
        {
            CancelInvoke("DoHazardDmg");
            doingDmg_Hazard = false;
        }
    }

    void DoHazardDmg()
    {
        if (player.currentHP >= 0)
        {
            player.TakeRadiationDamage();
        }
    }
}
