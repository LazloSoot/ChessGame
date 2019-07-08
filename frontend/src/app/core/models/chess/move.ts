import { Game } from "./game";
import { User } from "../user";

export class Move {
    id?: number;
    game?: Game;
    player?: User;
    ply?: number;
    fen?: number;
    moveNext?: string;
    fenAfterMove?: string;
    constructor()
    {}
}