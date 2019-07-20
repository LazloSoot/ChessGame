import { Component, OnInit, Output, EventEmitter, Input } from '@angular/core';
import { BoardColor, PieceStyle, BoardTextureType, PiecesTextureType, StyleOptions } from '../../../core';

@Component({
	selector: 'app-settings',
	templateUrl: './settings.component.html',
	styleUrls: ['./settings.component.less']
})
export class SettingsComponent implements OnInit {
	@Output() onSettingsDefined: EventEmitter<StyleOptions> = new EventEmitter<StyleOptions>(null);
	@Input() styles: StyleOptions;
	private boardColors: BoardColor[];
	private boardColor: BoardColor;
	private pieceStyles: PieceStyle[];
	private pieceStyle: PieceStyle;

	constructor() { }

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
			
		this.boardColor = this.boardColors.filter(e => e.value === this.styles.boardColor)[0];
		this.pieceStyle = this.pieceStyles.filter(e => e.value === this.styles.piecesStyle)[0];
	}

	apply() {
		this.onSettingsDefined.emit(new StyleOptions(this.boardColor.value, this.pieceStyle.value));
	}

	getBoardUrlPrev() {
		return `${imgsUrl}${this.boardColor.value}/prev.jpg`;
	}

	getPieceUrlPrev() {
		return `${imgsUrl}${this.pieceStyle.value}/prev.png`;
	}

}

const imgsUrl = "../../../../assets/images/Chess";
