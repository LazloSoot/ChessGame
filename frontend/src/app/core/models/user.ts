export class User {
    public uid?: string;
    public isOnline: boolean = false;
    constructor(
        public name: string, 
        public avatarUrl?: string,
        public id?: number,
        public registrationDate?: Date,
        public lastSeenDate?: Date
        ) {
    }
}