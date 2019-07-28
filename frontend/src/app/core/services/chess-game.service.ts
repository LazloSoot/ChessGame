import { Injectable, EventEmitter } from "@angular/core";
import {
	GameSettings,
	User,
	GameOptions,
	Side,
	MoveRequest,
	Move,
	PieceType,
	GameSide,
	OpponentType,
	SquareCoord
} from "../models";
import { HttpService, RequestMethod } from "./http.service";
import { Observable, BehaviorSubject, of } from "rxjs";
import { Game } from "../models/chess/game";
import { MovesService } from "./moves.service";
import { AppStateService } from "./app-state.service";

@Injectable({
	providedIn: "root"
})
export class ChessGameService {
	private _apiUrl: string = "/games";
	private _isMyTurn: BehaviorSubject<boolean> = new BehaviorSubject<boolean>(null);
	public get isMyTurnObs(): Observable<boolean> {
		return this._isMyTurn.asObservable();
	}

	public get isMyTurn() {
		return this._isMyTurn.value;
	}

	constructor(
		private httpService: HttpService,
		private moveService: MovesService,
		private appStateService: AppStateService
	) {}

	/// move game logic there from views! this object shouldnt get fen as param, it should get updated data
	// from server instead!
	public initializeGame(fen: string) {
		if (this.isFenValid(fen)) {
			const currentTurn = fen
				.split(" ")[1]
				.trim()
				.toLowerCase();
				
				setTimeout(() => {
					this._isMyTurn.next(
						(currentTurn === "w" &&
						this.appStateService.currentGame.options.selectedSide === GameSide.White) ||
							(currentTurn === "b" &&
							this.appStateService.currentGame.options.selectedSide === GameSide.Black)
					);
				});
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
		if (id)
			return this.httpService.sendRequest(
				RequestMethod.Get,
				this._apiUrl,
				id
			);
		else return of();
	}

	public createGameWithFriend(game: Game): Observable<Game> {
		return this.httpService.sendRequest(
			RequestMethod.Post,
			this._apiUrl,
			undefined,
			undefined,
			game
		);
	}

	public createGameVersusAI(game: Game): Observable<Game> {
		return this.httpService.sendRequest(
			RequestMethod.Post,
			`${this._apiUrl}/ai`,
			undefined,
			undefined,
			game
		);
	}

	public createGameVersusRandPlayer(game: Game): Observable<Game> {
		return this.httpService.sendRequest(
			RequestMethod.Post,
			`${this._apiUrl}/player`,
			undefined,
			undefined,
			game
		);
	}

	public joinGame(gameId: number): Observable<Game> {
		return this.httpService.sendRequest(
			RequestMethod.Put,
			`${this._apiUrl}/${gameId}/join`
		);
	}

	public commitMove(moveRequest: MoveRequest): Promise<Move> {
		return this.moveService.commitMove(moveRequest)
		.toPromise()
		.then((move)=> {
			if(move) {
				const parts = move.fenAfterMove.split(' ');
				const currentTurnSide = parts[1].trim().toLowerCase();
				if ((currentTurnSide === 'w' && this.appStateService.currentGame.options.selectedSide === GameSide.White) ||
					currentTurnSide === 'b' && this.appStateService.currentGame.options.selectedSide === GameSide.Black) {
					this._isMyTurn.next(true);
				}
				else {
					this._isMyTurn.next(false);
				}
			}
			return move;
		});
	}

	public GetAllValidMovesForFigureAt(
		squareName: string
	): Observable<string[]> {
		return this.httpService.sendRequest(
			RequestMethod.Get,
			`${this._apiUrl}/${this.appStateService.currentGame.gameId}/moves/available`,
			squareName
		);
	}

	public canISelectPiece(piece: PieceType): boolean {
		switch (this.appStateService.currentGame.options.opponentType) {
			case OpponentType.Computer: {
				return true;
			}
			default: {
				return this._isMyTurn.value && this.isItMyPiece(piece);
			}
		}
	}

	private isItMyPiece(piece: PieceType): boolean {
		const pieceName = piece.split(".")[0];
		return (
			(this.appStateService.currentGame.options.selectedSide === GameSide.White &&
				pieceName[pieceName.length - 1] === "W") ||
			(this.appStateService.currentGame.options.selectedSide === GameSide.Black &&
				pieceName[pieceName.length - 1] === "B")
		);
	}
}
