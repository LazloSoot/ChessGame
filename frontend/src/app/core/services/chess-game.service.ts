import { Injectable } from "@angular/core";
import { GameSettings, User, GameOptions, Side } from "../models";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { Game } from "../models/chess/game";

@Injectable({
	providedIn: "root"
})
export class ChessGameService {
	private _apiUrl: string = "/games";
	private _gameSettings: GameSettings;
	private fen: string;
	constructor(private httpService: HttpService) {}

	public initializeGame(
		settings: GameSettings = new GameSettings()
	) {
		this._gameSettings = settings;
		if (this.isFenValid(settings.startFen)) {
			this.fen = settings.startFen;
		} else {
			throw new Error("Fen is not valid!");
		}
	}

	public isFenValid(fen: string) {
		if (fen) {
			return this.isFenValid;
		}
	}

	public get(id: number): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Get, this._apiUrl, id);
	}

	public createGame(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, this._apiUrl, undefined, game);
	}

	public joinGame(gameId: number): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Put, `${this._apiUrl}/${gameId}/join`);
	}
}
