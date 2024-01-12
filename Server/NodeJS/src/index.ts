import config from "./config.json";
import net from "node:net";
import { ExpressManager } from "./express";
import { TokenManager } from './token';
let tokenManager = new TokenManager();

interface Lobby{
	id: number
	users: LobbyUser[]
}

interface LobbyUser {
	validated: boolean
	userID: string
	socket: net.Socket
	lobby: Lobby
}

enum MessageType{
    UserInfo = 1,
    LobbyInfo = 2
}


let LobbyList: {[id: number] :Lobby} = {

}

function SendMessage(user:LobbyUser, messageType:MessageType, data:any){
	let messageLength =Buffer.alloc(2);
	let messageBuffer = Buffer.from(JSON.stringify(
		{
			type:messageType,
			data:data
		}
	), 'utf8');

	messageLength.writeUInt16LE(messageBuffer.length);
	console.log("Sending message length %s", messageBuffer.length)
	user.socket.write(messageLength);
	user.socket.write(messageBuffer);
}

function GetRandomLobbyNumber(): number{
	return Math.floor(Math.random() * 999999);
}

function JoinEmptyLobby(user:LobbyUser){
	console.log("JoinEmptyLobby")
	let randomLobbyID = GetRandomLobbyNumber();

	for (let index = 0; index < 1000; index++) {
		if(LobbyList[randomLobbyID] != undefined){
			// Suuuurely 1000 tries would do it :P
			randomLobbyID = GetRandomLobbyNumber();
		}else{
			index = 99999;
		}
	}

	LobbyList[randomLobbyID] = {
		id:randomLobbyID,
		users:[]
	}

	JoinLobby(randomLobbyID, user);
}

function LobbyToSerialisable(lobby:Lobby){
	let lobbyToSend: {[id: string]:any} = {
		id:lobby.id,
		users:[]
	}

	for (let i = 0; i < lobby.users.length; i++) {
		lobbyToSend.users.push(lobby.users[i].userID);
	}

	return lobbyToSend;
}

function JoinLobby(lobbyIDToJoin: number, user:LobbyUser){
	if(LobbyList[lobbyIDToJoin] == undefined){
		console.error("Unable to find lobby ID: ", lobbyIDToJoin);
	}else{
		LobbyList[lobbyIDToJoin].users.push(user);
		user.lobby = LobbyList[lobbyIDToJoin];

		let lobbySerialisable = LobbyToSerialisable(user.lobby);
		for (let i = 0; i < user.lobby.users.length; i++) {
			SendMessage(user.lobby.users[i], MessageType.LobbyInfo, lobbySerialisable);
		}

		console.log(`User ${user.userID} joined lobby ${lobbyIDToJoin} (${LobbyList[lobbyIDToJoin].users.length} users in lobby now)`);
	}
}

function LeaveLobby(user:LobbyUser){
	if(user.lobby.users.length == 1){
		// There's only 1 player in the lobby so 'leaving' won't do anything because
		// we always want the player to be in a lobby, even if it's just by themselves
	}if(user.lobby.users.length > 1){
		for (let i = 0; i < user.lobby.users.length; i++) {
			if(user.lobby.users[i] == user){
				// Remove the user from the lobby
				user.lobby.users.splice(i,1);
			}
		}

		console.log(`User ${user.userID} left lobby ${user.lobby.id} (${user.lobby.users.length} users in lobby now)`);
	}
}

function CleanupUser(user:LobbyUser | null){
	if(user == null){
		// Nothing to clean up
	} else {
		console.log("Cleaning up user.");
		
		if(user.lobby.users.length == 1){
			// No one other than the player is left in the lobby, just delete it
			console.log(`Deleting lobbyID: ${user.lobby.id}`)
			delete LobbyList[user.lobby.id]
		}else{
			// There's someone else in the lobby so we can just leave it
			LeaveLobby(user);
		}
		user = null
		console.log("LobbyList length: " + Object.keys(LobbyList).length);
	}
}

console.log("Hello world", config.foo);

let expressManager = new ExpressManager();
expressManager.Start();

function HandleAnonymousMessage(socket:net.Socket, data:Buffer): LobbyUser | null{
	let semiPos = data.indexOf(";")

	if(semiPos > 0){
		let jwt = data.toString().substring(0, semiPos);
		let userToken = tokenManager.ReadUserToken(jwt);

		if (userToken.valid && userToken.data){
			socket.setTimeout(0);
			console.log("Valid JWT");
			return {
				validated: true,
				userID: userToken.data.userID,
				socket:socket,
				lobby:{
					id:0,
					users:[]
				}
			}
		}else{
			socket.end('goodbye');
			console.log("Invalid JWT");
		}
	}else if(data.length > 600){
		socket.end('goodbye');
		console.log("Lengthy request without JWT");
	}

	return null;
}

function HandleAuthenticatedMessage(socket:net.Socket, data:Buffer, user:LobbyUser){

}

const server = net.createServer((socket) => {
	let lobbyUser:LobbyUser | null = null;

	socket.setTimeout(5000); // Wait 10 seconds for JWT
	
	socket.on('timeout', () => {
		console.log('socket timeout');
		try{
			socket.destroy();
		}catch(err){
			console.error("destroy error: ", err);
		}
	}); 

	socket.on('data', (data) => {
		if(lobbyUser == null){
			let messageResult = HandleAnonymousMessage(socket, data);
			
			if (messageResult == null){
				console.log("messageResult == null || lobbyUser == null");
				// The user was not validated. The socket has been closed if required.
			}else if (messageResult.validated == true){
				// The user is now validated
				console.log("messageResult.validated == true");
				
				lobbyUser = messageResult;
				console.log("Sending MessageType.UserInfo");
				SendMessage(lobbyUser, MessageType.UserInfo, {userID:lobbyUser.userID});
				JoinEmptyLobby(lobbyUser);
			}
		}else if (lobbyUser.validated == true){
			HandleAuthenticatedMessage(socket, data, lobbyUser);
		}
	});

	socket.on('close', function() {
		if(lobbyUser != null){
			CleanupUser(lobbyUser);
		}
		console.log('Connection closed');
	});
	
	socket.on('error', function(err) {
		console.error("Socket error: ", err);
	});
}).on('error', (err) => {
	console.error("Net Server error: ", err);
});

server.listen(7776, '127.0.0.1');
