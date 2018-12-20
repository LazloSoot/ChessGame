import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-chess-board',
  templateUrl: './chess-board.component.html',
  styleUrls: ['./chess-board.component.less']
})
export class ChessBoardComponent implements OnInit {
  private squares;
  constructor() { }

  ngOnInit() {
    this.initSquares();
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
    return '../../../../assets/images/Chess/Board/Wood/' + squareName + '.png';
  }
}
