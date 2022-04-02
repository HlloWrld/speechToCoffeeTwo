using System.Reflection.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Hosting.Internal;

namespace Dotnet_Final_Complete.Controllers
{
    // [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class SpeechController : ControllerBase
    {
        private readonly ILogger<SpeechController> _logger;

        public SpeechController(ILogger<SpeechController> logger)
        {
            _logger = logger;
        }   
        [HttpGet]
        public IEnumerable<Text> Get() 
        {
            return Enumerable.Range(1,1).Select(index => new Text {
                text = "data"
            }).ToArray();
        }
 
        [HttpPost, DisableRequestSizeLimit]
        public async Task<IActionResult>Post() 
        {
                try
                    {
                var formCollection = await Request.ReadFormAsync();
                var file = formCollection.Files.First();
                var folderName = Path.Combine("Resources", "Sounds");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                if (file.Length > 0)
                {
                    var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
                    var fullPath = Path.Combine(pathToSave, fileName);
                    var dbPath = Path.Combine(folderName, fileName);
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }        
            var text = await Listen(fullPath);
            Console.WriteLine(text);
    
                    return Ok(new { dbPath });
                    
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex}");
            }

        }
        
        public string OutputSpeechRecognitionResult(SpeechRecognitionResult speechRecognitionResult)
        {
            switch (speechRecognitionResult.Reason)
            {
                case ResultReason.RecognizedSpeech:
                    return speechRecognitionResult.Text;
                case ResultReason.NoMatch:
                    return "CANCELED: Speech could not be recognized.";
                case ResultReason.Canceled:
                    var cancellation = CancellationDetails.FromResult(speechRecognitionResult);

                    if (cancellation.Reason == CancellationReason.Error)
                    {
                        return $"CANCELED: ErrorCode={cancellation.ErrorCode}, ErrorDetails={cancellation.ErrorDetails}, Double check the speech resource key and region.";
                    } else {
                        return $"CANCELED: {cancellation.Reason.ToString()}";
                    }
                default: return "CANCELED: No speech assessed";
            }
        }
        static string YourSubscriptionKey = "1ebf10647ec64e5d84887b9331702759";
        static string YourServiceRegion = "eastus";
        public async Task<string> Listen(string filePath)
        {
            // set config vars
            var speechConfig = SpeechConfig.FromSubscription(YourSubscriptionKey, YourServiceRegion);        
            speechConfig.SpeechRecognitionLanguage = "en-US";

            // load the audio file to be listened to by the speech recognizer
            using var audioConfig = AudioConfig.FromWavFileInput(filePath);
            using var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig);

            // convert speech to text
            var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();
            var test = OutputSpeechRecognitionResult(speechRecognitionResult);
            Console.WriteLine(test);
            return test;
        }
    }
}