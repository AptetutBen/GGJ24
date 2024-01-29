import * as k8s from '@kubernetes/client-node';
import { randomUUID } from 'crypto';
import needle from 'needle';
import { Discord } from './discord';

let discord = new Discord();

enum MessageType{
    None = -1,
    MMO = 1, // lol
    Group = 2 // lol
}

export interface GameServerPodInfo{
	port: number
	name: string
}

export class KubeTime {
    public k8sApi: k8s.CoreV1Api;
    private maxServers = 10;
    private podsBeingTerminated:{[id: string]:boolean} = {}

    constructor() {
        let kc = new k8s.KubeConfig();
        kc.loadFromDefault();
        
        this.k8sApi = kc.makeApiClient(k8s.CoreV1Api);
    }

    private timeout(ms:number) {
        return new Promise(resolve => setTimeout(resolve, ms));
    }

    public async CreatePod(clientVersion:string, gameMode:number):Promise<GameServerPodInfo | null>{
        try {
            
            const items = await this.RequestPods();
            if(items == null){
                console.error("Error requesting pods.");
                return null;
            }
            
            let serverNumbersInUse:{[id:number]:boolean} = {};

            for (let i = 0; i < items.length; i++) {
                let item = items[i];

                if(item.metadata == undefined || item.metadata.labels == undefined){
                    break;   
                }
                
                let serviceNumber:string|null = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    break;
                }

                serverNumbersInUse[Number(serviceNumber)] = true;
            }
            
            let nextServerNumber = -1;
            for (let i = 1; i <= this.maxServers; i++) {
                if(serverNumbersInUse[i] == undefined){
                    nextServerNumber = i;
                    break;
                }
            }

            if(nextServerNumber > this.maxServers || nextServerNumber == -1){
                console.error("Hit server capacity!!!");
                return null;
            }
            
            let container = new k8s.V1Container();
            container.name = "ggj24-gameserver-" + randomUUID();
            container.image = "10.147.20.23:5000/ggj24/ggj24-gameserver:" + clientVersion;

            let gameServerPod = new k8s.V1Pod();
            gameServerPod.apiVersion = "v1";
            gameServerPod.spec = new k8s.V1PodSpec();
            gameServerPod.spec.containers = [];
            gameServerPod.spec.containers.push(container);
            gameServerPod.metadata = new k8s.V1ObjectMeta();
            gameServerPod.metadata.name = container.name;
            gameServerPod.metadata.labels = {
                "ggj24.service":nextServerNumber.toString(),
                "ggj24.gameMode":gameMode.toString()
            };
            
            const gameServerRequest = await this.k8sApi.createNamespacedPod('ggj24', gameServerPod);
            discord.Post("Creating pod: "+ container.name);
            console.log("Created pod: " + container.name);
            return {
                name:container.name,
                port: Number("301" + ("00" + nextServerNumber.toString()).slice(-2))
            }
            
        } catch (err) {
            console.error("Caught exception: ", err);
            return null;
        }
    }

    public async GetPodPort(podName:string):Promise<number>{
        try {
            const podsRes = await this.k8sApi.readNamespacedPod(podName, 'ggj24');

            if(podsRes.body.status == undefined || podsRes.body.status.conditions == undefined){
                console.log("Status or condition was not defined");
                return -1;
            }

            let readyConditionFound = false;
            for (let i = 0; i < podsRes.body.status.conditions.length; i++) {
                const condition = podsRes.body.status.conditions[i];

                if(condition.type == "Ready"){
                    readyConditionFound = true;

                    if(condition.status != "True"){
                        // console.log("Waiting for pod to be ready, status: " + condition.status );
                        return -1;
                    }else{
                        // console.log("Pod is ready");
                    }
                }
            }
            
            if(readyConditionFound == false){
                console.log("Ready condition was not found");
                return -1;
            }

            if(podsRes.body.metadata == undefined || podsRes.body.metadata.labels == undefined){
                console.error("Pod metadata or label was empty even though the pod was ready");
                return -1;
            }
            
            // Pod is ready, now wait for the game server
            let serviceNumber = podsRes.body.metadata.labels["ggj24.service"];
            if(serviceNumber == null){
                console.error("serviceNumber was null even though the pod was ready");
                return -1;
            }
            
            return Number(serviceNumber);
        }catch(err){
            console.error(err);
            return -1;
        }
    }

    public async WaitForPodToBeReady(podName:string, gameMode:number | null = null):Promise<boolean>{
        const timeoutDurationInSeconds = 20;

        let timeout = Date.now() + (timeoutDurationInSeconds * 1000);
        let ready = false;

        console.log("Waiting for pod: " + podName);
        let serviceNumber = -1;

        while(ready == false && timeout > Date.now()){
            await this.timeout(300);

            if(serviceNumber == -1){
                serviceNumber = await this.GetPodPort(podName);
            }else{
                try {
                    const resp = await needle('get', "http://10.147.20.23:302" + ("00" + serviceNumber.toString()).slice(-2) + "/info", {parse_response:false});
                    JSON.parse(resp.body);
                    console.log("Pod "+podName+" ready after " + (timeoutDurationInSeconds - ((timeout - Date.now())/1000)).toString() + " seconds.");
                    if(gameMode == MessageType.MMO){
                        const gameModeResp = await needle('get', "http://10.147.20.23:302" + ("00" + serviceNumber.toString()).slice(-2) + "/mode_mmo", {parse_response:false});
                    }else if(gameMode == MessageType.Group){
                        const gameModeResp = await needle('get', "http://10.147.20.23:302" + ("00" + serviceNumber.toString()).slice(-2) + "/mode_group", {parse_response:false});
                    }
                    return true;
                } catch (e) {
                    console.log("Game server is not ready");
                }
            }

            // console.log((timeout - Date.now())/1000 + "seconds left on timeout");
        }

        
        console.log("Server timed out! " + timeout + " > " + Date.now());
        console.log("ready: " + ready + "\ntimeout > Date.now(): " + (timeout > Date.now()));

        return false;
    }

    public async RequestPods(): Promise<k8s.V1Pod[] | null>{
        try {
            const podsRes = await this.k8sApi.listNamespacedPod('ggj24');
            return podsRes.body.items;
        } catch (err) {
            console.error(err);
            return null;
        }
        
    };

    public async GetPodsRunningGameMode(gameModeToFind:number): Promise<GameServerPodInfo | null>{
        try {
            const items = await this.RequestPods();
            if(items == null){
                return null;
            }

            for (let i = 0; i < items.length; i++) {
                let item = items[i];

                if(item.metadata == undefined || item.metadata.labels == undefined){
                    break;   
                }
                
                let gameMode = item.metadata.labels["ggj24.gameMode"];
                let serviceNumber = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    break;
                }
                if(gameMode == null){
                    break;
                }
                if(item.metadata.name == null){
                    break;
                }
                if(item.status == null || item.status.conditions == null){
                    break;
                }
                if(this.podsBeingTerminated[item.metadata.name]){
                    break;
                }

                let success = await this.WaitForPodToBeReady(item.metadata.name);
                
                if(gameModeToFind.toString() == gameMode){
                    return {
                        name: item.metadata.name,
                        port: Number("301" + ("00" + serviceNumber.toString()).slice(-2))
                    }
                }
            }
        } catch (err) {
            console.error("Error in LookForServersToTerminate", err);
        }

        return null;
    }

    public async LookForServersToTerminate(){
        try {
            const items = await this.RequestPods();
            if(items == null){
                return
            }

            let podsAlive:string[] = [];

            for (let i = 0; i < items.length; i++) {
                let item = items[i];
                
                if(item.metadata == undefined || item.metadata.labels == undefined){
                    break;   
                }
                
                let serviceNumber:string|null = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    break;
                }
                if(item.metadata.name == undefined){
                    break;
                }
                if(this.podsBeingTerminated[item.metadata.name] == true){
                    break;
                }

                podsAlive.push(item.metadata.name);

                const resp = await needle('get', "http://10.147.20.23:302" + ("00" + serviceNumber.toString()).slice(-2) + "/info");
                
                if(resp.body.shouldTerminate === true){
                    discord.Post("Terminating pod: " + item.metadata.name);
                    console.log("Terminating " + item.metadata.name);
                    this.podsBeingTerminated[item.metadata.name] = true;
                    await this.k8sApi.deleteNamespacedPod(item.metadata.name, "ggj24");
                }
            }

            for (const [key, value] of Object.entries(this.podsBeingTerminated)) {
                if(podsAlive.includes(key) == false){
                    delete this.podsBeingTerminated[key];
                }
            }

        } catch (err) {
            console.error("Error in LookForServersToTerminate", err);
        }
    }
    
    public async ReadPods(): Promise<void>{
        try {
            const items = await this.RequestPods();
            if(items == null){
                return
            }

            for (let i = 0; i < items.length; i++) {
                let item = items[i];

                if(item.metadata == undefined || item.metadata.labels == undefined){
                    break;   
                }
                
                let serviceNumber:string|null = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    break;
                }

                const resp = await needle('get', "http://10.147.20.23:302" + ("00" + serviceNumber.toString()).slice(-2) + "/info");
                console.log(serviceNumber + ":", JSON.stringify(resp.body));
            }
        } catch (err) {
            console.error(err);
        }
         
    };
}

