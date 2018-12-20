import { Component, OnInit } from '@angular/core';
import { BoardTextureType, PiecesTextureType } from '../../core';

@Component({
  selector: 'app-chess-game',
  templateUrl: './chess-game.component.html',
  styleUrls: ['./chess-game.component.less']
})
export class ChessGameComponent implements OnInit {
  private boardType: BoardTextureType = BoardTextureType.Wood;
  private piecesType: PiecesTextureType = PiecesTextureType.Symbols;
  private fen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
  constructor() { }

  ngOnInit() {
  }

  change1(){
    this.boardType = BoardTextureType.StoneBlack;
  }

  change2(){
    this.boardType = BoardTextureType.StoneBlue;
  }

  change3(){
    this.boardType = BoardTextureType.StoneGrey;
  }

  change4(){
    this.piecesType = PiecesTextureType.Pieces;
  }
}
