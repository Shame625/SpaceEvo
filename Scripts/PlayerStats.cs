using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerStats : MonoBehaviour {

    public Dictionary<string, float> baseModuleHp = new Dictionary<string, float>();
    public Dictionary<string, float> moduleHp = new Dictionary<string, float>(); 
    void Awake()
    {
        baseModuleHp.Add("coreModule", 100);
        baseModuleHp.Add("bridgeModule", 15);
        baseModuleHp.Add("gunModule", 10);
        baseModuleHp.Add("engineModule", 5);
        baseModuleHp.Add("hpModule", 20);
        baseModuleHp.Add("magnetModule", 15);
        baseModuleHp.Add("playergunModule", 15);
        baseModuleHp.Add("shieldModule", 30);
        baseModuleHp.Add("turningModule", 5);
        baseModuleHp.Add("grinderModule", 0);

        moduleHp.Add("coreModule", 100);
        moduleHp.Add("bridgeModule", 15);
        moduleHp.Add("gunModule", 10);
        moduleHp.Add("engineModule", 5);
        moduleHp.Add("hpModule", 20);
        moduleHp.Add("magnetModule", 15);
        moduleHp.Add("playergunModule", 15);
        moduleHp.Add("shieldModule", 30);
        moduleHp.Add("turningModule", 5);
        moduleHp.Add("grinderModule", 0);
    }

    //This is what each module gets

    //Core Module
    public Vector2 coreModuleSize =new Vector2(0.75f, 0.75f);
    public float coreModuleBaseHP = 100;

    //Engine Module
    public float maximumSpeed = 1;
    public float Acceleration = 0.5f;
    public float Deceleration = 0.5f;
    public float engineModuleBaseHP = 5;

    //Rotation module
    public float rotationSpeed = 10;
    public float rotationModuleBaseHP = 5;

    //Player Controlled Gun Module
    public float playergun_Dmg = 2;
    public float playergun_ReloadRate = 1.5f;
    public float playergun_Range = 10;
    public float playergunModuleBaseHP = 15;

    //Auto Gun Module
    public float autogun_Dmg = 0.5f;
    public float autogun_ReloadRate = 1;
    public float autogun_Range = 5f;
    public float autogunModuleBaseHP = 10;

    //Grinder Module
    public float grinder_Dmg = 5;
    public float grinder_ReloadRate = 1;
    public float grinder_Range = 1;

    //Hp module
    public float hp_Yield = 5;
    public float hp_Perfive = 1;
    public float hpModuleBaseHP = 20;

    //Shield module
    public float shield_Yield = 30;
    public float shield_Perfive = 30;
    public float shieldModuleBaseHP = 30;

    //Magnet module
    public float magnet_Radius = 5;
    public float magnet_Strenght = 10;
    public float magnetModuleBaseHP = 15;

    //Bridge module
    public float bridgeModuleBaseHP = 15;

    public void Upgrade(string Name, Dictionary<string, int> currentLevels)
    {
        switch (Name)
        {
            case "coreModule":
                coreModuleSize = new Vector2(0.75f + currentLevels[Name] * 0.5f, 1f + currentLevels[Name] * 0.5f);
                coreModuleBaseHP = 100 + currentLevels[Name] * 20;
                baseModuleHp["coreModule"] = coreModuleBaseHP;
                break;
            case "engineModule":
                    maximumSpeed = 1 + currentLevels[Name] * 0.25f;
                    Acceleration = 0.5f + currentLevels[Name] * 0.25f;
                    Deceleration = 0.5f + currentLevels[Name] * 0.25f;
                    engineModuleBaseHP = 5 + currentLevels[Name] * 5;
                    baseModuleHp["engineModule"] = engineModuleBaseHP;
                //if change occurs, recalculate speed!
                    GetComponent<Player>().CalculateMovement();
                break;
            case "turningModule":
                rotationSpeed = 10 + currentLevels[Name] * 2.5f;
                rotationModuleBaseHP = 5 + currentLevels[Name] * 5;
                baseModuleHp["turningModule"] = rotationModuleBaseHP;
                //Movement must be recalculated
                GetComponent<Player>().CalculateMovement();
                break;
            case "playergunModule":
                playergunModuleBaseHP = 15 + currentLevels[Name] * 15f;
                playergun_Dmg = 2 + currentLevels[Name] * 2f;
                playergun_ReloadRate = 1.5f - currentLevels[Name] * 0.1f;
                playergun_Range = 10 + currentLevels[Name] * 1;
                baseModuleHp["playergunModule"] = playergunModuleBaseHP;
                break;
            case "gunModule":
                autogunModuleBaseHP = 10 + currentLevels[Name] * 10;
                autogun_Dmg = 0.5f + currentLevels[Name] * 0.5f;
                autogun_ReloadRate = 2 - currentLevels[Name] * 0.1f;
                autogun_Range = 5 + currentLevels[Name] * 1.25f;
                baseModuleHp["gunModule"] = autogunModuleBaseHP;
                break;
            case "grinderModule":
                grinder_Dmg = 5 + currentLevels[Name] * 3f;
                grinder_ReloadRate = 1 - currentLevels[Name] * 0.05f;
                grinder_Range = 7.5f + currentLevels[Name] * 0.5f;
                break;
            case "hpModule":
                hpModuleBaseHP = 20 + currentLevels[Name] * 20;
                hp_Yield = 5 + currentLevels[Name] * 5;
                hp_Perfive = 5 + currentLevels[Name] * 1.5f;
                baseModuleHp["hpModule"] = hpModuleBaseHP;
                break;
            case "shieldModule":
                shieldModuleBaseHP = 15 + currentLevels[Name] * 15;
                shield_Yield = 50 + currentLevels[Name] * 10f;
                shield_Perfive = 10 + currentLevels[Name] * 2.5f;
                baseModuleHp["shieldModule"] = shieldModuleBaseHP;
                break;
            case "magnetModule":
                magnetModuleBaseHP = 15 + currentLevels[Name] * 15;
                magnet_Radius = 5 + currentLevels[Name] * 2.5f;
                magnet_Strenght = 10 + currentLevels[Name] * 5f;
                baseModuleHp["magnetModule"] = magnetModuleBaseHP;
                break;

            case "bridgeModule":
                bridgeModuleBaseHP = 15 + currentLevels[Name] * 15;
                baseModuleHp["bridgeModule"] = bridgeModuleBaseHP;
                break;
            default:
                break;
        }
    }

}
