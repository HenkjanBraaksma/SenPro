Before cloning the project from Github, make sure you have the following things:
Visual Studio 15.3 or above
Android SDKs for 7.0 (others optional, but I always built for 7.0)
.Net Core 2.0 (I think all computers have this already)

- Go to the Start menu > Android SDK Tools > SDK Manager and install the Android SDKs for 7.0
- Update Visual Studio 2017 to at least version 15.3
These two take a long time to install, so I suggest installing them at the same time.
- Install the latest .Net Core SDK: https://www.microsoft.com/net/download/windows
For the admin login, ask Arto/Seppo


Most weird errors (missing reference to .net 2.0, IntelliSense not working because all classes are considered "Miscelaneous Item"s, etc) can be fixed by updating Visual Studio.

If problems persist (namely, the miscelaneous item thing), try this.

- Open a new Xamarin Android Blank App project
- Right click the project in the Solution Explorer and go to Properties. In Application, set the target framework to Android 7.0.
- In the Android Manifest, set the minimum android version to 7.0, and the target to 7.0
- Next, under Required Permissions, tick the following:
ACCESS_COARSE_LOCATION
ACCESS_FINE_LOCATION
BLUETOOTH
BLUETOOTH_ADMIN

- Install the following NuGet packages:
Newtonsoft.Json
OxyPlot.Xamarin.Android
Plugin.BLE
Xamarin.Forms

- Add all classes of the original android app (right click your project, select add existing item, and go to the classes).
These classes are:
GraphActivity.cs
PermissionActivity.cs
ReconnectActivity.cs
RestService.cs
SaMi.cs
SenProReading.cs
StartUpActivity.cs

- In one of the classes, rightclick the namespace, select rename, and rename it to the name of your project. It should change the namespace for all classes.

- Next, add the layouts. These should be added to "Resources\layout" and can be found in the same folder in the original project. There are three of them.

- Now, add Savonia.Measurements.Client to your solution as a second project.

- Finally, in the app project, right click References, select Add References, and find Savonia.Measurements.Clients in Projects. Tick the box and press OK.

- Rebuild the solution, and everything should work.


KEYS
SK507-2017305DCC94F130F11-01	Write	Write key for mobile app to write data to SaMi
SK507-201745D7825AF7E4411-01	Read	Common read key
SK507-201715F3E4EF4BFC5411-01	Modify	Modify key for Henkjan 