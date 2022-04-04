import * as express from 'express';
import * as bodyParser from 'body-parser';
import * as http from 'http';
import * as WebSocket from 'ws';
import * as Cors from 'cors';
import { AddressInfo } from 'ws';
import TaskManager from './TaskManager';

const app = express();
// app.use(bodyParser.json()); // Parsing JSON
app.use(bodyParser.urlencoded({ extended: true })); // Parsing Form data
app.use(Cors());

//initialize a simple http server
const server = http.createServer(app);

//initialize the WebSocket server instance
const wss = new WebSocket.Server({ server });

var taskManager = new TaskManager();

wss.on('connection', (ws: WebSocket) => {
    ws.send('Hi there, I am a WebSocket server');
});

app.get('/api/current-task-status', (req, res) => {
    res.send(taskManager.currentTaskStatus);
});

app.post('/api/mark-current-task-completed', (req, res) => {
    var data = JSON.parse(req.body?.data);
    taskManager.updateTaskStatus(
        data.isCompleted,
        data.failedTest,
        data.currentOutput,
    );
    res.send(true);
});

app.post('/api/move-to-next-task', (req, res) => {
    taskManager.moveToNextTask();
    wss.clients.forEach(client => {
        client.send(JSON.stringify({channel: "taskStatus", data: taskManager.currentTaskStatus}));
    });
    res.send(true);
});

app.post('/api/reset', (req, res) => {
    taskManager = new TaskManager();
    res.send(true);
});

app.post('/api/notify-code-change', (req, res) => {
    wss.clients.forEach(client => {
        client.send(JSON.stringify({channel: "xmlCode", data: req.body.blocklyXML}));
    });
    res.send('New code change notified! with following data' + req.body)
});

//start our server
server.listen(process.env.PORT || 8999, () => {
    var adressInfo = server.address() as AddressInfo;
    console.log(`Server started on port ${adressInfo.port} :)`);
});