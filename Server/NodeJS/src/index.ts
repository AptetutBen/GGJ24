import config from "./config.json";
import net from "node:net";
import { ExpressManager } from "./express";


console.log("Hello world", config.foo);

let expressManager = new ExpressManager();
expressManager.Start();

const server = net.createServer((socket) => {
	socket.write('Hello from a NodeJS TCP Server');
	socket.setTimeout(10000); // Wait 10 seconds for JWT

	socket.on('timeout', () => {
		console.log('socket timeout');
		socket.end();
	}); 

	socket.on('data', (data) => {
		console.log('Got Data: ' + data.toString());
		socket.setTimeout(0);

		// socket.end('goodbye\n');
	});

	socket.on('close', function() {
		console.log('Connection closed');
	});
}).on('error', (err) => {
	console.error("NodeJS Net Server error: ", err);
	throw err;
});

server.listen(7776, '127.0.0.1');
