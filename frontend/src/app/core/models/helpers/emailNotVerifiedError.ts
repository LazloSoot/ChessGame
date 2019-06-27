export class EmailNotVerifiedError extends Error{
    __proto__: EmailNotVerifiedError;
    constructor(message: string) {
        const trueProto = new.target.prototype;
        super(message);
        this.__proto__=trueProto;
    }
}