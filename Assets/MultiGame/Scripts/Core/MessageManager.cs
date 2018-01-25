 using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MultiGame;

namespace MultiGame {

	public class MessageManager : MonoBehaviour {
		
		[Tooltip("Although you don't need to ue this in any scene, you can use it to store a list of messages if you like which can be sent with SendAll")]
		public static List<ManagedMessage> managedMessages = new List<ManagedMessage>();

		[System.Serializable]
		public class ManagedMessage {
			public GameObject target;
			public string message;
			public bool msgOverride = false;//"lock" message?
			public int messageIndex = 0;
			[HideInInspector]
			public string[] possibleMessages;
			public bool isDirty = true;
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

		[System.Serializable]
		public class TaggedMessage
		{
			ManagedMessage message;
			string targetTag;
		}

		public static void SendAll () {
			foreach (ManagedMessage msg in managedMessages) {
				Send(msg);
			}
		}

		public static void SendTo (ManagedMessage managedMessage, GameObject target) {
			if (managedMessage.message == "--none--")
				return;
			Send(new ManagedMessage(target, managedMessage.message, managedMessage.sendMessageType, managedMessage.parameter, managedMessage.parameterMode));
		}

		public static void Send (ManagedMessage managedMessage) {
			if (managedMessage.message == "--none--")
				return;
			if (managedMessage.target == null) {
				Debug.LogError("Message " + managedMessage.message + " does not have a target assigned in the inspector, and none can be inferred! Please assign a target manually.");
				return;
			}
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

		public static void UpdateMessageGUI (ref ManagedMessage _msg , GameObject _self) {
	//		if (!_msg.isDirty)
	//			return;
			if (_msg == null)
				return;

			List<Component> components = new List<Component>();
			if (_msg.target == null)
				components.AddRange(_self.GetComponentsInChildren(typeof(MonoBehaviour)));
			else
				components.AddRange(_msg.target.GetComponentsInChildren(typeof(MonoBehaviour)));
			List<string> possibleMessages = new List<string>();
//			List<string> allMessages = new List<string>();
			MethodInfo[] _methods;

			foreach (Component component in components) {
				if (component != null) {
					_methods = component.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance|BindingFlags.DeclaredOnly);
					if (!(_methods.Length <1)) {
					foreach(MethodInfo info in _methods ) {
							if (!info.Name.StartsWith("get") && !info.Name.StartsWith("Cmd") && !info.Name.StartsWith("CallCmd")) {
							if (info.GetParameters().Length <= 1) {
								if (info.GetParameters().Length > 0) {
									if(CheckIsValidParam(info.GetParameters()[0]))
										possibleMessages.Add(info.Name);
								}
								else
									possibleMessages.Add(info.Name);

							}

						}

	//				bool _successfullyAssignedParamType = false;
	//				if (info.GetParameters().Length < 1) {
	//					_msg.parameterMode = ManagedMessage.ParameterModeTypes.None;
	//					_successfullyAssignedParamType = true;
	//				}
	//				else {
	//					if (info.GetParameters()[0].GetType() == typeof(int)) {
	//						_msg.parameterMode = ManagedMessage.ParameterModeTypes.Integer;
	//						_successfullyAssignedParamType = true;
	//					}
	//					if (info.GetParameters()[0].GetType() == typeof(bool)) {
	//						_msg.parameterMode = ManagedMessage.ParameterModeTypes.Bool;
	//						_successfullyAssignedParamType = true;
	//					}
	//					if (info.GetParameters()[0].GetType() == typeof(string)) {
	//						_msg.parameterMode = ManagedMessage.ParameterModeTypes.String;
	//						_successfullyAssignedParamType = true;
	//					}
	//					if (info.GetParameters()[0].GetType() == typeof(float)) {
	//						_msg.parameterMode = ManagedMessage.ParameterModeTypes.FloatingPoint;
	//						_successfullyAssignedParamType = true;
	//					}
	//				}
	//				if (!_successfullyAssignedParamType) {
	//					_msg.parameterMode = ManagedMessage.ParameterModeTypes.None;
	//					_successfullyAssignedParamType = true;
	//				}
						}
					}
				}
			}
			
	//		Debug.Log("Rebuilt possible messages array");
			
			if (possibleMessages != null)
				_msg.possibleMessages = possibleMessages.ToArray();
			
			_msg.isDirty = false;
		}

		public static bool CheckIsValidParam (ParameterInfo _paramInfo) {
			bool _ret = false;

			if (_paramInfo.ParameterType == typeof(int))
				_ret = true;
			if (_paramInfo.ParameterType == typeof(bool))
				_ret = true;
			if (_paramInfo.ParameterType == typeof(string))
				_ret = true;
			if (_paramInfo.ParameterType == typeof(float))
				_ret = true;

			return _ret;
		}

	}
}
//Copyright 2014 William Hendrickson
