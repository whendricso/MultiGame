using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MessageManager : MonoBehaviour {

	public static List<ManagedMessage> managedMessages = new List<ManagedMessage>();

	[System.Serializable]
	public class ManagedMessage {
		public GameObject target;
		public string message;
		public enum SendMessageTypes {Send, Broadcast};
		public SendMessageTypes sendMessageType = SendMessageTypes.Send;
		public string parameter;
		public enum ParameterModeTypes {None, Integer, FloatingPoint, Bool, String};
		public ParameterModeTypes parameterMode = ParameterModeTypes.None;

		public ManagedMessage (GameObject _target, string _message) {
			target = _target;
			message = _message;
		}

		public ManagedMessage (GameObject _target, string _message, SendMessageTypes _sendMessageType, string _parameter, ParameterModeTypes _parameterModeType) {
			target = _target;
			message = _message;
			sendMessageType = _sendMessageType;
			parameter = _parameter;
			parameterMode = _parameterModeType;
		}
	}

	public static void SendAll () {
		foreach (ManagedMessage msg in managedMessages) {
			Send(msg);
		}
	}

	public static void SendTo (ManagedMessage managedMessage, GameObject target) {
		Send(new ManagedMessage(target, managedMessage.message));
	}

	public static void Send (ManagedMessage managedMessage) {
//		if (string.IsNullOrEmpty( managedMessage.message))
//			managedMessage.message = "Activate";
		switch (managedMessage.parameterMode) {
		case ManagedMessage.ParameterModeTypes.None:
			if ( managedMessage.sendMessageType == ManagedMessage.SendMessageTypes.Broadcast)
				managedMessage.target.BroadcastMessage(managedMessage.message, SendMessageOptions.DontRequireReceiver);
			else
				managedMessage.target.SendMessage(managedMessage.message, SendMessageOptions.DontRequireReceiver);
			break;
		case ManagedMessage.ParameterModeTypes.Bool:
			if (managedMessage.sendMessageType == ManagedMessage.SendMessageTypes.Broadcast)
				managedMessage.target.BroadcastMessage(managedMessage.message, System.Convert.ToBoolean( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			else
				managedMessage.target.SendMessage(managedMessage.message, System.Convert.ToBoolean( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			break;
		case ManagedMessage.ParameterModeTypes.Integer:
			if (managedMessage.sendMessageType == ManagedMessage.SendMessageTypes.Broadcast)
				managedMessage.target.BroadcastMessage(managedMessage.message, System.Convert.ToInt32( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			else
				managedMessage.target.SendMessage(managedMessage.message, System.Convert.ToInt32( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			break;
		case ManagedMessage.ParameterModeTypes.FloatingPoint:
			if (managedMessage.sendMessageType == ManagedMessage.SendMessageTypes.Broadcast)
				managedMessage.target.BroadcastMessage(managedMessage.message, System.Convert.ToSingle( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			else
				managedMessage.target.SendMessage(managedMessage.message, System.Convert.ToSingle( managedMessage.parameter), SendMessageOptions.DontRequireReceiver);
			break;
		case ManagedMessage.ParameterModeTypes.String:
			if (managedMessage.sendMessageType == ManagedMessage.SendMessageTypes.Broadcast)
				managedMessage.target.BroadcastMessage(managedMessage.message, managedMessage.parameter, SendMessageOptions.DontRequireReceiver);
			else
				managedMessage.target.SendMessage(managedMessage.message, managedMessage.parameter, SendMessageOptions.DontRequireReceiver);
			break;
		}
	}
	
}
//Copyright 2014 William Hendrickson
