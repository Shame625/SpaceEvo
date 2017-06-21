using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShopItem_ModuleButton : MonoBehaviour {

    string Name;

    Image image;
    Text nameText;
    Text costText;

    Text levelText;

    Text currentOwnedItems;

    Shop shop;
    bool IsUpgrade = false;

    bool maxLevelReached = false;

    void Awake()
    {
        shop = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Shop>();

        image = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
        currentOwnedItems = transform.GetChild(0).GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetComponent<Text>();
        nameText = transform.GetChild(0).GetChild(1).GetComponent<Text>();
        costText = transform.GetChild(0).GetChild(2).GetComponent<Text>();
      
    }


    public void SetProperties(string name, string displayName, double cost, Sprite picture, int totalOwned, bool isUpgrade)
    {
        IsUpgrade = isUpgrade;
        gameObject.name = name;
        Name = name;
        image.sprite = picture;
        nameText.text = "Name: " + displayName;
        costText.text = "Cost: " + cost.ToString();
        currentOwnedItems.text = totalOwned.ToString();
 
        if (isUpgrade)
        {
            levelText = transform.GetChild(0).GetChild(3).GetComponent<Text>();
            levelText.text = "Level: " + (totalOwned+1).ToString();

            if (totalOwned>=10)
                MaxUpgradeLevelReached();
        }

    }


    public void SetCost(double cost)
    {
        costText.text = "Cost: " + cost.ToString();
    }


    public void SetCurrentOwnedItems(int num)
    {
        currentOwnedItems.text = num.ToString();
        if (IsUpgrade)
        {
            levelText.text = "Level: " + (num + 1).ToString();
        }
    }

    public void BuyModule()
    {
        shop.BuyItem(Name);
    }

    public void BuyUpgrade()
    {
        shop.BuyUpgrade(Name);
    }

    public void CanBuy(bool canBuy)
    {
        if (canBuy && maxLevelReached==false)
        {
            GetComponent<Image>().color = Color.green;
        }
        else if (!canBuy && maxLevelReached == false)
            GetComponent<Image>().color = Color.red;
    }

    public void MaxUpgradeLevelReached()
    {
        GetComponent<Image>().color = Color.yellow;
        GetComponent<Button>().enabled = false;
        costText.text = "Cost: Maximum level!";
        levelText.text = "Level: Maximum level!";
        maxLevelReached = true;
    }
}
