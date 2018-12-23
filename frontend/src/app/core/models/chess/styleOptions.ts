import { BoardTextureType } from "./texturesTypes/boardTextureType";
import { PiecesTextureType } from "./texturesTypes/piecesTextureType";

export class StyleOptions {
    constructor(
        public boardColor: BoardTextureType = BoardTextureType.Wood,
        public piecesStyle: PiecesTextureType = PiecesTextureType.Symbols
    ) {}
}