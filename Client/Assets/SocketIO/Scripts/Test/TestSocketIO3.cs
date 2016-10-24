#region License
/*
 * TestSocketIO.cs
 *
 * The MIT License
 *
 * Copyright (c) 2014 Fabio Panettieri
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;
using SocketIO;
using UnityStandardAssets.CrossPlatformInput;

public class TestSocketIO3 : MonoBehaviour
{
	private SocketIOComponent socket;

	public GameObject[] playerPrefab1;
	public GameObject[] playerPrefab2;
	public GameObject myPlayer;
	public GameObject[] players;
	InputField mainInputField;
	GameObject canvas;
	int index;
	public bool onLogin = false;

	public void Start() 
	{
		canvas = GameObject.Find ("HDUCanvas");

		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();

		socket.On("open", TestOpen);
		socket.On("boop", TestBoop);
		socket.On ("LOGIN_SUCESS", OnLoginSucess);
		socket.On("SPAW_PLAYER",OnInstantiatePlayer);
		socket.On("UPDATE_MOVE", onUserMove);
		socket.On("UPDATE_ROTATE", onUserRotate);
		socket.On("USER_DISCONNECTED", OnUserDisconnected );
        socket.On("UPDATE_JUMP", onUserMove);

		Debug.Log("Game is start");
		socket.On("error", TestError);
		socket.On("close", TestClose);

		mainInputField = FindObjectOfType (typeof(InputField)) as InputField;
		StartCoroutine("BeepBoop");

		  
	}

	private IEnumerator BeepBoop()
	{
		// wait 1 seconds and continue
		yield return new WaitForSeconds(1);
		
		socket.Emit("beep");
		//socket.Emit("USER_CONNECT");
		// wait 3 seconds and continue
		yield return new WaitForSeconds(3);
		
		socket.Emit("beep");
		
		// wait 2 seconds and continue
		yield return new WaitForSeconds(2);
		
		socket.Emit("beep");
		
		// wait ONE FRAME and continue
		yield return null;
		
		socket.Emit("beep");
		socket.Emit("beep");
	}


	public void TestOpen(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Open received: " + e.name + " " + e.data);
	}
	
	//primeira funcao implementada para testes
	public void TestBoop(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Boop received: " + e.name + " " + e.data);

		if (e.data == null) { return; }

		Debug.Log(
			"#####################################################" +
			"THIS: " + e.data.GetField("this").str +
			"#####################################################"
		);
	}
	
	public void TestError(SocketIOEvent e)
	{
		Debug.Log("[SocketIO] Error received: " + e.name + " " + e.data);
	}

	public void TestClose(SocketIOEvent e)
	{	
		Debug.Log("[SocketIO] Close received: " + e.name + " " + e.data);
	}

	//funcão acionada quando o botão de login de usuario e pressionado
	public void OnClickPlayBtn ()
	{

		if(mainInputField.text != ""  ){//verifica se o usuario preencheu com o nome

			canvas.SetActive(false);
			onLogin = true;
			Dictionary<string, string> data = new Dictionary<string, string>();//pacote JSON
			data["name"] = mainInputField.text;//preenche o campo nome 
			Vector3 position  = new Vector3(0,3,0);
			Vector3 rotation = new Vector3 (0, 0, 0);
			data["position"] = position.x+","+position.y+","+position.z;//preenche com o campo  position default
			data["rotation"] = rotation.x+","+rotation.y+","+rotation.z;
			index = Random.Range (0, playerPrefab1.Length);
			if (index == 0) {
				data["tipe"] = "carlos";
			}
			if (index == 1) {
				data["tipe"] = "fabio";
			}
			if (index == 2) {
				data["tipe"] = "iris";
			}
			socket.Emit("LOGIN", new JSONObject(data));//envia os dados do new Player para a funcao socket.on('USER_CONNECT') no servidor
			//obs: de uma olhada agora nesta funcao no servidor.

		}
		else{
			mainInputField.text = "Please enter your name again ";
		}


	}

	void OnLoginSucess(SocketIOEvent _myPlayer)
	{
		if (JsonToString (_myPlayer.data.GetField ("tipe").ToString (), "\"") == "carlos") {
			myPlayer = GameObject.Instantiate (playerPrefab1 [0], JsonToVector3 (JsonToString (_myPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
			myPlayer.name = JsonToString (_myPlayer.data.GetField ("name").ToString (), "\"");
			PlayerController playerCom = myPlayer.GetComponent<PlayerController> () as PlayerController;
			PlayerName playerName = myPlayer.GetComponentInChildren<PlayerName> () as PlayerName;
			playerName.setName (myPlayer.name);
			playerCom.id = JsonToString (_myPlayer.data.GetField ("id").ToString (), "\"");
		}
		if (JsonToString (_myPlayer.data.GetField ("tipe").ToString (), "\"") == "fabio") {
			myPlayer = GameObject.Instantiate (playerPrefab1 [1], JsonToVector3 (JsonToString (_myPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
			myPlayer.name = JsonToString (_myPlayer.data.GetField ("name").ToString (), "\"");
			PlayerController playerCom = myPlayer.GetComponent<PlayerController> () as PlayerController;
			PlayerName playerName = myPlayer.GetComponentInChildren<PlayerName> () as PlayerName;
			playerName.setName (myPlayer.name);
			playerCom.id = JsonToString (_myPlayer.data.GetField ("id").ToString (), "\"");
		}
		if (JsonToString (_myPlayer.data.GetField ("tipe").ToString (), "\"") == "iris") {
			myPlayer = GameObject.Instantiate (playerPrefab1 [2], JsonToVector3 (JsonToString (_myPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
			myPlayer.name = JsonToString (_myPlayer.data.GetField ("name").ToString (), "\"");
			PlayerController playerCom = myPlayer.GetComponent<PlayerController> () as PlayerController;
			PlayerName playerName = myPlayer.GetComponentInChildren<PlayerName> () as PlayerName;
			playerName.setName (myPlayer.name);
			playerCom.id = JsonToString (_myPlayer.data.GetField ("id").ToString (), "\"");
		}

	}

	//atualiza a movimentacao de um outro cliente para o cliente associado a  este script
 	void onUserMove (SocketIOEvent obj)
	{
		GameObject networkPlayer = GameObject.Find(  JsonToString( obj.data.GetField("name").ToString(), "\"") ) as GameObject;//localiza o cliente que se moveu
		NetworkPlayerControler playerCom = networkPlayer.GetComponent<NetworkPlayerControler> () as NetworkPlayerControler;
		playerCom.Move (JsonToVector3( JsonToString(obj.data.GetField("position").ToString(), "\"")));
		//networkPlayer.transform.position =  JsonToVector3( JsonToString(obj.data.GetField("position").ToString(), "\"") );// atualiza a posicao

	}

	//atualiza a movimentacao de um  cliente para o cliente associado a  este script
	void onUserRotate (SocketIOEvent obj)
	{
		GameObject networkPlayer = GameObject.Find(  JsonToString( obj.data.GetField("name").ToString(), "\"") ) as GameObject;//localiza o cliente que se moveu
		Vector4 rot = JsonToVector4( JsonToString(obj.data.GetField("rotation").ToString(), "\"") );// atualiza a posicao
		NetworkPlayerControler playerCom = networkPlayer.GetComponent<NetworkPlayerControler> () as NetworkPlayerControler;
		playerCom.Rotate (new Quaternion (
			rot.x, rot.y, rot.z, rot.w)
		);
		/*
		networkPlayer.transform.rotation =new Quaternion(
			rot.x, rot.y, rot.z, rot.w);
		*/
		
		
	}


	
	
	//funcao para destruir o cliente que se desconectou nessa instancia de cliente
	void OnUserDisconnected (SocketIOEvent obj)
	{

		Destroy( GameObject.Find( JsonToString(obj.data.GetField("name").ToString(), "\"")));

	}
	

	//instancia  players no cliente associado a este script
	void OnInstantiatePlayer(SocketIOEvent _newPlayer)
	{
	  
		if (onLogin) {
			if (JsonToString (_newPlayer.data.GetField ("tipe").ToString (), "\"") == "carlos") {
				GameObject newPlayer = GameObject.Instantiate (playerPrefab2[0], JsonToVector3 (JsonToString (_newPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
				newPlayer.name = JsonToString (_newPlayer.data.GetField ("name").ToString (), "\"");
				NetworkPlayerControler playerCom = newPlayer.GetComponent<NetworkPlayerControler> () as NetworkPlayerControler;
				playerCom.id = JsonToString (_newPlayer.data.GetField ("id").ToString (), "\"");
			}

			if (JsonToString (_newPlayer.data.GetField ("tipe").ToString (), "\"") == "fabio") {
				GameObject newPlayer = GameObject.Instantiate (playerPrefab2[1], JsonToVector3 (JsonToString (_newPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
				newPlayer.name = JsonToString (_newPlayer.data.GetField ("name").ToString (), "\"");
				NetworkPlayerControler playerCom = newPlayer.GetComponent<NetworkPlayerControler> () as NetworkPlayerControler;
				playerCom.id = JsonToString (_newPlayer.data.GetField ("id").ToString (), "\"");
			}

			if (JsonToString (_newPlayer.data.GetField ("tipe").ToString (), "\"") == "iris") {
				GameObject newPlayer = GameObject.Instantiate (playerPrefab2[2], JsonToVector3 (JsonToString (_newPlayer.data.GetField ("position").ToString (), "\"")), Quaternion.identity) as GameObject;
				newPlayer.name = JsonToString (_newPlayer.data.GetField ("name").ToString (), "\"");
				NetworkPlayerControler playerCom = newPlayer.GetComponent<NetworkPlayerControler> () as NetworkPlayerControler;
				playerCom.id = JsonToString (_newPlayer.data.GetField ("id").ToString (), "\"");
			}
		}
	 
	}
	

	//metodo responsavel por transmitir ao servidor o movimento do player associado a este cliente
	public void EmitPosition(Vector3 _pos)
	{
	  Dictionary<string, string> data = new Dictionary<string, string>();
		Vector3 position = new Vector3( _pos.x,_pos.y,_pos.z );
		data["position"] = position.x+","+position.y+","+position.z;
		
		socket.Emit("MOVE", new JSONObject(data));//faz uma chamada e envia o pacote JSON 'data' ao servidor para a funcao 'MOVE'
	}
	
	
	//metodo responsavel por transmitir ao servidor o movimento do player associado a este cliente
	public void EmitRotation(Quaternion _rot)
	{
		Dictionary<string, string> data = new Dictionary<string, string>();
		data["rotation"] = _rot.x+","+_rot.y+","+_rot.z+","+_rot.w;

		socket.Emit("ROTATE", new JSONObject(data));//faz uma chamada e envia o pacote JSON 'data' ao servidor para a funcao 'MOVE'
	}

    public void EmitJump(Vector3 _jump)
    {
        Dictionary<string, string> data = new Dictionary<string, string>();
        Vector3 position = new Vector3(_jump.x, _jump.y, _jump.z);
        data["position"] = position.x + "," + position.y + "," + position.z;

        socket.Emit("MOVE", new JSONObject(data));//faz uma chamada e envia o pacote JSON 'data' ao servidor para a funcao 'MOVE'
    }


	string  JsonToString( string target, string s){

		string[] newString = Regex.Split(target,s);

		return newString[1];

	}

	Vector3 JsonToVector3(string target ){

		Vector3 newVector;
		string[] newString = Regex.Split(target,",");
		newVector = new Vector3( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]));

		return newVector;

	}

	Vector4 JsonToVector4(string target ){

		Vector4 newVector;
		string[] newString = Regex.Split(target,",");
		newVector = new Vector4( float.Parse(newString[0]), float.Parse(newString[1]), float.Parse(newString[2]),float.Parse(newString[3]));

		return newVector;

	}
	
	
}
