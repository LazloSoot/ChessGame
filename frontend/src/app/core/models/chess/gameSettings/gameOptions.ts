import { GameSide } from "./gameSide";
import { OpponentType } from "./opponentType";
import { User } from "../../user";

export class GameOptions {
    constructor(
        public isEnPassantOn: boolean = true,
        public selectedSide: GameSide = GameSide.Random,
        public opponentType: OpponentType = OpponentType.Computer,
        public opponent: User = undefined
    ) {}
}