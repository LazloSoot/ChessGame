import { Component, OnInit, OnDestroy } from "@angular/core";
import {
	Action,
	Move,
	ChessGameService,
	GameSettings,
	SignalRService,
	AppStateService,
	Group,
	Hub,
	GameSide
} from "../../core";
import { MatDialog, MatDialogConfig } from "@angular/material";
import { NewGameDialogComponent } from "../../shared";
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
	constructor(
		private dialog: MatDialog,
		private chessGame: ChessGameService,
		private appStateService: AppStateService
	) {}

	ngOnInit() {
		this.appStateService.signalRConnection.on(
			Action.InvocationReceived,
			invocation => {
				debugger;
				console.log('---------------------------------------------------------');
				console.log(invocation);
			}
		);
	}

	ngOnDestroy() {
		this.appStateService.signalRConnection.off(Action.InvocationReceived);
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
}
