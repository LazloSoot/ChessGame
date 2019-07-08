import { TestBed, inject } from '@angular/core/testing';

import { ChessGameService } from './chess-game.service';

describe('ChessGameService', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [ChessGameService]
    });
  });

  it('should be created', inject([ChessGameService], (service: ChessGameService) => {
    expect(service).toBeTruthy();
  }));
});
