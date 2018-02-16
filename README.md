# Http429.SimulateThrottling

This Fiddler extension tampers HTTP response messages to HTTP 429 Too Many Requests errors.

It is written to only process \*.sharepoint.com requests, and those end with \/_vti_bin/client.svc

Throttling driven by a concurrent queue. 10 requests must occur within 5 seconds for the throttling to kick in.

*You can change any of the above hardcoded values. *

It is hardcoded for Fiddler version 4.6.3.44034, but you can change that to your version (see: https://github.com/OneBitSoftware/Http429.SimulateThrottling/blob/master/Http429.SimulateThrottling/ThrottleSimulatorExtension.cs#L2)

It requires a reference to Fiddler.exe, which is set to "C:\Program Files (x86)\Fiddler2\Fiddler.exe" in the .csproj file, so point that to your Fiddler installation .exe.

Feel free to raise any issues or submit PR's.
