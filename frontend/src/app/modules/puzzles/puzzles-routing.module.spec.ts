import { PuzzlesRoutingModule } from './puzzles-routing.module';

describe('PuzzlesRoutingModule', () => {
  let puzzlesRoutingModule: PuzzlesRoutingModule;

  beforeEach(() => {
    puzzlesRoutingModule = new PuzzlesRoutingModule();
  });

  it('should create an instance', () => {
    expect(puzzlesRoutingModule).toBeTruthy();
  });
});
