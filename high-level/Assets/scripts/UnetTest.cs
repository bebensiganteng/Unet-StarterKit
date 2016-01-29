using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class UnetTest : MonoBehaviour {

	public GameObject netmanager;
	private NetworkManagerHUD m_nethud;
	
	/// <summary>
	/// host is bottom, client is front
	/// </summary>
	public int m_isHost;
	
	private bool m_sceneEnd;
	private bool m_sceneLoad;
	
	public	UILabel m_statusLabel;
	public  UILabel m_hostLabel;
	public  UILabel m_countLabel;
	public  UILabel m_hitLabel;
	
	// Score variable
	private int m_count;
	private int m_groundHit;
	private int m_loadCounter;
	
	#region EVENT
	
	public delegate void SceneIsEnded ();
	public delegate void SceneIsLoaded ();
	public delegate void ScoreCount();
	public delegate void GroundHit ();
	
	public static event SceneIsEnded OnSceneEnded;
	public static event SceneIsLoaded OnSceneLoaded;
	public static event ScoreCount OnScoreCount;
	public static event GroundHit OnGroundHit;
	
	#endregion
	
	
	// Use this for initialization
	void Start () {
		
		m_isHost = -1;
		
		NetManager m_nm = netmanager.GetComponent<NetManager> ();
		m_nm.IPAddress = "192.168.1.25";
		m_nm.Port = 7777;
		
		m_nethud = netmanager.GetComponent<NetworkManagerHUD> ();
		
		
	}
	
	#region LABELS
	private void setStatusLabel(string label) {
		
		m_statusLabel.text = label;
		
	}
	
	
	#endregion
	
	// Update is called once per frame
	void Update () {
		
		if (Input.GetKeyDown (KeyCode.P)) m_nethud.showGUI = !m_nethud.showGUI;
		
		
	}
	
	//3.fadein開始
	private void fadeIn(){
		
		m_loadCounter = 0;
		
		setStatusLabel ("Fade In");
		Invoke ("fadeInEnd", 0.1f);
		
	}
	
	//4.fadeIn完了
	private void fadeInEnd(){
		
		float loadTime = (m_isHost > 0) ? 10.0f : 1.0f;
		
		setStatusLabel ("Scene Loading.." + loadTime);
		Invoke ("sceneLoadComplete", loadTime);
	}
	
	//6.シーンロード終わり
	private void sceneLoadComplete(){
		
		
		Debug.Log ("sceneLoadComplete:" + m_hostLabel.text);
		
		if (OnSceneLoaded != null) {
			
			setStatusLabel ("Scene Loaded");
			OnSceneLoaded ();
		}
	}
	
	//9.同時にフェードアウト
	public void fadeOut(){
		
		m_loadCounter = 0;
		setStatusLabel ("fadeout");
		
	}
	
	
	
	#region FROM UNET
	
	// Indicate HOST/CLIENT
	public void setIsHost(int num) {
		
		ResetScore ();
		
		m_isHost = num;
		m_hostLabel.text = (m_isHost > 0) ? "CLIENT" : "HOST";
		setStatusLabel ("scene playing");
		
	}
	
	
	// Bottom
	public void FromUnetEnded(){
		
		// Set scene Ended in Client
		setStatusLabel ("Scene Ended");
		//test
		fadeIn ();
	} 
	
	
	//8.Host,Clientのシーンロードが終わったという通知がサーバーからくる
	public void FromUnetLoaded(){
		
		m_loadCounter++;
		
		if (m_loadCounter == 2) {
			
			
			fadeOut ();
			
		}
		
	}
	
	public void FromUnetCount(){
		
		m_count++;
		m_countLabel.text = m_count.ToString ();
		
	}
	
	public void FromUnetHit() {
		
		m_groundHit++;
		m_hitLabel.text = m_groundHit.ToString ();
	}
	
	#endregion
	
	#region FROM BUTTONS
	
	public void AddScore() {
		
		if (m_isHost != -1) {
			
			if(OnScoreCount != null) OnScoreCount();
		}
		
	}
	
	public void AddHit() {
		
		if (m_isHost != -1) {
			
			if(OnGroundHit != null) OnGroundHit();
		}
		
	}
	
	public void EndScene() {
		
		
		if (m_isHost == 1) { //Front
			
			if (OnSceneEnded != null) {
				
				// Set scene Ended in host
				setStatusLabel ("Scene Ended");
				OnSceneEnded ();
				
				//FadeIn Front
				fadeIn();
			}
		}
	}
	
	public void ResetScore() {
		
		m_count 			= 0;	// count score;
		m_loadCounter 		= 0;	// count scene;
		m_groundHit 		= 0; // ground hit;
		
		m_hitLabel.text 	= m_groundHit.ToString ();
		m_countLabel.text 	= m_count.ToString ();
		setStatusLabel ("Scene Playing");
	}
	
	#endregion
}
