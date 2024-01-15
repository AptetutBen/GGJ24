using UnityEngine;
public class AccountServerMessage{}

public struct UserData{
    // TODO: Ben add things to this
    // Allowed: String / floats / ints / bools
}

public struct GameSettings{
    // TODO: Ben & Luis chat about what goes here
}

public class MessageUserInfo: AccountServerMessage{
    public string userID; 
    public UserData userData;
}

[System.Serializable]
public class MessageUserLobbyInfo: MessageUserInfo{
    public bool ready;
}

[System.Serializable]
public class MessageUserLobbyReady: AccountServerMessage{
    public string userID;
    public bool ready;
}

public class MessageLobbyInfo: AccountServerMessage{
    public string lobbyID;
    public MessageUserLobbyInfo[] users; // First user in the array is the lobby owner
}

[System.Serializable]
public class MessageReady: AccountServerMessage{
    public MessageUserLobbyReady[] users; // First user in the array is the lobby owner
}

public class MessageChat: AccountServerMessage{
    public string userID;
    public string chatMessage;
}

public abstract class AccountServerRequest{
    public int type;
    public abstract string ToJSON(); 
}

public class RequestJoinLobby: AccountServerRequest{
    public string lobbyID;
    
    public RequestJoinLobby(string lobbyID){
        this.lobbyID = lobbyID;
        type = (int)MessageType.JoinLobby;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestLeaveLobby: AccountServerRequest{

    public RequestLeaveLobby(){
        type = (int)MessageType.LeaveLobby;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestKickPlayer: AccountServerRequest{
    public string userID;
    
    public RequestKickPlayer(string userID){
        this.userID = userID;
        type = (int)MessageType.KickPlayer;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestUpdateUser: AccountServerRequest{
    public UserData userData;
    
    public RequestUpdateUser(UserData userData){
        this.userData = userData;
        type = (int)MessageType.UpdateUser;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestStartGame: AccountServerRequest{
    
    public RequestStartGame(){
        type = (int)MessageType.StartGame;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestReady: AccountServerRequest{
    public bool ready;
    
    public RequestReady(bool ready){
        this.ready = ready;
        type = (int)MessageType.Ready;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestChat: AccountServerRequest{
    public string chatData;
    
    public RequestChat(string chatData){
        this.chatData = chatData;
        type = (int)MessageType.Chat;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}

public class RequestGameSettings: AccountServerRequest{
    public GameSettings gameSettings;
    
    public RequestGameSettings(GameSettings gameSettings){
        this.gameSettings = gameSettings;
        type = (int)MessageType.GameSettings;
    }

    public override string ToJSON(){
        return JsonUtility.ToJson(this);
    }
}