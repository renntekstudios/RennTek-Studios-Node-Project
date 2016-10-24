using UnityEngine;
using System.Collections;
using UnityStandardAssets.Characters.ThirdPerson;
public class NetworkPlayerControler : MonoBehaviour {

	PlayerName playerName;
	public string	id;



	public void Start()
	{
		playerName = GetComponentInChildren<PlayerName> () as PlayerName;
		playerName.setName(this.name);




	}


	public void Move(Vector3 position) 
	{
		
		transform.position = position;
	}

	public void Rotate(Quaternion rotation) 
	{
		 
		transform.rotation= rotation;

	}




}
