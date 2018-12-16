import { ChessGameRoutingModule } from './chess-game-routing.module';

describe('ChessGameRoutingModule', () => {
  let chessGameRoutingModule: ChessGameRoutingModule;

  beforeEach(() => {
    chessGameRoutingModule = new ChessGameRoutingModule();
  });

  it('should create an instance', () => {
    expect(chessGameRoutingModule).toBeTruthy();
  });
});
