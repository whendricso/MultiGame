using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ExitGames.Client.Photon.Chat;
using MultiGame;

public class PhotonChatHandler : PhotonModule , IChatClientListener {

	[RequiredField("The default username for new players, send 'AssignUsername' to change the username.")]
	public string userName;
	public enum Regions{US, EU, Asia};
	public Regions region = Regions.US;
	public Text chatTextArea;
	public string appID = "";
	public string appVersion = "";
	[Range(0.1f, 100f)][Tooltip("How long do we wait before automatically refreshing the chat?")]
	public float chatTickTime = 1f;
	private float chatCounter = 1f;
	[Tooltip("Which connection protocol should we use for this game? If you don't know, choose Udp.")]
	ExitGames.Client.Photon.ConnectionProtocol connectionProtocol = ExitGames.Client.Photon.ConnectionProtocol.Udp;
	[System.NonSerialized]
	ChatClient client;
	ExitGames.Client.Photon.Chat.AuthenticationValues authValues = new ExitGames.Client.Photon.Chat.AuthenticationValues();

	[System.NonSerialized]
	public string messagesText = "";

	public bool consoleLogMessages = false;
	public bool debug = false;

	void Start () {
		client = new ChatClient(this,connectionProtocol);
		chatCounter = chatTickTime;
	}

	void Update () {
		chatCounter -= Time.deltaTime;
		if (chatCounter < 0f)
			return;
		RefreshChat();
		chatCounter = chatTickTime;
	}

	public MultiModule.MessageHelp refreshChatHelp = new MultiModule.MessageHelp("RefreshChat","Updates the chat information with all new messages");
	public void RefreshChat () {
		
		if (client != null)
			client.Service();

	}

	public MultiModule.MessageHelp assignUsernameHelp = new MultiModule.MessageHelp("AssignUsername","Changes the username for this chat participant",4,"The new username");
	public void AssignUsername (string _userName) {
		userName = _userName;
	}

	public void Connect () {
		authValues.UserId = userName;
		authValues.AuthType = ExitGames.Client.Photon.Chat.CustomAuthenticationType.None;
		if ( client.Connect(appID,appVersion,authValues))
			Debug.Log("Chat connection success!");
		else
			Debug.LogError("Chat connection failure!");
	}

	private string ResolveRegion () {
		string _ret = "US";

		switch (region) {
		case Regions.US:
			_ret = "US";
			break;
		case Regions.EU:
			_ret = "EU";
			break;
		case Regions.Asia:
			_ret = "ASIA";
			break;
		}

		return _ret;
	}

	public void OnConnected() {
		Debug.Log("Connected to chat successfully");
		client.Subscribe(new string[] { "channelNameHere" }); //subscribe to chat channel once connected to server
	}

	public void OnDisconnected() {
		Debug.Log("Disconnected from chat, reason: " + client.DisconnectedCause.ToString());
	}

	public void OnGetMessages( string channelName, string[] senders, object[] messages )
	{
		for ( int i = 0; i < senders.Length; i++ )
		{
			messagesText += string.Format("{0}={1}, \n", senders[i], messages[i].ToString());
		}

		if (consoleLogMessages)
			ConsoleLogMsgs( channelName, senders, messages);
	}

	/// <summary>
	/// Consoles the log msgs if enabled in the inspector.
	/// </summary>
	/// <param name="channelName">Channel name.</param>
	/// <param name="senders">Senders.</param>
	/// <param name="messages">Messages.</param>
	private void ConsoleLogMsgs (string channelName, string[] senders, object[] messages) {
		string msgs = "";

		Debug.Log(msgs);
	}

	public void OnStatusUpdate (string user, int status, bool gotMessage, object message) {
		
	}

	public void OnSubscribed (string[] channels, bool[] results) {

	}

	public void OnUnsubscribed (string[] channels) {

	}

	public void DebugReturn (ExitGames.Client.Photon.DebugLevel debugLevel, string debugString) {
		Debug.Log(debugString + "\n" + "Debug level: " + debugLevel.ToString());
	}

	public void OnChatStateChange (ChatState chatState) {
		if (debug)
			Debug.Log("Chat State:\n" + chatState.ToString());
	}

	public void OnPrivateMessage (string sender, object message, string channelName) {
		if (consoleLogMessages)
			Debug.Log(channelName + ") " + sender + ": " + message);
	}
}
