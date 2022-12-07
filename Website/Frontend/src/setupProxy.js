const { createProxyMiddleware } = require("http-proxy-middleware");

console.log("HAHA");
console.log(process.env);

module.exports = function (app) {
  app.use(
    "/api",
    createProxyMiddleware({
      target: "http://localhost:8999",
      changeOrigin: true,
    })
  );
};
