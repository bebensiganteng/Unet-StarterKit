using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class NetManager : NetworkManager {

	private string m_ipaddress;
	private int m_port;
	
	private void SetPort() {
		
		NetworkManager.singleton.networkPort = m_port;
		
	}
	
	private void SetIPAddress(){
		
		NetworkManager.singleton.networkAddress = m_ipaddress;
		
	}
	
	public void StartGame() {
		
		if (NetworkManager.singleton.isNetworkActive) 
			return;
		
		SetPort ();
		NetworkManager.singleton.StartHost ();
		
		Debug.Log ("NetManager.StartGame");
		
	}
	
	public void JoinGame() {
		
		if (NetworkManager.singleton.isNetworkActive) 
			return;
		
		SetPort ();
		SetIPAddress ();
		NetworkManager.singleton.StartClient ();
		
		Debug.Log ("NetManager.JoinGame");
	}
	
	void CheckStatus(string msg) {
		
		Debug.Log ("NetManager.CheckStatus------->" + msg);
		Debug.Log ("isNetworkActive------->" + NetworkManager.singleton.isNetworkActive);
		//Debug.Log ("IsClientConnected------->" + NetworkManager.singleton.IsClientConnected());
		Debug.Log ("-------------------------------------------------------");
	}
	
	public override void OnClientError (NetworkConnection conn, int errorCode)
	{
		base.OnClientError (conn, errorCode);
		
		Debug.LogError ("NetManager.OnClientError: " + errorCode);
	}
	
	public override void OnServerError (NetworkConnection conn, int errorCode)
	{
		base.OnServerError (conn, errorCode);
		
		Debug.Log ("NetManager.OnServerError: " + errorCode);
	}
	
	public override void OnServerDisconnect (NetworkConnection conn)
	{
		base.OnServerDisconnect (conn);
		
		// Try to reconnect;
		Invoke ("JoinGame", 10.0f);
		
		Debug.Log ("NetManager.OnServerDisconnect");
	}
	
	public override void OnServerConnect (NetworkConnection conn)
	{
		base.OnServerConnect (conn);
		
		Debug.Log ("NetManager.OnServerConnect [" + conn.connectionId + "]");
	}

	public override void OnClientConnect (NetworkConnection conn)
	{
		base.OnClientConnect (conn);
		
		Debug.Log ("NetManager.OnClientConnect: [" + conn.connectionId + "]");
	}

	public string IPAddress { set { m_ipaddress = value; } }
	public int Port { set { m_port = value; } }



}
