using UnityEngine;
public class AccountServerMessage{
    
}

public class UserInfoMessasge: AccountServerMessage{
    public string userID;
}

public class LobbyInfoMessasge: AccountServerMessage{
    public int id;
    public UserInfoMessasge[] users;
}