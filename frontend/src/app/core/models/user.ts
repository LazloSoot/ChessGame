export class User {
    constructor(
        public name: string, 
        public avatarUrl?: string,
        public id?: number,
        public registrationDate?: Date,
        public lastSeenDate?: Date,
        public uid?: string,
        public isOnline: boolean = false
        ) {
    }
}