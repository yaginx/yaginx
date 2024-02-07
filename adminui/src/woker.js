// worker.js文件
const workercode = () => {
  setInterval(() => {
      self.postMessage(Number.parseInt((Date.now() / 1000).toFixed(0)));
  }, 10000);

  self.onmessage = function(e) {
    console.log('Message received from main script');
    var workerResult = 'Received from main: ' + (e.data);
    console.log('Posting message back to main script');
    //self.postMessage(workerResult); // here it's working without self
  }
};

let code = workercode.toString();
code = code.substring(code.indexOf('{') + 1, code.lastIndexOf('}'));

const blob = new Blob([code], { type: 'application/javascript' });
const workerScript = URL.createObjectURL(blob);

export default workerScript;
