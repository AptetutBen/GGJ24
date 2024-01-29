import needle from 'needle';

let discordWebHook = "1201321114840879144/OhnqVYaaMiCRsMTd3jezj47RXebWIwJxjDF9Fsp2X_ywjY_0TI9io529renyGk6zoeuq"

export class Discord {

    constructor() {}

    public async Post(inputMessage:string):Promise<void>{
        var payload = {
            content: inputMessage.toString().replace(/\*/g, '**')
        };
        console.log("Posting to discord: " + inputMessage);
        const resp = await needle.post("https://discord.com/api/webhooks/" + discordWebHook, payload);
    };

}

