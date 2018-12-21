import { Component, OnInit } from '@angular/core';
import { BoardTextureType, PiecesTextureType } from '../../../core';

@Component({
  selector: 'app-game-settings',
  templateUrl: './game-settings.component.html',
  styleUrls: ['./game-settings.component.less']
})
export class GameSettingsComponent implements OnInit {
  private boardColors: BoardColor[];
  private boardColor: BoardColor;
  private pieceStyles: PieceStyle[];
  private pieceStyle: PieceStyle;
  constructor() { }

  ngOnInit() {
    let keys = Object.keys(BoardTextureType);
    this.boardColors = Array(keys.length).fill({}).map((x, i) => {
      let a = keys[i];
      let b = BoardTextureType[keys[i]];
      let c = BoardTextureType[BoardTextureType[keys[i]]];
      debugger;
      return new BoardColor(
        BoardTextureType[keys[i]],
        keys[i]
      )
    });

    keys = Object.keys(PiecesTextureType);
    this.pieceStyles = Array(keys.length).fill({}).map((x, i) => {
      return new PieceStyle(
        PiecesTextureType[keys[i]],
        keys[i]
      )
    });
  }

  getBoardUrlPrev() {
    return `${imgsUrl}${this.boardColor.value}/prev.jpg`;
  }

  getPieceUrlPrev() {
    return `${imgsUrl}${this.pieceStyle.value}/prev.jpg`;
  }
}

const imgsUrl = '../../../../assets/images/Chess';

export class BoardColor {
  constructor(
    public value: BoardTextureType,
    public viewValue: string
    ) {}
}

export class PieceStyle {
  constructor(
    public value: PiecesTextureType,
    public viewValue: string
    ) {}
}