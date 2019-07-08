import { PuzzlesModule } from './puzzles.module';

describe('PuzzlesModule', () => {
  let puzzlesModule: PuzzlesModule;

  beforeEach(() => {
    puzzlesModule = new PuzzlesModule();
  });

  it('should create an instance', () => {
    expect(puzzlesModule).toBeTruthy();
  });
});
