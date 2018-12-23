import { Injectable } from '@angular/core';
import { GameSettings } from '../models';

@Injectable({
  providedIn: 'root'
})
export class ChessGameService {
  private _gameSettings: GameSettings;
  private fen: string;
  constructor() {
   }

   public initializeGame(
     settings: GameSettings = new GameSettings(),
     fen: string = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
   )
   {
      this._gameSettings = settings;
      if(this.isFenValid(fen)) {
        this.fen = fen;
      } else {
        throw new Error("Fen is not valid!");
      }
   }

   public isFenValid(fen: string) {
     if(fen)
     {
       return this.isFenValid;
     }
   }
}
