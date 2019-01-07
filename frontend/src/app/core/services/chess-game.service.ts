import { Injectable } from "@angular/core";
import { GameSettings, User, GameOptions, Side, MoveRequest, Move, PieceType, GameSide, OpponentType } from "../models";
import { HttpService, RequestMethod } from "./http.service";
import { Observable } from "rxjs";
import { Game } from "../models/chess/game";
import { MovesService } from "./moves.service";

@Injectable({
	providedIn: "root"
})
export class ChessGameService {
	private _apiUrl: string = "/games";
	private _gameSettings: GameSettings;
	private fen: string;
	constructor(
		private httpService: HttpService,
		private moveService: MovesService
		) {}

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

	public createGameWithFriend(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, this._apiUrl, undefined, game);
	}

	public createGameVersusAI(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, `${this._apiUrl}/ai`, undefined, game);
	}

	public createGameVersusRandPlayer(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, `${this._apiUrl}/player`, undefined, game);
	}

	public joinGame(gameId: number): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Put, `${this._apiUrl}/${gameId}/join`);
	}

	public commitMove(moveRequest: MoveRequest): Observable<Move> {
		return this.moveService.commitMove(moveRequest);
	}

	public canISelectPiece(piece: PieceType): boolean {
		switch (this._gameSettings.options.opponentType) {
			case (OpponentType.Computer): {
				return true;
			}
			default: {
				const pieceName = piece.split('.')[0];
				return ((this._gameSettings.options.selectedSide === GameSide.White) && (pieceName[pieceName.length - 1] === 'W')) ||
					((this._gameSettings.options.selectedSide === GameSide.Black) && (pieceName[pieceName.length - 1] === 'B'));
			}
		}
	}
}
