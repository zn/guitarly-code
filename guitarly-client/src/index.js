import React from "react";
import ReactDOM from "react-dom";
import bridge from "@vkontakte/vk-bridge";

import { RouterContext } from '@happysanta/router';
import { router } from './routers';

import App from "./App";



// Init VK  Mini App
bridge.send("VKWebAppInit");
 
bridge.subscribe(({ detail: { type, data } }) => {
  if (type === "VKWebAppUpdateConfig") {
    const schemeAttribute = document.createAttribute("scheme");
    schemeAttribute.value = data.scheme ? data.scheme : "client_light";
    document.body.attributes.setNamedItem(schemeAttribute);
  }
});
 
ReactDOM.render(
  <RouterContext.Provider value={router}>
    <App />
  </RouterContext.Provider>,
  document.getElementById("root")
);

if (true || process.env.NODE_ENV === "development") {
  import("./eruda").then(({ default: eruda }) => {}); //runtime download
}
