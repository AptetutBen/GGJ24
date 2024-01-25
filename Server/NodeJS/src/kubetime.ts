import * as k8s from '@kubernetes/client-node';
import { randomUUID } from 'crypto';
import needle from 'needle';

export class KubeTime {
    public k8sApi: k8s.CoreV1Api;
    private maxServers = 10;

    constructor() {
        let kc = new k8s.KubeConfig();
        kc.loadFromDefault();
        
        this.k8sApi = kc.makeApiClient(k8s.CoreV1Api);
    }

    public async CreateDistribution():Promise<void>{
        try {
            
            const items = await this.RequestPods();
            if(items == null){
                return
            }
            
            let serverNumbersInUse:{[id:number]:boolean} = {};

            for (let i = 0; i < items.length; i++) {
                let item = items[i];

                if(item.metadata == undefined || item.metadata.labels == undefined){
                    continue;   
                }
                
                let serviceNumber:string|null = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    continue;
                }

                serverNumbersInUse[Number(serviceNumber)] = true;
            }
            
            let nextServerNumber = -1;
            for (let i = 1; i < this.maxServers; i++) {
                if(serverNumbersInUse[i] == undefined){
                    nextServerNumber = i;
                    break;
                }
            }

            if(nextServerNumber == this.maxServers){
                console.error("Hit server capacity!!!");
                return;
            }
            
            let container = new k8s.V1Container();
            container.name = "ggj24-gameserver-" + randomUUID();
            container.image = "10.147.20.23:5000/ggj24/ggj24-gameserver:2790d7c";

            let gameServerPod = new k8s.V1Pod();
            gameServerPod.apiVersion = "v1";
            gameServerPod.spec = new k8s.V1PodSpec();
            gameServerPod.spec.containers = [];
            gameServerPod.spec.containers.push(container);
            gameServerPod.metadata = new k8s.V1ObjectMeta();
            gameServerPod.metadata.name = container.name;
            gameServerPod.metadata.labels = {
                "ggj24.service":nextServerNumber.toString()
            };


            const gameServerRequest = await this.k8sApi.createNamespacedPod('ggj24', gameServerPod);
            
        } catch (err) {
            console.error(err);
        }
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

    public async ReadPods(): Promise<void>{
        try {
            const items = await this.RequestPods();
            if(items == null){
                return
            }

            for (let i = 0; i < items.length; i++) {
                let item = items[i];

                if(item.metadata == undefined || item.metadata.labels == undefined){
                    continue;   
                }
                
                let serviceNumber:string|null = item.metadata.labels["ggj24.service"];
                if(serviceNumber == null){
                    continue;
                }

                const resp = await needle('get', "http://10.147.20.23:3020" + serviceNumber + "/info");
                console.log(serviceNumber + ":", JSON.stringify(resp.body));
            }
        } catch (err) {
            console.error(err);
        }
         
    };
}

