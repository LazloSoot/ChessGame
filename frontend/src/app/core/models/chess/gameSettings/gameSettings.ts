import { StyleOptions } from "./styles/styleOptions";
import { GameOptions } from "./gameOptions";

export class GameSettings {
    gameId?: number;
    constructor(
        public style: StyleOptions = new StyleOptions(),
        public options: GameOptions = new GameOptions(),
        public startFen: string = 'r1b1kbnr/pppppppp/8/8/5P2/Nq1Q1B2/PPPP1P1P/Rn2K2R w KQkq - 0 1'
    ) {

    }
}