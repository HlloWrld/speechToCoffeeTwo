import React from "react";
import axios from "axios";
import Recorder from "recorder-js";

export default function Counter() {
  const audioContext = new (window.AudioContext || window.webkitAudioContext)();
  const [blob, setBlob] = React.useState(null);

  const recorder = new Recorder(audioContext, {
    // An array of 255 Numbers
    // You can use this to visualize the audio stream
    // If you use react, check out react-wave-stream
    // onAnalysed: (data) => console.log(data),
  });

  let isRecording = false;

  navigator.mediaDevices
    .getUserMedia({ audio: true })
    .then((stream) => recorder.init(stream))
    .catch((err) => console.log("Uh oh... unable to get stream...", err));

  function startRecording() {
    recorder.start().then(() => (isRecording = true));
  }

  function stopRecording() {
    recorder.stop().then(({ blob, buffer }) => {
      setBlob(blob);

      // buffer is an AudioBuffer
    });
  }

  function download() {
    let data = new FormData();

    data.append("blob", blob, "audio.wav");

    const config = {
      headers: { "content-type": "multipart/form-data" },
    };
    axios.post("/speech", data, config);
    Recorder.download(blob, "my-audio-file"); // downloads a .wav file
  }
  return (
    <div>
      <h1>Counter</h1>

      <p>This is a simple example of a React component.</p>

      {/* <p aria-live='polite'>
        Current count: <strong>{this.state.currentCount}</strong>
      </p> */}

      <button className='btn btn-primary' onClick={startRecording}>
        Record Audio
      </button>

      <button id='stop' className='btn btn-primary' onClick={stopRecording}>
        Stop
      </button>
      <button id='send' className='btn btn-primary' onClick={download}>
        Send
      </button>
      <audio id='audioDiv'></audio>
    </div>
  );
}
