import { Component, OnInit, Output } from "@angular/core";
import {
	BoardTextureType,
	PiecesTextureType,
	GameSettings,
	StyleOptions
} from "../../../core";
import { EventEmitter } from "@angular/core";
import { GameOptions } from "../../../core/models/chess/gameOptions";
import { MatDialogRef } from "@angular/material";

@Component({
	selector: "app-game-settings-dialog",
	templateUrl: "./game-settings-dialog.component.html",
	styleUrls: ["./game-settings-dialog.component.less"]
})
export class GameSettingsDialogComponent implements OnInit {
	@Output() onSettingsDefined: EventEmitter<GameSettings> = new EventEmitter<GameSettings>(null);
	private boardColors: BoardColor[];
	private boardColor: BoardColor = new BoardColor;
	private pieceStyles: PieceStyle[];
	private pieceStyle: PieceStyle = new PieceStyle;
	private isEnPassantOn: boolean = true;
	private isWhiteSide: boolean = true;
	private a;
	constructor(
		private dialogRef: MatDialogRef<GameSettingsDialogComponent>,
	) {}

	ngOnInit() {
		let keys = Object.keys(BoardTextureType);
		this.boardColors = Array(keys.length)
			.fill({})
			.map((x, i) => {
				return new BoardColor(BoardTextureType[keys[i]], keys[i]);
			});

		keys = Object.keys(PiecesTextureType);
		this.pieceStyles = Array(keys.length)
			.fill({})
			.map((x, i) => {
				return new PieceStyle(PiecesTextureType[keys[i]], keys[i]);
			});
	}

	submit() {
		this.onSettingsDefined.emit(
			new GameSettings(
				new StyleOptions(this.boardColor.value, this.pieceStyle.value),
				new GameOptions(this.isEnPassantOn, this.isWhiteSide)
			)
		);
		this.dialogRef.close();
	}

	back() {
		this.onSettingsDefined.emit(undefined);
		this.dialogRef.close();
	}

	getBoardUrlPrev() {
		return `${imgsUrl}${this.boardColor.value}/prev.jpg`;
	}

	getPieceUrlPrev() {
		return `${imgsUrl}${this.pieceStyle.value}/prev.png`;
	}

	getSidePrev() {
		return `${imgsUrl}/Symbols/Stone/` + ((this.isWhiteSide) ? 'QueenW.png' : 'QueenB.png');
	}
}

const imgsUrl = "../../../../assets/images/Chess";

export class BoardColor {
	constructor(public value: BoardTextureType = BoardTextureType.Wood, public viewValue: string = 'Wood') {}
}

export class PieceStyle {
	constructor(public value: PiecesTextureType = PiecesTextureType.Symbols, public viewValue: string = "Symbols") {}
}
