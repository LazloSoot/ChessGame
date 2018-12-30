import { Game } from "./game";
import { User } from "../user";
import { GameSide } from "./gameSettings";

export class Side {
    id?: number;
    game?: Game;
    points?: number;
    isDraw?: boolean;
    isResign?: boolean;
    constructor(
        public color: GameSide,
        public player?: User
    ) {}
}