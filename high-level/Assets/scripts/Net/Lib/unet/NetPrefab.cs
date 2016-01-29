using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[NetworkSettings(channel=0,sendInterval=0.0f)]
public class NetPrefab : NetworkBehaviour {

	private UnetTest m_unetTest;

	/**
	 * Just for expediency
	 */
	private void setSceneManager() {
	
		if (m_unetTest == null) m_unetTest = GameObject.Find ("Main Camera").GetComponent<UnetTest> (); 
	}

	// Use this for initialization
	void Start () {
	
		setSceneManager ();

	}
	
	void OnDisable() {

		UnetTest.OnSceneEnded -= OnSceneEnded;
		UnetTest.OnSceneLoaded -= OnSceneLoaded;
		UnetTest.OnScoreCount -= OnScoreCount;
		UnetTest.OnGroundHit -= OnGroundHit;
	}

	public override void PreStartClient ()
	{
		base.PreStartClient ();

		UnetTest.OnSceneEnded += OnSceneEnded;
		UnetTest.OnSceneLoaded += OnSceneLoaded;
		UnetTest.OnScoreCount += OnScoreCount;
		UnetTest.OnGroundHit += OnGroundHit;

	}

	public override void OnStartLocalPlayer ()
	{
		base.OnStartLocalPlayer ();

		setSceneManager ();

		m_unetTest.setIsHost (
			(isServer) ? 0 : 1
		);

	}

	// SCENE LOADED
	void OnSceneLoaded() {
	
		Debug.Log ("NetPrefab.OnSceneLoader:" + isServer + isLocalPlayer);

		if(isLocalPlayer) CmdSceneLoaded ();
//		CmdSceneLoaded ();
	
	}

	[Command]
	void CmdSceneLoaded() {


		
		RpcSceneLoaded ();
	
	}
	
	[ClientRpc]
	void RpcSceneLoaded() {

		m_unetTest.FromUnetLoaded ();

	}

	// SCENE ENDED
	void OnSceneEnded() {

		if (!isServer && isLocalPlayer)
			CmdSceneEnded ();
	}

	[Command]
	void CmdSceneEnded() {

		RpcSceneEnded ();
	}
	
	[ClientRpc]
	void RpcSceneEnded() {

		if (isServer)	// host
			m_unetTest.FromUnetEnded ();

	}

	// SCORE COUNT
	void OnScoreCount() {
	
		// if(bottom) 
		if (isServer && isLocalPlayer)
			CmdScoreCount ();
	}

	[Command]
	void CmdScoreCount() {
		
		RpcScoreCount ();
	}
	
	[ClientRpc]
	void RpcScoreCount() {

		if (!isServer)
			m_unetTest.FromUnetCount ();
		
	}

	// GROUND HIT
	void OnGroundHit() {

		if (isServer && isLocalPlayer)
			CmdHit ();
	}
	
	[Command]
	void CmdHit() {
		
		RpcHit ();
	}
	
	[ClientRpc]
	void RpcHit() {
		
		if (!isServer)
			m_unetTest.FromUnetHit ();
		
	}

	// INTEGRATED
	[Command]
	void CmdIntegrated(int timecode, int playernum) {
	
		RpcIntegrated (timecode, playernum);
	}

	[ClientRpc]
	void RpcIntegrated(int timecode, int playernum) {
	
		if (!isLocalPlayer) {
			Debug.Log ("NetPrefab.RpcIntegrated:" + timecode + " " + playernum);
		}
	}



}
