import { Injectable } from "@angular/core";
import { GameSettings, User, GameOptions, Side, MoveRequest, Move, PieceType, GameSide, OpponentType, SquareCoord } from "../models";
import { HttpService, RequestMethod } from "./http.service";
import { Observable, BehaviorSubject } from "rxjs";
import { Game } from "../models/chess/game";
import { MovesService } from "./moves.service";
import { AppStateService } from "./app-state.service";

@Injectable({
	providedIn: "root"
})
export class ChessGameService {
	private _apiUrl: string = "/games";
	private _gameSettings: GameSettings;
	private fen: string;
	private _isMyTurn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
	public get isMyTurnObs(): Observable<boolean> {
		return this._isMyTurn.asObservable();
	}

	public set isMyTurn(value: boolean) {
		this._isMyTurn.next(value);
	}

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
			const currentTurn = this.fen.split(' ')[1].trim().toLowerCase();
			this._isMyTurn.next((currentTurn === "w" && settings.options.selectedSide === GameSide.White)
				|| (currentTurn === "b" && settings.options.selectedSide === GameSide.Black));
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
		return this.httpService.sendRequest(RequestMethod.Post, this._apiUrl, undefined,undefined, game);
	}

	public createGameVersusAI(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, `${this._apiUrl}/ai`, undefined,undefined, game);
	}

	public createGameVersusRandPlayer(game: Game): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Post, `${this._apiUrl}/player`, undefined,undefined, game);
	}

	public joinGame(gameId: number): Observable<Game> {
		return this.httpService.sendRequest(RequestMethod.Put, `${this._apiUrl}/${gameId}/join`);
	}

	public commitMove(moveRequest: MoveRequest): Observable<Move> {
		return this.moveService.commitMove(moveRequest);
	}

	public GetAllValidMovesForFigureAt(squareName: string): Observable<string[]> {
		return this.httpService.sendRequest(RequestMethod.Get, `${this._apiUrl}/${this._gameSettings.gameId}/moves/available`, squareName);
	}

	public canISelectPiece(piece: PieceType): boolean {
		switch (this._gameSettings.options.opponentType) {
			case (OpponentType.Computer): {
				return true;
			}
			default: {
				return (this._isMyTurn.value && this.isItMyPiece(piece));
			}
		}
	}

	private isItMyPiece(piece: PieceType): boolean {
		const pieceName = piece.split('.')[0];
		return ((this._gameSettings.options.selectedSide === GameSide.White) && (pieceName[pieceName.length - 1] === 'W')) 
		|| ((this._gameSettings.options.selectedSide === GameSide.Black) && (pieceName[pieceName.length - 1] === 'B'));
	}
}
