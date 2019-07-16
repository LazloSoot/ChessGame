import { Component, OnInit } from '@angular/core';
import { BoardColor, PieceStyle, BoardTextureType, PiecesTextureType } from '../../../core';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.less']
})
export class SettingsComponent implements OnInit {
  private boardColors: BoardColor[];
	private boardColor: BoardColor = new BoardColor();
	private pieceStyles: PieceStyle[];
  private pieceStyle: PieceStyle = new PieceStyle();
  
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
  }

  apply() {
    
  }

  getBoardUrlPrev() {
		return `${imgsUrl}${this.boardColor.value}/prev.jpg`;
	}

	getPieceUrlPrev() {
		return `${imgsUrl}${this.pieceStyle.value}/prev.png`;
	}

}

const imgsUrl = "../../../../assets/images/Chess";
