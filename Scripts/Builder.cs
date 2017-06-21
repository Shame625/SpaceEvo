using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Builder : MonoBehaviour {
	public UIManager UI;
	GameManager GM;
	public GameObject legalSpotGO;
	GameObject legalSpotContainer;

	HashSet<Vector2> takenPositions = new HashSet<Vector2>();
	HashSet<Vector2> legalPositions = new HashSet<Vector2>();

	GameObject selectedModule;

	//Building Mode Handles, ugly as fucking dirt, wtf!
	public bool buildMode = false;
    //UH OH!
    public static bool isBuilding = false;

	int currentModule = 0;

	//FUCKING MOBILE HANDLE
	GameObject currentObj = null;
	GameObject previousObj = null;
	public GameObject deletionOverlay;
	GameObject delOverlay;
	bool deleteMode = false;

	Vector3 position;
	Quaternion rotation;
	float currentPlayerSpeed;

	Player player;


    //Nice look
    public AnimationClip moduleBeingDeleted;


	void Awake()
	{
		GM = GetComponent<GameManager> ();
		UI = GameObject.FindGameObjectWithTag ("UI").GetComponent<UIManager> ();
	}

	void Start()
	{
		//Generate inventory
		UI.GenerateInventoryUI(GM.moduleContainer.gameModuleForSale, GM.player.ownedModules);
        selectedModule = GM.moduleContainer.gameModuleForSaleList[0];
		//Generate Shop
	}

	void Update()
	{
		if (buildMode) {

			if (Input.GetMouseButtonDown (0)) {
				if (selectedModule == null) 
				{
                    selectedModule = GM.moduleContainer.gameModuleForSaleList[0];
				}
				Click ();
			}

			if (Input.GetMouseButtonDown (1)) {
				DeleteModule ();
			}

			if (Input.GetKeyDown (KeyCode.Q)) {
				currentModule--;
				if (currentModule < 0) {
                    currentModule = GM.moduleContainer.gameModuleForSaleList.Count- 1;
				}

                selectedModule = GM.moduleContainer.gameModuleForSaleList[currentModule];
			}

			if (Input.GetKeyDown (KeyCode.E)) {
				currentModule++;
                if (currentModule > GM.moduleContainer.gameModuleForSaleList.Count - 1)
                {
					currentModule = 0;
				}

                selectedModule = GM.moduleContainer.gameModuleForSaleList[currentModule];
			}
            if(Input.GetKeyDown(KeyCode.R))
            {
            ResetShip();
            }
		}

	}


    void RegenerateShip()
    {
        player.RegenerateDestroyedModule();
        player.GetModules();

        foreach (GameObject go in GM.player.playerModules)
        {
            foreach (Transform child in go.transform)
            {
                if (child.name == "ImportantCollider")
                {
                    if (child.gameObject.GetComponent<Rigidbody2D>() == null)
                    {
                        child.gameObject.AddComponent<Rigidbody2D>();
                        child.GetComponent<Rigidbody2D>().isKinematic = true;
                        child.GetComponent<Rigidbody2D>().gravityScale = 0;
                    }
                }
            }
            //Destroy(go.GetComponent<Rigidbody2D>());
            if (go.name == "gunModule")
            {
                go.transform.GetChild(1).transform.gameObject.SetActive(false);
            }
            if (go.name == "playergunModule")
            {
                go.transform.GetChild(0).transform.rotation = Quaternion.identity;
            }
            if (!go.activeSelf)
            {
                go.SetActive(true);
            }
        }
        player.ChangeOccured("shieldModule");
        player.ChangeOccured("hpModule");
        player.CalculateMovement();
    }

	public void Build()
	{//need check if in combat
        if (!GM.doingDmg_Hazard)
        {
            if (!buildMode)
            {
                buildMode = true;
                GM.BackgroundSwitch(true);
                player = GM.player;
                legalPositions.Clear();
                takenPositions.Clear();
                GM.SetCameraTarget(null);
                GenerateCopyOfPlayer();
                GM.Player.transform.position = new Vector3(0, 0, 0);
                GM.Everything.gameObject.SetActive(false);
                //make sure shop doesnt crap out
                GM.CalculateNewCosts();
                UI.SelectInventoryObject(selectedModule.name);  
                isBuilding = buildMode;
                RegenerateShip();
                CalculateIllegalSpots(player.playerModules);
                CalculateLegalSpots(player.playerModules);
                SpawnLegalSpots();
            }
            else
                CloseBuild();

            UI.OpenBuildingMode(buildMode);
        }
	}

	void GenerateCopyOfPlayer()
	{
		currentPlayerSpeed = GM.Player.GetComponent<PlayerMovement> ().speed;
		position = GM.Player.transform.position;
		rotation = GM.Player.transform.rotation;
		GM.Player.transform.position = Vector3.zero;
		GM.Player.transform.localScale = Vector3.one;   
		GM.Player.transform.rotation = Quaternion.identity;
		GM.Player.GetComponent<PlayerMovement> ().enabled = false;
		Destroy(GM.Player.GetComponent("Rigidbody2D"));
		Camera.main.GetComponent<SmoothCamera2D>().SetCameraBuildingMode();
        foreach (GameObject go in player.playerModules)
        {
            if (go.name == "engineModule")
                go.transform.GetChild(0).GetChild(0).GetComponent<TrailRenderer>().enabled = false;
        }
	}
	public void CloseBuild()
	{

		if (buildMode) {
            GM.BackgroundSwitch(false);
			GM.Everything.gameObject.SetActive (true);
			legalPositions.Clear ();
			takenPositions.Clear ();
			DestroyLegalSpots ();
			if (deleteMode) {
				DeleteMode ();
			}
			if (GM.Player != null) {


				GM.Player.AddComponent<Rigidbody2D>();
				GM.Player.GetComponent<Rigidbody2D> ().gravityScale = 0;
				GM.Player.transform.position = position;
				GM.Player.transform.rotation = rotation;
				GM.Player.GetComponent<PlayerMovement> ().speed = currentPlayerSpeed;
                foreach (GameObject go in player.playerModules)
                {
                    foreach (Transform child in go.transform)
                    {
                        if (child.name == "ImportantCollider")
                        {
                            Destroy(child.gameObject.GetComponent<Rigidbody2D>());
                        }
                    }
                    if(go.name == "gunModule")
                    go.transform.GetChild(1).gameObject.SetActive(true);
                    else if (go.name == "engineModule")
                    go.transform.GetChild(0).GetChild(0).GetComponent<TrailRenderer>().enabled = true;
                }
				GM.Player.GetComponent<PlayerMovement> ().enabled = true;

				GM.SetCameraTarget (GM.Player.transform);
                Camera.main.GetComponent<SmoothCamera2D>().SetCameraGameMode();

				//UI :(
				UI.LegalSpotsButton (true);
			}
		}
        GetComponent<SaveLoad>().SavePlayer();
		buildMode = false;
        isBuilding = buildMode;
	}
	void SpawnLegalSpots()
	{
		if (legalSpotContainer == null) {
			legalSpotContainer = new GameObject ("Legal spot container");
		}
		foreach (Vector2 vec in legalPositions) 
		{
			GameObject go = (GameObject)Instantiate (legalSpotGO, vec, Quaternion.identity);
			go.transform.SetParent (legalSpotContainer.transform);

		}
	}
	public void HideLegalSpots()
	{
		legalSpotContainer.SetActive (!legalSpotContainer.activeSelf);
		UI.LegalSpotsButton (legalSpotContainer.activeSelf);
	}

	public void DeleteMode()
	{
		deleteMode = !deleteMode;
		UI.DeleteionButton (deleteMode);
		if (delOverlay) {
			Destroy (delOverlay);
			currentObj = null;
			previousObj = null;
		}
	}

	void DestroyLegalSpots()
	{
		Destroy (legalSpotContainer);
		legalSpotContainer = null;
	}

	void CalculateIllegalSpots(List<GameObject> modules)
	{
		takenPositions.Clear();
		foreach (GameObject go in modules) {
			if (go.name == "engineModule") {
				takenPositions.Add (go.transform.position);
				takenPositions.Add (new Vector2 (go.transform.position.x, go.transform.position.y - 1));
			} else
				takenPositions.Add (go.transform.position);
		}
	}

	void CalculateLegalSpots(List<GameObject> modules)
	{
		legalPositions.Clear ();
		foreach (GameObject go in modules) 
		{
			//6 cases ugly
			Vector3 spot1 = new Vector3(GameManager.offsetX,GameManager.offsetY,0);
			Vector3 spot2 = new Vector3(GameManager.offsetX,-GameManager.offsetY,0);
			Vector2[] newSpots = {go.transform.position + Vector3.up, go.transform.position - Vector3.up, go.transform.position + spot1, go.transform.position - spot1,go.transform.position - spot2, go.transform.position +spot2};
			for (int i = 0; i <= 5; i++) {

				//Fuck float
				/*
				bool taken = false;
				foreach (Vector2 vec in takenPositions) 
				{
					if (newSpots [i].x <= vec.x + 0.1f && newSpots [i].x >= vec.x - 0.1f && newSpots [i].y <= vec.y + 0.1f && newSpots [i].y >= vec.y - 0.1f) 
					{
						taken = true;
						break;
					}
				}
				if(!taken)
				{
					legalPositions.Add (newSpots [i]);
				}*/
                Vector2 size = player.GetComponent<PlayerStats>().coreModuleSize;
                if (newSpots[i].x <= size.x && newSpots[i].x >= -size.x && newSpots[i].y <= size.y && newSpots[i].y >= -size.y)
                {
					if (!takenPositions.Contains (newSpots [i])) {
						legalPositions.Add (newSpots [i]);
					}
				}
			}
		}
	}


	void Click()
	{
		RaycastHit2D hit = Physics2D.Raycast (Camera.main.ScreenToWorldPoint (Input.mousePosition), Vector2.zero);
		if (!UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject ()){
		if (hit.collider != null) {
			if (ItemCheck (selectedModule.name)) {
				if (hit.transform.tag == "legalSpot") {
					if (selectedModule.name == "engineModule") {
						if (!takenPositions.Contains (hit.transform.position - Vector3.up)) 
                        {
							AddNewModule (selectedModule, hit.transform.position);
                            player.GetModules();
                            RegenerateBuildingSpots();
						}
					} else {
						AddNewModule (selectedModule, hit.transform.position);
                        player.GetModules();
                        RegenerateBuildingSpots();
					}
                    
				}
				
			}
			//Mobile deleting
			if (hit.transform.parent.tag == "Modules" && hit.transform.parent.name != "coreModule" && deleteMode) {

				currentObj = hit.transform.parent.gameObject;
                UI.LegalSpotsButton(true);
				if (delOverlay == null) {
					delOverlay = (GameObject)Instantiate (deletionOverlay, hit.transform.parent.position, Quaternion.identity);
				} else {
					delOverlay.transform.position = currentObj.transform.position;
				}
				if (currentObj == previousObj) {
					DeleteObj (currentObj);
					Destroy (delOverlay);
				}
				previousObj = currentObj;
			}
		}
		if (hit.collider == null && deleteMode) {
			if (delOverlay != null) {
				Destroy (delOverlay);
				currentObj = null;
				previousObj = null;
			}
		}
	}
	}

	void AddNewModule(GameObject module, Vector3 hit)
	{
		GameObject go = (GameObject)Instantiate (selectedModule, hit, selectedModule.transform.rotation);

		go.name = selectedModule.transform.name;
        if (go.name == "gunModule")
        {
            go.transform.GetChild(1).gameObject.SetActive(false);
        }
        go.GetComponent<Module>().SetColor(GameManager.LevelColors[player.ownedUpgrades[go.name]]);
		go.transform.SetParent (GM.Player.transform);
		player.playerModules.Add (go);
        player.ChangeOccured(selectedModule.transform.name);
		AddOrDeduceModuleFromPlayerInventory (module.name, false);
	}

	void DeleteModule()
	{
		RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

		if (hit.collider != null) {
			if (hit.collider.tag == "Modules" && hit.transform.parent.name!="coreModule" && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject()) 
			{
                DeleteObj(hit.transform.parent.gameObject);
			}
		}
	}

	void DeleteObj(GameObject hit)
	{

		GameObject Top = null;
		GameObject Down= null;
		GameObject Left= null;
		GameObject Right= null;
		GameObject LeftDown = null;
		GameObject RightDown = null;

		foreach (GameObject go in player.playerModules) 
		{
			if (go.transform.position.x == hit.transform.position.x && go.transform.position.y == hit.transform.position.y + 1) 
			{
				Top = go;
			}
			else if (go.transform.position.x == hit.transform.position.x && go.transform.position.y == hit.transform.position.y - 1) 
			{
				Down = go;
			}
			else if (go.transform.position.x == hit.transform.position.x -GameManager.offsetX && go.transform.position.y == hit.transform.position.y +GameManager.offsetY) 
			{
				Left = go;
			}
			else if (go.transform.position.x == hit.transform.position.x + GameManager.offsetX && go.transform.position.y == hit.transform.position.y + GameManager.offsetY) 
			{
				Right = go;
			}
			else if (go.transform.position.x == hit.transform.position.x + GameManager.offsetX && go.transform.position.y == hit.transform.position.y - GameManager.offsetY) 
			{
				RightDown = go;
			}
			else if (go.transform.position.x == hit.transform.position.x - GameManager.offsetX && go.transform.position.y == hit.transform.position.y - GameManager.offsetY) 
			{
				LeftDown = go;
			}
		}

		GameObject tempObj = hit.transform.gameObject;
		RemoveModuleFromPlayer (hit.transform.gameObject);

		player.CheckIfConnectedToCore ();
		bool legalDeletion = true;
		if (Top != null) {
			if (Top.name != "coreModule") {
                if (Top.GetComponentInParent<Module>().hasCore == false)
                {
					legalDeletion = false;
				}
			}
		}
		if (Down != null) {
			if (Down.name != "coreModule") {
                if (Down.GetComponentInParent<Module>().hasCore == false)
                {
					if (legalDeletion == true) {
						legalDeletion = false;
					}
				}
			}
		}
		if (Left != null) {
			if (Left.name != "coreModule") {
                if (Left.GetComponentInParent<Module>().hasCore == false)
                {
					if (legalDeletion == true) {
						legalDeletion = false;
					}
				}
			}
		}
		if (Right != null) {
			if (Right.name != "coreModule") {
                if (Right.GetComponentInParent<Module>().hasCore == false)
                {
					if (legalDeletion == true) {
						legalDeletion = false;
					}
				}
			}
		}

		if (RightDown != null) {
			if (RightDown.name != "coreModule") {
                if (RightDown.GetComponentInParent<Module>().hasCore == false)
                {
					if (legalDeletion == true) {
						legalDeletion = false;
					}
				}
			}
		}
		if (LeftDown != null) {
			if (LeftDown.name != "coreModule") {
                if (LeftDown.GetComponentInParent<Module>().hasCore == false)
                {
					if (legalDeletion == true) {
						legalDeletion = false;
					}
				}
			}
		}

		if (legalDeletion) {
			AddOrDeduceModuleFromPlayerInventory (hit.transform.gameObject.name, true);
			RemoveModuleFromPlayer (hit.transform.gameObject);
            tempObj.GetComponent<Animation>().clip = moduleBeingDeleted;
            tempObj.GetComponent<Animation>().Play();
            Destroy(tempObj, moduleBeingDeleted.length);
            RegenerateBuildingSpots();
            player.GetModules();
		} else 
		{
			player.playerModules.Add (tempObj);
		}
		player.alreadyLooked.Clear ();
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Modules")) 
        {
            if (go != null)
            {
                if (go.name != "coreModule")
                {
                    go.GetComponentInParent<Module>().hasCore = false;
                }
            }
		}
		
        player.CalculateMovement();
	}

	void RemoveModuleFromPlayer(GameObject go)
	{
		player.playerModules.Remove (go);
        player.ChangeOccured(go.name);
	}

    void ResetShip()
    {
        List<GameObject> temp = new List<GameObject>();
        foreach (GameObject go in player.playerModules)
        { 
            if(go.name!="coreModule")
            {
                AddOrDeduceModuleFromPlayerInventory(go.name, true);

                temp.Add(go);
                go.GetComponent<Animation>().clip = moduleBeingDeleted;
                go.GetComponent<Animation>().Play();
                Destroy(go, moduleBeingDeleted.length);
            }
        }
        foreach (GameObject go in temp)
        {
            RemoveModuleFromPlayer(go);
        }
        //Recalculate movement
        player.CalculateMovement();

        RegenerateBuildingSpots();
        player.GetModules();
    }

	void AddOrDeduceModuleFromPlayerInventory(string Name, bool condition)
	{
		if (condition)
			player.ownedModules [Name]++;
		else 
			player.ownedModules [Name]--;

		UI.RefreshInventoryItem (Name, player.ownedModules [Name]);
	}

	bool ItemCheck(string Name)
	{
		if (player.ownedModules [Name] > 0)
			return true;
		else
			return false;
	}

	public void RegenerateBuildingSpots()
	{
		DestroyLegalSpots ();
		buildMode = true;
		CalculateIllegalSpots (player.playerModules);
		CalculateLegalSpots (player.playerModules);
		SpawnLegalSpots ();
	}


	public void SelectItem(string Name)
	{
        selectedModule = GM.moduleContainer.gameModuleForSale[Name];
        UI.SelectInventoryObject(Name);
	}

}
