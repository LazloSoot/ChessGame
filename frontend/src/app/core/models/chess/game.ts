import { Side } from './side';
import { Move } from './move';
import { GameStatus } from './gameStatus';
import { GameSide } from './gameSettings';

export class Game {
    id?: number;
    moves?: Move[];
    isWon?: boolean;
    isDraw?: boolean;
    isResigned?: boolean;
    status?: GameStatus;
    side?: GameSide;
    creationDate?: Date;
    constructor(
        public fen: string,
        public sides: Side[]
        ) {}
}