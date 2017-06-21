using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIManager : MonoBehaviour {

	Dictionary <string, GameObject> inventoryButtons = new Dictionary<string, GameObject>();

    public Dictionary<string, GameObject> buyModuleButtons = new Dictionary<string, GameObject>();
    public Dictionary<string, GameObject> buyUpgradesButtons = new Dictionary<string, GameObject>();

    

	public GameObject SelectedItem;

    public GameObject playerCurrencyGO;

	//building stuff General
    
	public GameObject buildingModeObject;
	public GameObject buildButton;
	public GameObject legalSpotsButton;
    public GameObject showHideShopButton;

	public GameObject deleteButton;


	//Player inventory
	public GameObject inventoryItemPrefab;
	public GameObject Inventory;
    public GameObject inventoryContainer;
	//Shop
    public GameObject wholeShop;
    public GameObject modulesButton;
    public GameObject upgradesButton;

    public GameObject ModulesForSale;
    public GameObject shopModuleItemPrefab;
    public GameObject moduleShopContainer;

    public GameObject UpgradesForSale;
    public GameObject upgradeModuleItemPrefab;
    public GameObject upgradeShopContainer;

	//Debug for now, I hope :(
	public GameObject Debug2;
	public GameObject Debug3;

    //Health Bar
    public GameObject healthBar;
    public Image healthBarContent;
    public Image currentMaxHealth;
    public Text healthBarText;

    //Shield Bar
    public GameObject shieldBar;
    public Image shieldBarContent;
    public Text shieldBarText;

    GameObject selectedObject = null;

    public void DisplayPlayerCurrency(double currency)
    {
        playerCurrencyGO.GetComponent<Text>().text = "Scraps: " + currency.ToString();
    }

	public void GenerateInventoryUI(Dictionary<string, GameObject> modules, Dictionary<string,int> ownedItems)
	{
		foreach (var pair in modules) 
		{
                GameObject temp = (GameObject)Instantiate(inventoryItemPrefab, Vector3.zero, Quaternion.identity);
                temp.GetComponent<InventoryButton>().SetProperties(pair.Value.name);
                temp.transform.GetChild(0).GetComponent<Image>().sprite = pair.Value.GetComponent<Module>().icon;
                temp.transform.GetChild(1).GetChild(0).GetComponent<Text>().text = ownedItems[pair.Value.name].ToString();
                temp.transform.SetParent(Inventory.transform, false);
                inventoryButtons.Add(pair.Value.name, temp);
		}
	}


    public void GenerateShopUI(List<ShopItem> shopItems)
    {
        foreach (ShopItem item in shopItems)
        {
            GameObject temp = (GameObject)Instantiate(shopModuleItemPrefab, Vector3.zero, Quaternion.identity);
            temp.GetComponent<ShopItem_ModuleButton>().SetProperties(item.Name, item.DisplayName, item.baseCost, item.ShopImage, item.totalModules, false);
            //Set text
            temp.transform.SetParent(moduleShopContainer.transform, false);
            buyModuleButtons.Add(item.Name, temp);
        }
    }

    public void GenerateUpgradeUI(List<ShopItem> shopItems)
    {
        foreach (ShopItem item in shopItems)
        {
            GameObject temp = (GameObject)Instantiate(upgradeModuleItemPrefab, Vector3.zero, Quaternion.identity);
            temp.GetComponent<ShopItem_ModuleButton>().SetProperties(item.Name, item.DisplayName, item.baseCost, item.ShopImage, item.totalModules, true);
            //Set text
            temp.transform.SetParent(upgradeShopContainer.transform, false);
            buyUpgradesButtons.Add(item.Name, temp);
        }
    }

	public void RefreshInventoryItem(string ownedItems, int item)
	{
		inventoryButtons[ownedItems].transform.GetChild (1).GetChild (0).GetComponent<Text> ().text = item.ToString();
	}


    public void SelectInventoryObject(string name)
    {
        if (selectedObject != null)
        {
            if (selectedObject.name != name)
            {
                selectedObject.GetComponent<Image>().color = Color.white;
            }
        }
        inventoryButtons[name].GetComponent<Image>().color = Color.cyan;
        selectedObject = inventoryButtons[name].gameObject;
    }

	public void OpenBuildingMode(bool condition)
	{
		buildingModeObject.SetActive (condition);
		BuildButton(condition);
		Debug2.SetActive (!condition);
		Debug3.SetActive (!condition);
	}
	void BuildButton(bool condition)
	{
		if (condition) {
			buildButton.GetComponent<Image> ().color = Color.cyan;
			buildButton.transform.GetChild (0).GetComponent<Text> ().color = Color.cyan;
		} else 
		{
			buildButton.GetComponent<Image> ().color = Color.white;
			buildButton.transform.GetChild (0).GetComponent<Text> ().color = Color.white;
		}
	}
	public void LegalSpotsButton(bool condition)
	{
		if (condition) {
			legalSpotsButton.GetComponent<Image> ().color = Color.cyan;
			legalSpotsButton.transform.GetChild (0).GetComponent<Text> ().color = Color.cyan;
		} else 
		{
			legalSpotsButton.GetComponent<Image> ().color = Color.white;
			legalSpotsButton.transform.GetChild (0).GetComponent<Text> ().color = Color.white;
		}
	}

	public void DeleteionButton(bool condition)
	{
		if (condition) {
			deleteButton.GetComponent<Image> ().color = Color.red;
			deleteButton.transform.GetChild (0).GetComponent<Text> ().color = Color.red;
		} else 
		{
			deleteButton.GetComponent<Image> ().color = Color.white;
			deleteButton.transform.GetChild (0).GetComponent<Text> ().color = Color.white;
		}
	}

    public void OpenModulesForSale()
    {
        if (!ModulesForSale.activeSelf)
        {
            modulesButton.GetComponent<Image>().color = Color.cyan;
            upgradesButton.GetComponent<Image>().color = Color.white;
            ModulesForSale.SetActive(true);
            UpgradesForSale.SetActive(false);
        }
    }

    public void OpenUpgradesForSale()
    {
        if (ModulesForSale.activeSelf)
        {
            modulesButton.GetComponent<Image>().color = Color.white;
            upgradesButton.GetComponent<Image>().color = Color.cyan;
            ModulesForSale.SetActive(false);
            UpgradesForSale.SetActive(true);
        }
    }

    public void CheckIfYouCanBuy(double currency, Dictionary<string, double> currentCost, bool condition)
    {
        foreach (var pair in currentCost)
        {
            if (currency >= currentCost[pair.Key])
            {
                if (!condition)
                {
                    buyModuleButtons[pair.Key].GetComponent<ShopItem_ModuleButton>().CanBuy(true);
                }
                else
                    buyUpgradesButtons[pair.Key].GetComponent<ShopItem_ModuleButton>().CanBuy(true);
            }
            else
            {
                if (!condition)
                {
                    buyModuleButtons[pair.Key].GetComponent<ShopItem_ModuleButton>().CanBuy(false);
                }
                else
                    buyUpgradesButtons[pair.Key].GetComponent<ShopItem_ModuleButton>().CanBuy(false);
            }
        }
    }

    bool showShop = true;
    public AnimationClip moveLeft;
    public AnimationClip moveRight;

    public AnimationClip moveUiRight;
    public AnimationClip moveUiLeft;

    void AnimateShop(AnimationClip animationShop, AnimationClip animationInventory)
    {
        wholeShop.GetComponent<Animation>().clip = animationShop;
        inventoryContainer.GetComponent<Animation>().clip = animationInventory;
        wholeShop.GetComponent<Animation>().Play();
        inventoryContainer.GetComponent<Animation>().Play();
    }
    public void ShowHideShop()
    {
        showShop = !showShop;
        wholeShop.GetComponent<Animation>().Play();
        if (showShop)
        {
            AnimateShop(moveLeft, moveUiLeft);
            showHideShopButton.GetComponent<Image>().color = Color.cyan;
            showHideShopButton.transform.GetChild(0).GetComponent<Text>().color = Color.cyan;
            Camera.main.GetComponent<SmoothCamera2D>().OpenShop();
            
        }
        else
        {
            AnimateShop(moveRight, moveUiRight);
            showHideShopButton.GetComponent<Image>().color = Color.white;
            showHideShopButton.transform.GetChild(0).GetComponent<Text>().color = Color.white;
            Camera.main.GetComponent<SmoothCamera2D>().ClosedShop();
        }
    }

    float hptargetFill = 1;
    float shieldtargetFill = 1;
    float currentMaxHp = 1;
    void Update()
    {
        if (healthBarContent.fillAmount != hptargetFill)
        {
            healthBarContent.fillAmount = Mathf.Lerp(healthBarContent.fillAmount, hptargetFill, 0.15f);
        }

        if (shieldBarContent.fillAmount != shieldtargetFill)
        {
            shieldBarContent.fillAmount = Mathf.Lerp(shieldBarContent.fillAmount, shieldtargetFill, 0.15f);
        }
    }

    public void PlayerHealthBar(float currentHp, float maxHp)
    {
        hptargetFill = currentHp / maxHp;
        //  healthBarContent.fillAmount = currentHp / maxHp;
        healthBarText.text = currentHp.ToString() + "/" + maxHp.ToString();
    }

    public void SetCurrentMaxHp(float currentMax, float maxHp)
    {
        currentMaxHealth.fillAmount = currentMax / maxHp;
    }

    public void Shield(float currentShield, float maxShield)
    {
        shieldtargetFill = currentShield / maxShield;
        //  healthBarContent.fillAmount = currentHp / maxHp;
        shieldBarText.text = currentShield.ToString() + "/" + maxShield.ToString();
    }
    public void SetShiled()
    {
        shieldtargetFill = 1;
        shieldBarContent.fillAmount = 1;   
    }
}
