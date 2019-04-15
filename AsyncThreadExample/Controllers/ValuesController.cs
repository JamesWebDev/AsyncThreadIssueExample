using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using RestSharp;

namespace AsyncThreadExample.Controllers
{
    [System.Web.Http.RoutePrefix("api/test")]
    [System.Web.Http.AllowAnonymous]
    public class ValuesController : ApiController
    {
        

        [System.Web.Http.HttpPost]
        [System.Web.Http.AllowAnonymous]
        [System.Web.Http.Route("LogTheMessage")]
        public void LogTheMessage([FromBody] LogTestMessage messageToLog)
        {
            System.Diagnostics.Debug.WriteLine($"LogTheMessage: {messageToLog.Message} " + DateTime.Now.ToString("HH:mm:ss.fff tt"));
        }

        [HttpPost]
        [System.Web.Http.AllowAnonymous]
        [Route("MyAsyncIssueExample")]
        public async Task<IHttpActionResult> MyAsyncIssueExample(LogTestMessage data)
        {
            var process = new TestProcess();
            var result = await process.Main(data);            
            return Ok(result);
        }
    }

    public class TestProcess
    {
        public async Task<LogTestMessage> DoSomeNormalAsyncThing(LogTestMessage data)
        {
            System.Diagnostics.Debug.WriteLine("DoSomeNormalAsyncThing begin " + DateTime.Now.ToString("HH:mm:ss.fff tt"));
            var client = new RestClient("http://localhost:63894/api/test/LogTheMessage");
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(data), ParameterType.RequestBody);

            // easy async support
            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            Console.WriteLine("DoSomeNormalAsyncThing: client.ExecuteAsync, StatusCode=" + response.StatusCode);
            data.Result = true;
            System.Diagnostics.Debug.WriteLine("DoSomeNormalAsyncThing end " + DateTime.Now.ToString("HH:mm:ss.fff tt"));
            return data;
        }

        public async Task<LogTestMessage> DoSomeNonAwaitedAsyncThing(LogTestMessage data)
        {

            System.Diagnostics.Debug.WriteLine("DoSomeNonAwaitedAsyncThing begin " + DateTime.Now.ToString("HH:mm:ss.fff tt"));
            var nonAwaited = new LogTestMessage() { Message = "LogTheMessage for DoSomeNonAwaitedAsyncThing" };
            var client = new RestClient("http://localhost:63894/api/test/LogTheMessage");
            var request = new RestRequest(Method.POST);
            request.AddParameter("application/json", JsonConvert.SerializeObject(nonAwaited), ParameterType.RequestBody);

            // easy async support
            var cancellationTokenSource = new CancellationTokenSource();
            var response = await client.ExecuteTaskAsync(request, cancellationTokenSource.Token);
            Console.WriteLine("DoSomeNonAwaitedAsyncThing: client.ExecuteAsync, StatusCode=" + response.StatusCode);
            await Task.Delay(2000);

            nonAwaited.Result = true;
            System.Diagnostics.Debug.WriteLine("DoSomeNonAwaitedAsyncThing end " + DateTime.Now.ToString("HH:mm:ss.fff tt"));
            return nonAwaited;
        }
        public async Task<LogTestMessage> Main(LogTestMessage data)
        {
            DoSomeNonAwaitedAsyncThing(data);
            //Task.Run(async () => { DoSomeNonAwaitedAsyncThing(data); }); 
            var result = await DoSomeNormalAsyncThing(data);
            return result;
        }

    }



    public class LogTestMessage 
    {
        public string Message { get; set; }
        public bool? Result { get; set; }
    }
}
