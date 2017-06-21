using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player : MonoBehaviour 
{
    //stats
    //just for display GUI
    public float MaximumHP;
    public float currentHP;

    public float hpModifier = 0;
    public float hp_Perfive = 0;

    public float MaximumShield;
    public float currentShield;
    public float shield_PerFive = 0;

    float cooldown = 5;

    PlayerStats playerStats;
    Builder GM;
    public GameObject[] bullets;

    List<string> keys;

    public float slerpCooldown;

    void Awake()
    {
        playerStats = GetComponent<PlayerStats>();
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Builder>();
    }

    public List<GameObject> destroyedModules = new List<GameObject>();
	public List<GameObject> playerModules = new List<GameObject>();
	public Dictionary<string, int> ownedModules = new Dictionary<string, int>();
	public Dictionary<string, int> ownedUpgrades = new Dictionary<string, int>();

    public List<Module> modules = new List<Module>();
    public List<Module> playerGuns = new List<Module>();
    public List<Module> autoGuns = new List<Module>();

	//Determination if there is connection to core of ship
	public List<GameObject> alreadyLooked = new List<GameObject>();

    float playerGunCooldown = 0;
    public float autoGunCooldown = 1;

    bool playerDebug = false;


    void Update()
    {
        //debug 
        Vector2 diff = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.L))
            playerDebug = !playerDebug;

        if (!Builder.isBuilding)
        {

                if (Input.GetMouseButtonDown(0))
                {   //Debug bullet
                    if (playerDebug)
                    {
                        GameObject go = (GameObject)Instantiate(bullets[0], diff, Quaternion.identity);
                        go.GetComponent<Bullet>().SetProperties(Color.cyan, 10, 50, 10);
                    }
                Shoot(0, Vector3.zero, Quaternion.identity);
                }


            if (hp_Perfive > 0 || shield_PerFive> 0)
            {
                if (cooldown > 0)
                {
                    cooldown -= Time.deltaTime;
                }
                else
                {
                    if (currentHP < MaximumHP)
                        RegenHp();
                    if (currentShield<MaximumShield)
                        RegenShield();

                    cooldown = 5;
                }
            }
            if (playerGunCooldown > 0)
            {
                playerGunCooldown -= Time.deltaTime;
            }

            if (autoGunCooldown > 0)
            {
                autoGunCooldown -= Time.deltaTime;
            }
            else
            {
                autoGunCooldown = playerStats.autogun_ReloadRate;
            }

            if (slerpCooldown <= 0)
            {
                slerpCooldown = 0.1f;
            }
            slerpCooldown = slerpCooldown - Time.deltaTime;
        }
    }

    public void RegenerateDestroyedModule()
    {
        foreach (GameObject go in destroyedModules)
        {
            go.GetComponent<Module>().alive = true;
            playerModules.Add(go);
            
        }
        foreach (Module mod in modules)
        {
            mod.alive = true;
        }
        destroyedModules.Clear();
    }

    public void RegenHp()
    {
        foreach (Module go in modules)
        {
            if (go.transform.gameObject.activeSelf)
            {
                if (go.currentHp <= go.maximumHp)
                {
                    go.currentHp += hp_Perfive;
                    if (go.currentHp > go.maximumHp)
                    {
                        go.currentHp = go.maximumHp;
                    }
                }
            }
        }
        CalculateHp();
    }

    public void RegenShield()
    {
        currentShield += shield_PerFive;
        if (currentShield > MaximumShield)
            currentShield = MaximumShield;
        GM.UI.Shield(currentShield, MaximumShield);
    }

	public void InitializeModulesAndUpgrades(string Name, int ownedModules, int ownedUpgrades)
	{
		
		this.ownedModules.Add (Name, ownedModules);
		this.ownedUpgrades.Add (Name, ownedUpgrades);
	}

	public void CheckIfConnectedToCore()
	{
		transform.GetChild (0).GetComponent<Module> ().GetNeighborsObjects ();
	}

    public void CheckIfConnectedToCoreAttacked()
    {
        transform.GetChild(0).GetComponent<Module>().GetNeighborsObjects();
        foreach (Module go in modules)
        {
            if (go.hasCore == false)
            {
                go.alive = false;
                go.currentHp = 0;
                go.PlayDead();
            }
        }
        foreach (Module go in modules)
        {
            go.hasCore = false;
        }
        alreadyLooked.Clear();
        CalculateHp();
        CalculateMovement();
    }

    public void Upgrade(string Name)
    {
        playerStats.Upgrade(Name, ownedUpgrades);
    }

    public void LoadUpgrades()
    {
        foreach (var pair in ownedUpgrades)
        {
            playerStats.Upgrade(pair.Key, ownedUpgrades);
            ChangeOccured(pair.Key);
        }
    }

    public void GetModules()
    {
        modules.Clear();
        autoGuns.Clear();
        playerGuns.Clear();
        foreach (GameObject go in playerModules)
        {
            modules.Add(go.GetComponent<Module>());
            if (go.name == "gunModule")
            {
                autoGuns.Add(go.GetComponent<Module>());
            }
            else if (go.name == "playergunModule")
            {
                playerGuns.Add(go.GetComponent<Module>());
            }
        }
        RecalculateHp();
    }

    public void CalculateMovement()
    {
        float temp_speed = 0;
        float temp_acc = 0;
        float temp_rotation = 20;
        foreach (GameObject go in playerModules)
        {
            if (go.GetComponent<Module>().alive)
            {
                if (go.name == "engineModule")
                {
                    temp_speed += playerStats.maximumSpeed;
                    temp_acc += playerStats.Acceleration;
                }
                if (go.name == "turningModule")
                {
                    temp_rotation += playerStats.rotationSpeed;
                }
            }
        }

        GetComponent<PlayerMovement>().SetSpeed(temp_speed, temp_acc);
        GetComponent<PlayerMovement>().SetRotation(temp_rotation);
    }

    public void RecalculateHp()
    {

        MaximumHP = 0;
        foreach (Module go in modules)
        {
            go.maximumHp = playerStats.moduleHp[go.name];
            go.currentHp = playerStats.moduleHp[go.name];
            MaximumHP += playerStats.moduleHp[go.name];
        }
        CalculateHp();
        SetStartHP();
        SetShield();
        GM.UI.PlayerHealthBar(currentHP, MaximumHP);
        GM.UI.Shield(currentShield, MaximumShield);
    }

    public void SetStartHP()
    {
        currentHP = MaximumHP;
    }

    public void HpModuleDestroyed()
    {
        List<GameObject> temp = new List<GameObject>(playerModules);
        int count = 0;
        
        foreach (Module go in modules)
        {
            if(go.name == "hpModule")
            {
                if(go.alive)
                {
                    count++;
                }
            }
            if (go.alive)
            {
                go.OverrideTakeDmg(playerStats.hp_Yield);
                go.maximumHp -= playerStats.hp_Yield;
            }
        }
        hp_Perfive = count * playerStats.hp_Perfive;
        foreach (Module mod in modules)
        {
            if (mod.alive)
            {
                mod.maximumHp = playerStats.baseModuleHp[mod.name] + count * playerStats.hp_Yield;
            }
        }
    }

    public void ShieldModuleDestroyed()
    {
        int count = 0;
        foreach (Module module in modules)
        {
            if (module.name == "shieldModule")
            {
                if(module.alive)
                count++;
            }
        }
        
        shield_PerFive = count * playerStats.shield_Perfive;
        MaximumShield = count * playerStats.shield_Yield;
        GM.UI.Shield(currentShield, MaximumShield);
    }

    public void ChangeOccured(string Name)
    {
        switch (Name) 
        { 
            case "engineModule":
                CalculateMovement();
                break;
            case "turningModule":
                CalculateMovement();
                break;
            case "gunModule":
                foreach (GameObject go in playerModules)
                {
                    if (go.name == "gunModule")
                    {
                        go.GetComponent<AutoGun>().SetRange(playerStats.autogun_Range, playerStats.autogun_ReloadRate);
                    }
                }
                break;
            case "playergunModule":

                break;
            case "hpModule":
                int hpModuleCount = 0;
                foreach (GameObject go in playerModules)
                {
                    if (go.name == "hpModule")
                    {
                        hpModuleCount++;
                    }
                }
                hpModifier = playerStats.hp_Yield * hpModuleCount;
                hp_Perfive = playerStats.hp_Perfive * hpModuleCount;
                break;
            case "shieldModule":
                int shieldModuleCount = 0;
                foreach (GameObject go in playerModules)
                {
                    if (go.name == "shieldModule")
                    {
                        shieldModuleCount++;
                    }
                    MaximumShield = shieldModuleCount * playerStats.shield_Yield;
                    shield_PerFive = playerStats.shield_Perfive * shieldModuleCount;
                }
                SetShield();
                break;
            default:
                break;
        }
        HpModule();
        HpChange(Name);
        RecalculateHp();
        GM.UI.SetShiled();
    }


    public void SetShield()
    {
        currentShield = MaximumShield;
    }

    public void HpModule()
    {
        keys = new List<string>(playerStats.baseModuleHp.Keys);
        foreach (string key in keys)
        {
            if (key != "grinderModule")
            {
                playerStats.moduleHp[key] = playerStats.baseModuleHp[key] + hpModifier;
                HpChange(key);
            }
        }
    }

    void HpChange(string Name)
    {
        if (Name != "grinderModule")
        {
            foreach (Module go in modules)
            {

                if (go.name == Name)
                {

                    go.maximumHp = playerStats.moduleHp[Name];
                    go.currentHp = playerStats.moduleHp[Name];
                }
            }
        }
    }

    public void Shoot(int bullet, Vector3 startingPosition, Quaternion myRotation)
    {
        if (!Builder.isBuilding)
        {
            
            switch (bullet)
            {
                case 0:
                        foreach (Module temp in playerGuns)
                        {
                                if (playerGunCooldown <= 0)
                                {
                                    if (temp.alive)
                                    {
                                        GameObject newGo = (GameObject)Instantiate(bullets[bullet], temp.GetComponent<playerGun>().shootStart.position, temp.GetComponent<playerGun>().movablePart.rotation);
                                        newGo.GetComponent<Bullet>().SetProperties(GameManager.LevelColors[ownedUpgrades["playergunModule"]], 15 + ownedUpgrades["playergunModule"], playerStats.playergun_Dmg, ownedUpgrades["playergunModule"]);
                                    }
                                }
                                else
                                {
                                    temp.GetComponent<playerGun>().LookAtMouse();
                                }

                        }
                        if (playerGunCooldown <= 0)
                        {
                            playerGunCooldown = playerStats.playergun_ReloadRate;
                        }
                    break;
                case 1:
                    GameObject go = (GameObject)Instantiate(bullets[bullet], startingPosition, myRotation);
                    go.GetComponent<Bullet>().SetProperties(GameManager.LevelColors[ownedUpgrades["gunModule"]], 10 + ownedUpgrades["gunModule"], playerStats.autogun_Dmg, ownedUpgrades["gunModule"]);
                    break;
                default:
                    break;
            }
        }
    }

    void CalculateHp()
    {
        float temp = 0;
        float currentMax = 0;
        foreach (GameObject go in playerModules)
        {
            temp += go.GetComponent<Module>().currentHp;
            currentMax += go.GetComponent<Module>().maximumHp;
        }
        currentHP = temp;
        GM.UI.Shield(currentShield, MaximumShield);
        GM.UI.PlayerHealthBar(currentHP, MaximumHP);
        GM.UI.SetCurrentMaxHp(currentMax, MaximumHP);
    }


    public void TakeDamageShield(float value)
    {
        currentShield -= value;
        GM.UI.Shield(currentShield, MaximumShield);
        if (currentShield < 0)
            GM.UI.Shield(0, MaximumShield);
    }

    public void TakeDamageIndividual(float value)
    {
        currentHP -= value;
        GM.UI.PlayerHealthBar(currentHP, MaximumHP);
    }


    public void TakeRadiationDamage()
    {
        foreach (Module mod in modules)
        {
            if (mod.alive)
            {
                mod.TakeDamage(mod.maximumHp * 0.1f);
            }
        }
    }
}
