using Microsoft.Cognitive.LUIS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Media.Capture;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechRecognition;
using Windows.Media.SpeechSynthesis;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UwpDroneController
{
    public class LUISIntentStatus
    {
        public SpeechService service;
        public bool Success { get; set; }
        public bool handled { get; set; } = false;
        public string TextResponse { get; set; }
        public string SpeechRespose { get; set; }
    }

    public class SpeechService
    {
        private SpeechRecognizer _speechRecognizer;
        private SpeechRecognizer _continousSpeechRecognizer;
        private SpeechSynthesizer _speechSynthesizer;

        private static SpeechService _instance;
        public static SpeechService Instance => _instance ?? (_instance = new SpeechService());

        private bool _speechInit = false;
        private TextBox _textBox = null;
        private IntentRouter _router;
        private MediaPlayer _player;

        private SpeechService()
        {

        }

        public async Task Initialize(TextBox textBox)
        {
            if (_speechInit == true || !(await CheckForMicrophonePermission()))
                return;

            _textBox = textBox;

            _continousSpeechRecognizer = new SpeechRecognizer();
            _continousSpeechRecognizer.Constraints.Add(new SpeechRecognitionListConstraint(new List<String>() { "Hey Dorthy" }, "start"));
            var result = await _continousSpeechRecognizer.CompileConstraintsAsync();

            if (result.Status != SpeechRecognitionResultStatus.Success)
            {
                return;
            }

            _speechRecognizer = new SpeechRecognizer();
            result = await _speechRecognizer.CompileConstraintsAsync();
            _speechRecognizer.HypothesisGenerated += _speechRecognizer_HypothesisGenerated;

            if (result.Status != SpeechRecognitionResultStatus.Success)
            {
                return;
            }

            _continousSpeechRecognizer.ContinuousRecognitionSession.ResultGenerated += ContinuousRecognitionSession_ResultGenerated;
            await _continousSpeechRecognizer.ContinuousRecognitionSession.StartAsync(SpeechContinuousRecognitionMode.Default);

            _speechInit = true;
        }

        private void ContinuousRecognitionSession_ResultGenerated(SpeechContinuousRecognitionSession sender, SpeechContinuousRecognitionResultGeneratedEventArgs args)
        {
            if (args.Result.Confidence == SpeechRecognitionConfidence.High || args.Result.Confidence == SpeechRecognitionConfidence.Medium)
            {
                var ignore = Helpers.RunOnCoreDispatcherIfPossible(() => { var ignoreWakeupRet = WakeUpAndListen(); }, false);
            }
        }

        private void _speechRecognizer_HypothesisGenerated(SpeechRecognizer sender, SpeechRecognitionHypothesisGeneratedEventArgs args)
        {
            if (_textBox != null)
            {
                var ignore = Helpers.RunOnCoreDispatcherIfPossible(() => _textBox.Text = args.Hypothesis.Text.ToLower(), false);
            }
        }


        private async Task WakeUpAndListen()
        {
            try
            {
                await _continousSpeechRecognizer.ContinuousRecognitionSession.CancelAsync();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

            }
            await SpeakAsync("hey!");

            int retry = 3;

            while (true)
            {
                _textBox.Text = "";

                var spokenText = await ListenForText();
                if (string.IsNullOrWhiteSpace(spokenText) ||
                    spokenText.ToLower().Contains("cancel") ||
                    spokenText.ToLower().Contains("never mind"))
                {
                    break;
                }
                else
                {
                    _textBox.Text = $"\"{spokenText.ToLower()}\"";
                    var state = await HandleIntent(spokenText);
                    if (!state.Success)
                    {
                        _textBox.Text = "I don't know how to perform that action";
                        await Task.Delay(1000);

                        if (--retry < 1)
                            break;
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(state.TextResponse))
                        {
                            _textBox.Text = state.TextResponse;
                        }
                        else
                        {
                            _textBox.Text = state.SpeechRespose;
                        }

                        if (!state.handled)
                        {
                            if (!string.IsNullOrWhiteSpace(state.SpeechRespose))
                            {
                                await SpeakAsync(state.SpeechRespose);
                            }
                            else
                            {
                                await Task.Delay(1000);
                            }
                        }

                        retry = 3;

                    }
                }
            }

            await _continousSpeechRecognizer.ContinuousRecognitionSession.StartAsync();
        }

        private async Task<string> ListenForText()
        {
            string result = null;

            try
            {
                SpeechRecognitionResult speechRecognitionResult = await _speechRecognizer.RecognizeAsync();
                if (speechRecognitionResult.Status == SpeechRecognitionResultStatus.Success)
                {
                    result = speechRecognitionResult.Text;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return result;
        }

        public async Task SpeakAsync(string toSpeak)
        {
            if (_speechSynthesizer == null)
            {
                _speechSynthesizer = new SpeechSynthesizer();
                var voice = SpeechSynthesizer.AllVoices.Where(v => v.Gender == VoiceGender.Female && v.Language.Contains("en")).FirstOrDefault();
                if (voice != null)
                {
                    _speechSynthesizer.Voice = voice;
                }
            }
            var syntStream = await _speechSynthesizer.SynthesizeTextToStreamAsync(toSpeak);

            if (_player == null)
            {
                _player = new MediaPlayer();
            }

            var taskSource = new TaskCompletionSource<object>();
            TypedEventHandler<MediaPlayer, object> mediaEnded = null;
            mediaEnded += (s, e) =>
            {
                _player.MediaEnded -= mediaEnded;
                taskSource.SetResult(null);
            };

            _player.MediaEnded += mediaEnded;
            _player.Source = MediaSource.CreateFromStream(syntStream, syntStream.ContentType);
            _player.Play();

            await taskSource.Task;
        }

        public async Task<LUISIntentStatus> HandleIntent(string text)
        {
            try
            {
                if (_router == null)
                {
                    var handlers = new DroneIntents();
                    _router = IntentRouter.Setup(Keys.LUISAppId, Keys.LUISAzureSubscriptionKey, handlers, false);
                }
                var status = new LUISIntentStatus();
                status.service = this;
                var handled = await _router.Route(text, status);

                return status;
            }
            catch (Exception)
            {
                return new LUISIntentStatus()
                {
                    SpeechRespose = "LUIS and I are not talking right now, make sure IDs are correct in Keys.cs",
                    TextResponse = "Can't access LUIS, make sure to populate Keys.cs with the LUIS IDs",
                    Success = true
                };
            }
        }

        private async Task<bool> CheckForMicrophonePermission()
        {
            try
            {
                // Request access to the microphone 
                MediaCaptureInitializationSettings settings = new MediaCaptureInitializationSettings();
                settings.StreamingCaptureMode = StreamingCaptureMode.Audio;
                settings.MediaCategory = MediaCategory.Speech;
                MediaCapture capture = new MediaCapture();

                await capture.InitializeAsync(settings);
            }
            catch (UnauthorizedAccessException)
            {
                // The user has turned off access to the microphone. If this occurs, we should show an error, or disable
                // functionality within the app to ensure that further exceptions aren't generated when 
                // recognition is attempted.
                return false;
            }

            return true;
        }
    }
}
