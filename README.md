# Http429.SimulateThrottling

This Fiddler extension tampers HTTP response messages to HTTP 429 Too Many Requests errors.

It is written to only process \*.sharepoint.com requests, and those end with /_vti_bin/client.svc

Throttling driven by a concurrent queue. 10 requests must occur within 5 seconds for the throttling to kick in.

You can change any of the above hardcoded values. 
