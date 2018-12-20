import { Component, OnInit } from '@angular/core';
import { BoardTextureType, PiecesTextureType, Move } from '../../core';

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

  onMove(move: Move)
  {
    console.log(move);
  }
}
