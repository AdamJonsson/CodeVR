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

if (adresses[0] !== process.env.REACT_APP_ADRESS) {
  console.log("Local IP has changes, updating the .evn");
  fs.writeFileSync(__dirname + "/.env", "REACT_APP_ADRESS=" + adresses[0]);
}

console.log("Local IP Adress of Server: ");
console.log(adresses);
