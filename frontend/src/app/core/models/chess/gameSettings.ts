import { StyleOptions } from "./styleOptions";
import { GameOptions } from "./gameOptions";

export class GameSettings {
    constructor(
        public style: StyleOptions = new StyleOptions(),
        public options: GameOptions = new GameOptions()
    ) {

    }
}