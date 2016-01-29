using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.UI;

public class LowLevel : MonoBehaviour {

	public InputField inputIP;
	public InputField inputPort;
	public InputField inputMessage;
	public Text errorField;
	public Text msgField;

	private int _port;
	private string _address;

	private int _unreliableID;
	private int _hostID;
	
	private int _connectionID;

	void Start() {

		_hostID = -1;
		inputIP.text = "192.168.1.66";
		inputPort.text = "8888";

	}

	public void CreateSocket() {
	
		if (_hostID > -1)
			return;

		if (inputIP.text.Length == 0 || inputPort.text.Length == 0) {

			errorField.text = "IP Address or Port Error";
			return;

		}

		_port = int.Parse (inputPort.text);
		_address = inputIP.text;

		//http://docs.unity3d.com/Manual/UNetUsingTransport.html
		NetworkTransport.Init ();
		ConnectionConfig config = new ConnectionConfig ();
		
		_unreliableID = config.AddChannel (QosType.Unreliable);	// http://blogs.unity3d.com/2014/06/11/all-about-the-unity-networking-transport-layer/
		
		int maxConnections = 10;
		
		HostTopology topology = new HostTopology (config, maxConnections);
		
		_hostID = NetworkTransport.AddHost (topology, _port);
		Debug.Log ("Socket Open.  _hostID: " + _hostID);

		if (_hostID > -1) {
			errorField.text = "Socket Connected!";
		}

	}
	

	public void Client() {

		CreateSocket ();

		byte error;
		_connectionID = NetworkTransport.Connect (_hostID, _address, _port, 0, out error);


//		if (error != NetworkError.Ok) {
//		
//			Debug.Log ("NetworkError");
//		
//		} else {
//
//			Debug.Log ("Connected to server. ConnectionID: " + _connectionID);
//
//		}


	}

	public void SendSocketMessage() {

	
		if (_hostID == -1) {
			errorField.text = "No Socket created!";
			return;
		}


		if (inputMessage.text.Length == 0) {
			errorField.text = "No message";
			return;
		}

		byte error;
		byte[] buffer = new byte[1024];
		Stream stream = new MemoryStream (buffer);

		BinaryFormatter formatter = new BinaryFormatter ();
		formatter.Serialize (stream, inputMessage.text);

		int bufferSize = 1024;

		NetworkTransport.Send (_hostID, _connectionID, _unreliableID, buffer, bufferSize, out error);

		Debug.Log ("LowLevel.SendSocketMessage:" + inputMessage.text);
		errorField.text = inputMessage.text + " is sent";
	}
	

	// Update is called once per frame
	void Update () {

		if (_hostID < 0)
			return;
	
		int recHostId;
		int connectionId;
		int channelId;
		byte[] recBuffer = new byte[1024];
		int bufferSize = 1024;
		int dataSize;
		byte error;

		NetworkEventType recData = NetworkTransport.Receive(out recHostId, out connectionId, out channelId, recBuffer, bufferSize, out dataSize, out error);

		switch (recData) {
		
			case NetworkEventType.Nothing:
				break;
			case NetworkEventType.ConnectEvent:
				Debug.Log("Incoming connection event received");
				break;
			case NetworkEventType.DataEvent:
				Stream stream = new MemoryStream(recBuffer);
				BinaryFormatter formatter = new BinaryFormatter();
				string message = formatter.Deserialize(stream) as string;

				msgField.text += "\n" + message;

				Debug.Log("Incoming message event received: " + message);
				break;
			case NetworkEventType.DisconnectEvent:
				Debug.Log("remote client event disconnected");
				break;


		}
	}

}
