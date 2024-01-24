import * as k8s from '@kubernetes/client-node';
import needle from 'needle';

export class KubeTime {
    public k8sApi: k8s.CoreV1Api;

    constructor() {
        let kc = new k8s.KubeConfig();
        kc.loadFromDefault();
        
        this.k8sApi = kc.makeApiClient(k8s.CoreV1Api);
    }

    public async ReadPods(): Promise<string>{
        try {
            const podsRes = await this.k8sApi.listNamespacedService('ggj24');
            
            for (let i = 0; i < podsRes.body.items.length; i++) {
                const item = podsRes.body.items[i];
                if (item.spec && item.spec.ports){
                    const resp = await needle('get', "http://192.168.88.65:" + item.spec.ports[0].nodePort + "/info");

                    console.log(item.spec.ports[0].nodePort + ":", JSON.stringify(resp.body));
                }
            }
        } catch (err) {
            console.error(err);
        }
        
        return "";
    };
}

