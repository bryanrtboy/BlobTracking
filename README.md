# BlobTracking
A project to tweak and improve the Unity implementation of [TSPS](http://www.tsps.cc/), a cross-platform Toolkit for Sensing People in Spaces, and to make it easier to set up in a performance environment. NOTE: TSPS must be running as a separate application, with it's OSC port set up to send messages to Unity via OSC.Net.

![gif](https://github.com/bryanrtboy/BlobTracking/blob/master/preview.gif)

A typical scenario for this would be to track people moving on top of a floor projection. To do so, it is often required to zoom in and out the tracking grid, re-size it's aspect ratio and nudge the tracking grid up, down, left or right. This project includes all of that, along with the UI to make it happen.  The UI can then be hidden. The next time the application is opened, it will remember the settings that were used previously.

Todo: Implement a generic method to read from OSC in order to support apps like Isadora.
