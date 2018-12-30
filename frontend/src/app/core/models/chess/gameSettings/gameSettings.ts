import { StyleOptions } from "./styles/styleOptions";
import { GameOptions } from "./gameOptions";

export class GameSettings {
    gameId?: number;
    constructor(
        public style: StyleOptions = new StyleOptions(),
        public options: GameOptions = new GameOptions(),
        public startFen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1'
    ) {

    }
}