import * as express from 'express';
import * as bodyParser from 'body-parser';
import * as http from 'http';
import * as WebSocket from 'ws';

const app = express();
app.use(bodyParser.json());

//initialize a simple http server
const server = http.createServer(app);

//initialize the WebSocket server instance
const wss = new WebSocket.Server({ server });

wss.on('connection', (ws: WebSocket) => {
    ws.send('Hi there, I am a WebSocket server');
});

app.get('/api/test', (req, res) => {
    res.send('Hello World!')
});

app.post('/api/notify-code-change', (req, res) => {
    wss.clients.forEach(client => {
        client.send(JSON.stringify(req.body));
    });
    res.send('New code change notified! with following data' + req.body)
});

//start our server
server.listen(process.env.PORT || 8999, () => {
    console.log(`Server started on port ${server.address()} :)`);
});
