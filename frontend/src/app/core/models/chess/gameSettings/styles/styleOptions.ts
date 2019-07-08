import { BoardTextureType } from "./boardTextureType";
import { PiecesTextureType } from "./piecesTextureType";

export class StyleOptions {
    constructor(
        public boardColor: BoardTextureType = BoardTextureType.Wood,
        public piecesStyle: PiecesTextureType = PiecesTextureType.Symbols
    ) {}
}