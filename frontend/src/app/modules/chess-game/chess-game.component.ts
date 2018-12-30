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
	ServerAction
} from "../../core";
import { MatDialog, MatDialogConfig, MatDialogRef } from "@angular/material";
import {
	NewGameDialogComponent,
	InvitationDialogComponent,
	WaitingDialogComponent
} from "../../shared";
import { Game } from "../../core/models/chess/game";
import { Side } from "../../core/models/chess/side";

@Component({
	selector: "app-chess-game",
	templateUrl: "./chess-game.component.html",
	styleUrls: ["./chess-game.component.less"]
})
export class ChessGameComponent implements OnInit {
	private gameSettings: GameSettings = new GameSettings();
	private fen: string =
		"rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";
	private isGameInitialized = false;
	private waitingDialog: MatDialogRef<WaitingDialogComponent>;
	constructor(
		private dialog: MatDialog,
		private chessGame: ChessGameService,
		private appStateService: AppStateService
	) {}

	ngOnInit() {
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationReceived,
			(invocation: Invocation) => {
				this.handleInvocation(invocation);
			}
		);
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationAccepted,
			() => {
				this.waitingDialog.close();
				// вывод инфо о начале игры
			}
		);
		this.appStateService.signalRConnection.on(
			ClientEvent.InvocationDismissed,
			() => {
				this.waitingDialog.close();
				// вывод инфо об отказе
			}
		);
	}

	ngOnDestroy() {
		this.appStateService.signalRConnection.off(
			ClientEvent.InvocationReceived
		);
	}

	ngAfterViewInit() {
		setTimeout(() => {
			let config: MatDialogConfig = {
				disableClose: true,
				closeOnNavigation: true
			};
			let dialogRef = this.dialog.open(NewGameDialogComponent, config);
			dialogRef.componentInstance.onSettingsDefined.subscribe(
				(settings: GameSettings) => {
					if (settings) {
						this.chessGame.initializeGame(settings);
						if (settings.options.selectedSide === GameSide.Random) {
							settings.options.selectedSide = this.getRandomSide();
						}
						if (settings.options.opponent) {
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
							this.chessGame
								.createGame(newGame)
								.subscribe(game => {
									if (game) {
										this.gameSettings.gameId = game.id;
										this.waitingDialog = this.dialog.open(
											WaitingDialogComponent,
											config
										);
										// открыть dialog и ждать пользователя
									}
								});
						}
						this.gameSettings = settings;
						this.isGameInitialized = true;
					} else {
					}
				}
			);
			dialogRef.afterClosed().subscribe(() => {
				dialogRef.componentInstance.onSettingsDefined.unsubscribe();
			});
		}, 50);
	}

	onMove(move: Move) {
		console.log(move);
	}

	private getRandomSide() {
		let rand = Math.random() * 100;
		let side = rand > 54 ? GameSide.Black : GameSide.White;
		return side;
	}

	private handleInvocation(invocation: Invocation) {
		let dialogRef = this.dialog.open(InvitationDialogComponent, {
			data: invocation
		});
		dialogRef.afterClosed().subscribe(result => {
			if (result) {
				this.chessGame.get(invocation.gameId).subscribe(game => {
					if (game) {
						let inviterSide = game.sides.find(
							s => s.player.uid === invocation.inviter.uid
						);
						let side = new Side(
							inviterSide.color === GameSide.White
								? GameSide.Black
								: GameSide.White,
							this.appStateService.getCurrentUser()
						);
						side.gameId = game.id;

						this.chessGame.joinGame(side).subscribe(side => {
							if (game) {
								console.log("game ready");
							} else {
								console.log("user does not join to game.ERROR");
							}
						});
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
}
