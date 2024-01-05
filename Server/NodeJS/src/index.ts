import config from "./config.json";
import net from "net";

console.log("Hello world", config.foo);

let server = net.createServer(function(socket) {
	socket.write('Echo server\r\n');
	socket.pipe(socket);
});

server.listen(7776, '127.0.0.1');
