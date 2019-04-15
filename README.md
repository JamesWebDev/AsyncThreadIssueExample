# Example of Async Thread Issue
## non-Awaited task threads stop executing when api controller returns.
Clone the repo. 
Open AsyncThreadExample.sln
Look at Controllers/ValuesController.cs

note line 73: System.Diagnostics.Debug.WriteLine("DoSomeNonAwaitedAsyncThing end " + DateTime.Now.ToString("HH:mm:ss.fff tt"));

Start the api.

run
```powershell  
Invoke-WebRequest -Uri "http://localhost:63894/api/test/MyAsyncIssueExample" -Body "{ Message: 'Hello World' }" -Method Post 
```
The output will look something like 
```
DoSomeNonAwaitedAsyncThing begin 09:07:18.606 AM
DoSomeNormalAsyncThing begin 09:07:18.688 AM
LogTheMessage: LogTheMessage for DoSomeNonAwaitedAsyncThing 09:07:18.702 AM
LogTheMessage: Hello World 09:07:18.702 AM
DoSomeNormalAsyncThing end 09:07:18.741 AM
```
Notice ```DoSomeNormalAsyncThing end 09:07:20.741 AM``` is missing from the output

Try reducing the ``` await Task.Delay(2000); ``` to ```await Task.Delay(1); ```
```
DoSomeNonAwaitedAsyncThing begin 09:41:41.243 AM
DoSomeNormalAsyncThing begin 09:41:41.244 AM
LogTheMessage: LogTheMessage for DoSomeNonAwaitedAsyncThing 09:41:41.246 AM
LogTheMessage:  09:41:41.246 AM
DoSomeNormalAsyncThing end 09:41:41.248 AM
```
Notice output includes **DoSomeNormalAsyncThing end** this time.

## Notice web.config app settings added
  <appSettings>
    <add key="aspnet:UseTaskFriendlySynchronizationContext" value="true"/>
    <add key="aspnet:AllowAsyncDuringSyncStages" value="true"/>

## Workarounds
If you make the **DoSomeNonAwaitedAsyncThing** an Async Void the thread runs to completion.
If you wrap the call to **DoSomeNonAwaitedAsyncThing** in Task.Run the thread runs to completion

