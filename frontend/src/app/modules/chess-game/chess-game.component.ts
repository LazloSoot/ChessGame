import { Component, OnInit, OnDestroy } from '@angular/core';
import { BoardTextureType, PiecesTextureType, Move, ChessGameService, GameSettings, SignalRService, AppStateService, Group, Hub } from '../../core';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { NewGameDialogComponent } from '../../shared';

@Component({
	selector: 'app-chess-game',
	templateUrl: './chess-game.component.html',
	styleUrls: ['./chess-game.component.less']
})
export class ChessGameComponent implements OnInit, OnDestroy {
	private gameSettings: GameSettings = new GameSettings();
	private fen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
	private isGameInitialized = false;
	private signalRConnection;
	constructor(
		private dialog: MatDialog,
		private chessGame: ChessGameService,
		private signalRService: SignalRService,
		private appStateService: AppStateService
	) { }

	ngOnInit() {
		this.signalRConnection = this.signalRService.connect(
			`${Group[Group.PlayRoom]}`,
			Hub.ChessGame
		);
		
	}

	ngOnDestroy() {
		this.signalRService.leaveGroup(
		`${Group[Group.PlayRoom]}`,
		Hub.ChessGame
		);
	}

	ngAfterViewInit() {
		setTimeout(() => {
			let config :  MatDialogConfig = {
				disableClose: true,
				closeOnNavigation: true
			};
			let dialogRef = this.dialog.open(NewGameDialogComponent, config);
		dialogRef.componentInstance.onSettingsDefined
		.subscribe(
			(settings) => {
				if(settings)
				{
					this.chessGame.initializeGame(settings);
					this.gameSettings = settings;
					this.isGameInitialized = true;
				}else {

				}
		});
		dialogRef.afterClosed().subscribe(() => {
			dialogRef.componentInstance.onSettingsDefined.unsubscribe();
		});
		}, 50);
	}

	onMove(move: Move) {
		console.log(move);
	}
}
