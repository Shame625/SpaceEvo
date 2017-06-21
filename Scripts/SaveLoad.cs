using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBuildingBlocks
{
	public string Name;
	public Vector2 Position;
	public PlayerBuildingBlocks(string name, Vector2 position)
	{
		Name = name;
		Position = position;
	}
}

public class PlayerOwnedModulesAndUpgrades
{
	public string Name;
	public int ownedModules;
	public int ownedUpgrades;
	public PlayerOwnedModulesAndUpgrades(string name, int ownedmodules, int ownedupgrades)
	{
		Name = name;
		ownedModules = ownedmodules;
		ownedUpgrades = ownedupgrades;
	}
}

public class SaveLoad : MonoBehaviour {

	GameManager GM;
	List<PlayerOwnedModulesAndUpgrades> playerOwnedItems = new List<PlayerOwnedModulesAndUpgrades> ();
	List<PlayerBuildingBlocks> playerModules = new List<PlayerBuildingBlocks>();



	void Awake()
	{
		GM = GetComponent<GameManager> ();
	}


	//Saving process
	string playerModulesData = "";
	string playerOwnedData = "";

	public void SavePlayer()
	{
		playerModules.Clear ();
		playerOwnedItems.Clear ();
		playerModulesData = "";
		playerOwnedData = "";

		foreach (var pair in GM.player.ownedModules) 
		{
			playerOwnedData += pair.Key + ":" + pair.Value.ToString () + "|" + GM.player.ownedUpgrades [pair.Key].ToString ()+",";

			GM.moduleContainer.totalModulesAndUpgrades [pair.Key].Reset ();
			GM.moduleContainer.totalModulesAndUpgrades [pair.Key].IncreaseValues (pair.Value, GM.player.ownedUpgrades [pair.Key]);
		}

		foreach (GameObject go in GM.player.playerModules) 
		{
                string position = go.transform.localPosition.x.ToString() + "|" + go.transform.localPosition.y.ToString();
                playerModulesData += go.name + ":" + position + ",";
                GM.moduleContainer.totalModulesAndUpgrades[go.name].IncreaseModuleCount(1);
		}

		playerModulesData = playerModulesData.Remove (playerModulesData.Length - 1);
		playerOwnedData = playerOwnedData.Remove (playerOwnedData.Length - 1);

		PlayerPrefs.SetString("PlayerModules", playerModulesData);
		PlayerPrefs.SetString ("PlayerOwnedItems", playerOwnedData);
	}

	public void LoadPlayer()
	{
		playerModules.Clear ();
		playerOwnedItems.Clear ();
		//Check if save exsists
		if (PlayerPrefs.GetString ("PlayerModules") != "") {
			//Load method
			//If there is exsisting data, ignore Generate ship, and instead load all the saved location of player prefabs, including their names, aslo
			//save upgrade levels, and other stuff, when loading level, it is important to count all the modules owned, inlcuding the upgrades due the
			//formula that will be used for pricing stuff, awesome!

			//Grab data from file
			playerOwnedData = PlayerPrefs.GetString ("PlayerOwnedItems");
			playerModulesData = PlayerPrefs.GetString ("PlayerModules");

			string[] playerOwnedItemsConstructionData = playerOwnedData.Split (',');
			foreach (string str in playerOwnedItemsConstructionData) {
				string[] temp = str.Split (':');
				string name = temp [0];

				string[] tempItems = temp [1].Split ('|');
				int temp_ownedModules = int.Parse (tempItems [0]);
				int temp_ownedUpgrades = int.Parse (tempItems [1]);

				playerOwnedItems.Add (new PlayerOwnedModulesAndUpgrades (name, temp_ownedModules, temp_ownedUpgrades));
			}

			string[] playerModuleConstructedData = playerModulesData.Split (',');
			foreach (string str in playerModuleConstructedData) {
				string[] temp = str.Split (':');
				string name = temp [0];

				string[] tempPos = temp [1].Split ('|');
				Vector2 position = new Vector2 (float.Parse (tempPos [0]), float.Parse (tempPos [1]));
				playerModules.Add (new PlayerBuildingBlocks (name, position));
			}
			

			GM.SpawnPlayer (playerOwnedItems, playerModules);
		}
	}

	public void SaveOtherData()
	{
		//Player currencies
		//Player currentMaxLevel "int"

	}
}
