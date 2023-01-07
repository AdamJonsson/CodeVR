const os = require("os");
const fs = require("fs");
require("dotenv").config();

console.log("NODE ENV: ");
console.log(process.env.NODE_ENV);

if (process.env.NODE_ENV === "production") {
  console.log(process.env.REACT_APP_ADRESS);
  console.log("In prod. Remember to set REACT_APP_ADRESS ENV variable");
} else {
  // Iterate over interfaces ...

  const ifaces = os.networkInterfaces();

  const adresses = Object.values(ifaces).reduce((acc, iface) => {
    const targetAdresses = iface
      .filter((adress) => adress.family === "IPv4" && !adress.internal)
      .map((a) => a.address);
    return acc.concat(targetAdresses);
  }, []);

  console.log("Local IP Adress of Server: ");
  console.log(adresses);

  if (adresses[0] !== process.env.REACT_APP_ADRESS) {
    console.log("Local IP has changes, updating the .evn and writing to file");
    fs.writeFileSync(__dirname + "/.env", "REACT_APP_ADRESS=" + adresses[0]);
  }
}
