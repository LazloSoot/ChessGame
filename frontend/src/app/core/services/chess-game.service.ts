import { Injectable } from "@angular/core";
import { GameSettings, User, GameOptions } from "../models";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { Game } from "../models/chess/game";

@Injectable({
	providedIn: "root"
})
export class ChessGameService {
	private apiUrl: string = "/games";
	private _gameSettings: GameSettings;
	private fen: string;
	constructor(private httpService: HttpService) {}

	public initializeGame(
		settings: GameSettings = new GameSettings(),
		fen: string = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1"
	) {
		this._gameSettings = settings;
		if (this.isFenValid(fen)) {
			this.fen = fen;
		} else {
			throw new Error("Fen is not valid!");
		}
	}

	public isFenValid(fen: string) {
		if (fen) {
			return this.isFenValid;
		}
	}

	public createGame(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, this.apiUrl, undefined, game);
	}
}
