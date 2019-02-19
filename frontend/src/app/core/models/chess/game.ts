import { Side } from './side';
import { Move } from './move';
import { GameStatus } from './gameStatus';
import { GameSide } from './gameSettings';

export class Game {
    id?: number;
    moves?: Move[];
    status?: GameStatus;
    creationDate?: Date;
    constructor(
        public fen: string,
        public sides: Side[]
        ) {}
}