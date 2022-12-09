const os = require("os");
const fs = require("fs");
require("dotenv").config();

console.log("NODE ENV: ");
console.log(process.env.NODE_ENV);

if (process.env.NODE_ENV === "production") {
  console.log("Creating config for production");
  if (process.env.REACT_APP_HEROKU_BASE_URL) {
    console.log(
      "Setting ENV REACT_APP_HEROKU_BASE_URL to: " +
        process.env.REACT_APP_HEROKU_BASE_URL
    );
    process.env.REACT_APP_ADRESS = process.env.REACT_APP_HEROKU_BASE_URL;
  } else {
    console.log(
      "REACT_APP_HEROKU_BASE_URL is missing. Please specify in docker-compose and rebuild "
    );
    process.exit(1);
  }
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
