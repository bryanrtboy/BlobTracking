For these components to work you need the Flux Timeline editor http://u3d.as/content/nuno-afonso/flux.
HowTo:
1.Create an UniOSCConnection 
2.Open the Flux timeline editor and the Flux inspector. (Window/Flux/)
3. Create a Sequence in the Flux editor.
4. Drag the UniOSCConnection from the Hierachy into the Flux timeline.
5.Click on the little '+' sign at the new  created timeline entry
6. Goto 'OSC' and choose one of the event types:

Send OSC: 
Sends a single OSC message. if you don't use the 'is Trigger' option you can additionally specify a send interval.
There are two default datatypes appended to the OSCMessage (string & float)

Tween Float: 
Sends a single OSC message that has the tweening value as data attached (float).
As this eventtype continuously sends OSC messages you have an optional 'Use Interval' option where you can specify the sending interval. (Otherwise your framerate determines the interval )




