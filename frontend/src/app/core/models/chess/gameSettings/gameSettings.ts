import { StyleOptions } from "./styles/styleOptions";
import { GameOptions } from "./gameOptions";

export class GameSettings {
    gameId?: number;
    constructor(
        public style: StyleOptions = new StyleOptions(),
        public options: GameOptions = new GameOptions(),
        public startFen: string = 'rnbq1k2/ppppp3/5p2/7Q/8/2rB4/PPPPPPPP/RNB1K1NR w KQkq - 0 1'
    ) {

    }
}