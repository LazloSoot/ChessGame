import { Component, OnInit, Input, SimpleChange } from '@angular/core';
import { PieceType, PiecesTextureType, BoardTextureType } from '../../../core';
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-chess-board',
  templateUrl: './chess-board.component.html',
  styleUrls: ['./chess-board.component.less']
})
export class ChessBoardComponent implements OnInit {
  @Input() piecesTextureType: PiecesTextureType = PiecesTextureType.Symbols;
  @Input() boardTextureType: BoardTextureType = BoardTextureType.Wood;
  @Input() fen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
  private baseBoardPath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  private basePiecePath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
  private bgColor = "#813A0D";
  private squares;

  constructor() { }

  ngOnInit() {
    this.initSquares();
    this.basePiecePath.next(this.getPieceBasePath());
    this.baseBoardPath.next(imgsUrl + this.boardTextureType);
  }

  getSquaresCount() {
    return this.squares;
  }

  ngOnChanges(changes: SimpleChange) {
    for(let propName in changes){
      if(propName === 'piecesTextureType')
      {
        this.basePiecePath.next(imgsUrl + changes[propName].currentValue);
      }
      if(propName === 'boardTextureType')
      {
        this.baseBoardPath.next(imgsUrl + changes[propName].currentValue);
        let colorKey = Object.keys(BoardTextureType).find(key => BoardTextureType[key] === changes[propName].currentValue);
        if(colorKey && colors[colorKey])
        {
          this.bgColor = colors[colorKey];
        }
      }
    }
    
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

  getSquarePath(squareName) {
    return `${this.baseBoardPath.value}/` +  squareName + '.png';
  }

  getPiecePath(piece: PieceType) {
    return  this.basePiecePath.value + '/' + piece;
  }

  private getPieceBasePath(): string {
    return `${imgsUrl}` + this.piecesTextureType + 
    (this.boardTextureType == BoardTextureType.Wood) ? '/Wood' : '/Stone';
  }
}

const imgsUrl = '../../../../assets/images/Chess';
const colors = 
{
  "Wood" : "#813A0D",
  "StoneBlack" : "#181818",
  "StoneBlue" : "#43849D",
  "StoneGrey" : "#535352"
}