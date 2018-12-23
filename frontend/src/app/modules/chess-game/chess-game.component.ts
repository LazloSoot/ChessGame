import { Component, OnInit } from '@angular/core';
import { BoardTextureType, PiecesTextureType, Move, ChessGameService, GameSettings } from '../../core';
import { MatDialog, MatDialogConfig } from '@angular/material';
import { GameSettingsDialogComponent } from '../../shared';

@Component({
	selector: 'app-chess-game',
	templateUrl: './chess-game.component.html',
	styleUrls: ['./chess-game.component.less']
})
export class ChessGameComponent implements OnInit {
	private gameSettings: GameSettings = new GameSettings();
	private fen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
	private isGameInitialized = false;
	constructor(
		private dialog: MatDialog,
		private chessGame: ChessGameService
	) { }

	ngOnInit() {
		
		
	}

	ngAfterViewInit() {
		setTimeout(() => {
			let config :  MatDialogConfig = {
				disableClose: true,
				closeOnNavigation: true
			};
			let dialogRef = this.dialog.open(GameSettingsDialogComponent, config);
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
