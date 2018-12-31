import { PiecesTextureType } from "./piecesTextureType";

export class PieceStyle {
	constructor(public value: PiecesTextureType = PiecesTextureType.Symbols, public viewValue: string = "Symbols") {}
}