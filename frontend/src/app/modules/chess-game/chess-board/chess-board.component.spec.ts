import { ChessBoardComponent } from './chess-board.component';

describe('ChessBoardComponent', () => {
  let chessBoardComponent: ChessBoardComponent;

  beforeEach(() => {
    chessBoardComponent = new ChessBoardComponent();
  });

  it('should create an instance', () => {
    expect(chessBoardComponent).toBeTruthy();
  });
});
