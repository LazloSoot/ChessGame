export class User {
    constructor(
        public uid: string, 
        public name: string, 
        public avatarUrl?: string,
        public id?: number
        ) {
    }
}