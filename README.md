
# MultiGame
MultiGame makes game development in Unity fast and simple(r) for artists, programmers, and designers!

It works with all other scripts, tools, add-ons. Seriously, MultiGame is built to be robust and will not break your existing systems. It contains a plethora of tools to place objects and set up scenes. It also has programmer tools to send events, write smart Inspectors, and provide help to the user.

## Component Documentation:
https://terpri.org/mgdoc/

## Tutorial video:
https://youtu.be/8ptbRaj_qK8

### Copious Code Base
The most common components to any game project can be found inside! I'm so confident that you'll like MultiGame, I'm making half of it open source so that you can feel confident including it in your own projects.

MultiGame has been developed and tested on multiple projects since January 2013. It's flexible ManagedMessage system provides a powerful event framework that works automatically with *any* script, even those not included in MultiGame.

Thank you to everyone who has contributed along the way.

# Instructions
![alt text](https://preview.ibb.co/dxHNZb/Open_Multi_Game_Toolbar.png "Click Window MultiGame Rapid Dev Tool")
1. Just open the .unitypackage file and it will open the Unity importer in your project.
2. Click Window -> MultiGame -> Rapid Dev Tool
3. Dock the toolbar on the left or right side of the screen
4. Select an object and click a button to quickly add functionality.

# Editor Tools
You will find several editor tools:
- Rapid Dev Toolbar, contains the most common tools and components
- Prefab Painter, to paint objects onto other objects quickly and easily with many useful settings
- Splines, to place objects procedurally or move them along a procedural path

# Components
You can find many additional components in the Add Component -> MultiGame menu found at the top of the screen, or on the Inspector.

![alt text](https://preview.ibb.co/egTUVG/Multi_Game_Add_Component.png "Add Component MultiGame")

# Contribution
If you'd like to contribute to MultiGame, please open an Issue and submit a pull request.

Submissions should come in the form of a new component, and should follow conventions from similar files. For example, when writing new camera controllers, first review other camera controllers and other components it's likely to interact with.

## Messages and Delegates ##
MultiGame uses `MessageManager.ManagedMessage` to communicate with and call functionality on other components. This system allows MultiGame to interface with third-party systems without breaking functionality and without tight coupling, and it provides several advantages over delegates at a small performance cost.

Because of this convention, submissions relying on delegates for communication are likely to be rejected. If you need a delegate for some other purpose, such as global registry to a singleton, that usage is expressly permitted, but should not be used as an alternative to ManagedMessage unless implementation constraints require it, and the delegate is not directly exposed to the user (which can cause confusion)

## MessageManager Guidelines ##
- Messages are declared in the following format:  `public MessageManager.ManagedMessage message;`
- `OnValidate` must be used to update the message GUI as follows: `MessageManager.UpdateMessageGUI(ref message, gameObject);` otherwise, the list of available messages will not be updated, leading to user frustration
- Messages can be sent via the `MessageManager` as follows: `MessageManager.Send(message);`
- Please avoid calling `Send` every frame from a large number of objects, because it is several times more expensive than a standard method call.

**Example:**
```csharp
[Tooltip("What should we send when clicked?")]
public MessageManager.ManagedMessage message;
void OnValidate () {
  MessageManager.UpdateMessageGUI(ref message, gameObject);
}
void OnMouseUpAsButton() {
  MessageManager.Send(message);
}
```

## Help and Documentation Requirements ##
- All new component or system submissions must contain, at a minimum, in-editor documentation including a brief explanation of the component. These are created with `HelpInfo` public fields in the following format: `public HelpInfo help = new HelpInfo(string text, [string videoLink]);` where `videoLink` is optional.
- Public methods that are intended to be called using the `MessageManager` system must have `MessageHelp` declared in the following format: `public MessageHelp methodHelp = new MessageHelp(string _messageName, string _text,[ int _argumentType, string _argumentHelp]); public void Method () { [...]`
- Public fields must have a `Tooltip`, `RequiredFieldAttribute`, or other Editor hinting to allow users to quickly understand components and refresh prior knowledge.

## Automation policy ##
When contributing changes, please limit machine learning use to code review and debugging purposes, and don't submit new features that have been "vibe-coded".

Use of documentation generators or other non-machine-learning automation systems is unrestricted.

MultiGame's Component Documentation at https://terpri.org/mgdoc is generated by Robin Templeton.
