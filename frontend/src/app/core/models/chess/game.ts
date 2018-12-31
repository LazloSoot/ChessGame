import { Side } from './side';
import { Move } from './move';

export class Game {
    id?: number;
    moves?: Move[];
    constructor(
        public fen: string,
        public sides: Side[]
        ) {}
}