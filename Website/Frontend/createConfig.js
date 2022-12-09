const os = require("os");
const fs = require("fs");
require("dotenv").config();

const ifaces = os.networkInterfaces();

// Iterate over interfaces ...
const adresses = Object.values(ifaces).reduce((acc, iface) => {
  const targetAdresses = iface
    .filter((adress) => adress.family === "IPv4" && !adress.internal)
    .map((a) => a.address);
  return acc.concat(targetAdresses);
}, []);

console.log(adresses);

process.env.NODE_ENV = "production";

if (adresses[0] !== process.env.REACT_APP_ADRESS) {
  console.log("Local IP has changes, updating the .evn");
  if (process.env.NODE_ENV === "production") {
    console.log("In production, creating ENV variable");
    process.env.REACT_APP_ADRESS = process.env.REACT_APP_HEROKU_BASE_URL;
  } else {
    fs.writeFileSync(__dirname + "/.env", "REACT_APP_ADRESS=" + adresses[0]);
  }
}

console.log("Local IP Adress of Server: ");
console.log(adresses);
