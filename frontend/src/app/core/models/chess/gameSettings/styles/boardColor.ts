import { BoardTextureType } from "./boardTextureType";

export class BoardColor {
	constructor(public value: BoardTextureType = BoardTextureType.Wood, public viewValue: string = 'Wood') {}
}