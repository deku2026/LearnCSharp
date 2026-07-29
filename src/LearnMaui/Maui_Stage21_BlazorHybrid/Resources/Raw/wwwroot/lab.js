function updateFromDotNet(message) {
  document.getElementById("status").textContent = message;
  window.HybridWebView.SendRawMessage(`updated:${message}`);
  return "dom-updated";
}

async function invokeNative() {
  const result = await window.HybridWebView.InvokeDotNet("Echo", ["hello from JavaScript"]);
  document.getElementById("result").textContent = result;
}

document.getElementById("native").addEventListener("click", invokeNative);
