var MenuSkin : GUISkin;

 

var toggleTxt : boolean;

var toolbarInt : int = 0;

var toolbarStrings : String[] = ["Toolbar1", "Toolbar2", "Toolbar3"];

var selGridInt : int = 0;

var selStrings : String[] = ["Grid 1", "Grid 2", "Grid 3", "Grid 4"];

var hSliderValue : float = 0.0;

var hSbarValue : float;

 

function OnGUI() {

GUI.skin = MenuSkin;

    GUI.BeginGroup(new Rect(Screen.width/2-150,Screen.height/2-150,300,300));

        GUI.Box(Rect(0,0,300,300),"This is the title of a box");

            GUI.Button(Rect(0,25,100,20),"I am a button");

                 GUI.Label (Rect (0, 50, 100, 20), "I'm a Label!");

                     toggleTxt = GUI.Toggle(Rect(0, 75, 200, 30), toggleTxt, "I am a Toggle button");

                          toolbarInt = GUI.Toolbar (Rect (0, 110, 250, 25), toolbarInt, toolbarStrings);

                             selGridInt = GUI.SelectionGrid (Rect (0, 160, 200, 40), selGridInt, selStrings, 2);

                                 hSliderValue = GUI.HorizontalSlider (Rect (0, 210, 100, 30), hSliderValue, 0.0, 1.0);

                                    hSbarValue = GUI.HorizontalScrollbar (Rect (0, 230, 100, 30), hSbarValue, 1.0, 0.0, 10.0);

                                        GUI.EndGroup ();

}