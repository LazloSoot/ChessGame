import { Component, OnInit, Input } from '@angular/core';
import { PieceType, PiecesTextureType, BoardTextureType } from '../../../core';

@Component({
  selector: 'app-chess-board',
  templateUrl: './chess-board.component.html',
  styleUrls: ['./chess-board.component.less']
})
export class ChessBoardComponent implements OnInit {
  @Input() piecesTextureType: PiecesTextureType = PiecesTextureType.Symbols;
  @Input() boardTextureType: BoardTextureType = BoardTextureType.Wood;
  private basePiecePath: string;
  private squares;
  constructor() { }

  ngOnInit() {
    this.initSquares();
    this.basePiecePath = this.getPieceBasePath();
  }

  getSquaresCount() {
    return this.squares;
  }

  initSquares(){
    let currentIndex = 0;
    let currentRow = 8;
    this.squares = Array(64).fill({}).map((x,i)=> 
    {
      currentIndex = i % 8;
      x = {
        name: String.fromCharCode(97 + currentIndex) + currentRow
      };
      if(currentIndex === 7)
      {
        currentRow--;
      }
      return x;
    }
    );
  }

  getImagePath(squareName) {
    return `${imgsUrl}${this.boardTextureType}/` +  squareName + '.png';
  }

  getPiecePath(piece: PieceType) {
    return  this.basePiecePath + '/' + piece;
  }

  private getPieceBasePath(): string {
    return `${imgsUrl}` + this.piecesTextureType + 
    (this.boardTextureType == BoardTextureType.Wood) ? '/Wood' : '/Stone';
  }
}

const imgsUrl = '../../../../assets/images/Chess';