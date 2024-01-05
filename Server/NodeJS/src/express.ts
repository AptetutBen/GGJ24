import express, { Request, Response, Express} from 'express';
import { TokenManager } from './token';

export class ExpressManager {
    app:Express;
    port:string | number;
    tokenManager:TokenManager;
    
    constructor() {
        this.app = express();
        this.port = process.env.PORT || 3000;
        this.tokenManager = new TokenManager();
    }

    Start():void{
        this.app.get('/server/account/guest', (req: Request, res: Response) => {
            res.send(this.tokenManager.GenerateGuestUserToken());
        });
        
        this.app.listen(this.port, () => {
            console.log(`Server running at http://localhost:${this.port}`);
        });
    }
}