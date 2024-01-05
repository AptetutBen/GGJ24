import config from "./config.json";
import net from "net";
import { ExpressManager } from "./express";


console.log("Hello world", config.foo);

let expressManager = new ExpressManager();
expressManager.Start();

let server = net.createServer(function(socket) {
	socket.write('Echo server\r\n');
	socket.pipe(socket);
});

server.listen(7776, '127.0.0.1');
