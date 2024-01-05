import jwt, { JwtPayload } from 'jsonwebtoken';
import { v4 as uuidv4 } from 'uuid';

interface UserTokenData {
    userID: string,
    iat: number,
    exp: number
}
interface UserToken {
    valid: boolean;
    data?: UserTokenData;
}

export class TokenManager {

    constructor() {}

    public ReadUserToken(inputToken:string): UserToken{
        let token:UserToken = {
            valid:false
        }

        // invalid token - synchronous
        try {
            token.valid = true
            token.data = jwt.verify(inputToken, 'wrong-secret') as UserTokenData;
        } catch(err) {
            if (inputToken.length < 600){
                console.log("Invalid token: " + inputToken.toString())
            }else{
                console.log("Invalid token of length " + inputToken.length)
            }
        }
        
        return token
    };

    public GenerateGuestUserToken(): string{
        let newToken: UserTokenData = {
            userID: uuidv4(),
            iat: Math.floor(Date.now() / 1000) - 30,
            exp: Math.floor(Date.now() / 1000) + (60 * 60 * 48) // 48 hours
        }

        return jwt.sign(newToken, 'ChangeToUseSecretManagerToStoreSecret');
    };
}

