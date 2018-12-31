export class User {
    id: number;
    uid: string;
    avatarUrl: string;
    name: string;
    constructor(uid: string, name: string) {
        this.uid = uid;
        this.name = name;
        this.avatarUrl = undefined;
        this.id = undefined;
    }
}