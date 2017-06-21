using UnityEngine;
using System.Collections;

public class InventoryButton : MonoBehaviour {
	
	string Name;
	Builder builder;

	void Awake()
	{
		builder = GameObject.FindGameObjectWithTag ("GameManager").GetComponent<Builder> ();
	}
	public void SetProperties(string name)
	{
		gameObject.name = name;
		Name = name;
	}

	public void Click()
	{
		builder.SelectItem (Name);
	}
}
