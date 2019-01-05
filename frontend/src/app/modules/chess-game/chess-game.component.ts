import { Component, OnInit, OnDestroy } from "@angular/core";
import {
	ClientEvent,
	Move,
	ChessGameService,
	GameSettings,
	SignalRService,
	AppStateService,
	Group,
	Hub,
	GameSide,
	Invocation,
	ServerAction,
	StyleOptions,
	GameOptions,
	MovesService,
	MoveRequest,
	OpponentType
} from "../../core";
import { MatDialog, MatDialogConfig, MatDialogRef } from "@angular/material";
import {
	NewGameDialogComponent,
	InvitationDialogComponent,
	WaitingDialogComponent
} from "../../shared";
import { Game } from "../../core/models/chess/game";
import { Side } from "../../core/models/chess/side";
import { BehaviorSubject } from "rxjs";

@Component({
	selector: "app-chess-game",
	templateUrl: "./chess-game.component.html",
	styleUrls: ["./chess-game.component.less"]
})
export class ChessGameComponent implements OnInit {
	private gameSettings: GameSettings = new GameSettings();
	private game: Game;
	private fen: string =
		"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
	private isGameInitialized = false;
	private waitingDialog: MatDialogRef<WaitingDialogComponent>;
	private invitationDialog: MatDialogRef<InvitationDialogComponent>;
	private awaitedUserUid: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	constructor(
		private dialog: MatDialog,
		private chessGame: ChessGameService,
		private appStateService: AppStateService,
		private movesService: MovesService
	) {}

	ngOnInit() {
		this.subscribeSignalREvents();

		this.awaitedUserUid.subscribe((value) => {
			if(!value && this.waitingDialog)
			{
				this.waitingDialog.close();
			}
		});
	}

	ngOnDestroy() {
		this.appStateService.signalRConnection.off(
			ClientEvent.InvocationReceived
		);
	}

	onMove(move: Move) {
		console.log(move);
		debugger;
		//let request = new MoveRequest(move.moveNext, this.game.id);
		//this.movesService.commitMove(request);
		//.subscribe((move) => {
		//	if(move) {
		//		
		//	}
		//}, error => {
		//	this.fen = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
		//})
		//
	}

	private getRandomSide() {
		let rand = Math.random() * 100;
		let side = rand > 54 ? GameSide.Black : GameSide.White;
		return side;
	}

	private handleInvocation(invocation: Invocation) {
		this.invitationDialog = this.dialog.open(InvitationDialogComponent, {
			data: invocation
		});
		this.invitationDialog.afterClosed().subscribe(result => {
			if (result) {
						this.chessGame.joinGame(invocation.gameId).subscribe(game => {
							if (game) {
								const selectedSide = game.sides.find(s => s.player.uid === this.appStateService.getCurrentUser().uid);
								const gameOptions = new GameOptions(
									true,
									selectedSide.color,
									OpponentType.Player,
									game.sides.find(s => s.player.uid !== selectedSide.player.uid).player
								);
								this.gameSettings = new GameSettings(new StyleOptions(), gameOptions, game.fen);
								this.gameSettings.gameId = game.id;
								this.fen = game.fen;
								this.chessGame.initializeGame(this.gameSettings);
								this.isGameInitialized = true;
							} else {
								throw new Error("User has not joined to game.ERROR")
							}
						});
			} else {
				this.appStateService.signalRConnection.send(
					ServerAction.DismissInvocation,
					`${Group.User}${invocation.inviter.uid}`
				);
			}
		});
	}

	private openNewGameDialog() {
		const config: MatDialogConfig = {
			disableClose: true,
			closeOnNavigation: true
		};
		let dialogRef = this.dialog.open(NewGameDialogComponent, config);
		dialogRef.componentInstance.onSettingsDefined.subscribe(
			async (settings: GameSettings) => {
				if (settings) {
					if (settings.options.selectedSide === GameSide.Random) {
						settings.options.selectedSide = this.getRandomSide();
					}
					switch (settings.options.opponentType) {
						case (OpponentType.Player): {
							await this.createGameVersusRandPlayer(settings);
							break;
						}
						case (OpponentType.Friend): {
							await this.createGameWithFriend(settings);
							break;
						}
						default: {
							await this.createGameVersusComputer(settings);
							break;
						}
					}
					this.gameSettings = settings;
					this.chessGame.initializeGame(settings);
					this.isGameInitialized = true;
				} else {
					throw new Error("Game settings is invlid!ERROR")
				}
			}
		);
		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSettingsDefined.unsubscribe();
		});
	}

	private async createGameWithFriend(settings: GameSettings) {
		const config: MatDialogConfig = {
			disableClose: true,
			closeOnNavigation: true
		};
		this.awaitedUserUid.next(settings.options.opponent.uid);
		let sides: Side[] = [
			new Side(settings.options.selectedSide),
			new Side(
				settings.options.selectedSide ===
					GameSide.White
					? GameSide.Black
					: GameSide.White,
				settings.options.opponent
			)
		];
		let newGame = new Game(settings.startFen, sides);
		await this.chessGame
			.createGame(newGame)
			.subscribe(async game => {
				if (game) {
					this.gameSettings.gameId = game.id;
					this.waitingDialog = this.dialog.open(
						WaitingDialogComponent,
						config
					);
					this.waitingDialog.afterClosed()
						.subscribe(
							(isCanceled) => {
								debugger;
								if (isCanceled) {
									this.appStateService.signalRConnection
										.send(
											ServerAction.CancelInvocation,
											`${Group.User}${this.awaitedUserUid.value}`);
									this.awaitedUserUid.next(null);
								}
							})
				}
			});
	}

	private async createGameVersusRandPlayer(settings: GameSettings) {

	}

	private async createGameVersusComputer(settings: GameSettings) {

	}

	private subscribeSignalREvents() {
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationReceived,
			(invocation: Invocation) => {
				this.handleInvocation(invocation);
			}
		);
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationAccepted,
			(gameId) => {
				if(gameId && this.gameSettings && this.gameSettings.gameId === gameId)
				{
					this.awaitedUserUid.next(null);
					// вывод инфо о начале игры
					this.chessGame.get(gameId)
					.subscribe((game) => {
						this.game = game;
						this.gameSettings.startFen = game.fen;
						this.fen = game.fen;
					})
				}
			}
		);
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationDismissed,
			byUserWithUid => {
				debugger;
				if (
					this.awaitedUserUid.value &&
					byUserWithUid &&
					this.awaitedUserUid.value === byUserWithUid
				) {
					this.awaitedUserUid.next(null);
					// вывод инфо об отказе
				}
			}
		);
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationCanceled,
			() => {
				if(this.invitationDialog)
				{
					this.invitationDialog.close();
				}
			}
		);
	}

}
