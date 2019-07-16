import { Component, OnInit, Output } from "@angular/core";
import {
	BoardTextureType,
	PiecesTextureType,
	GameSettings,
	StyleOptions,
	BoardColor,
	PieceStyle,
	GameSide,
	OpponentType,
	User,
	UserService,
	AppStateService,
	Page
} from "../../../core";
import { EventEmitter } from "@angular/core";
import { GameOptions } from "../../../core/models/chess/gameSettings/gameOptions";
import { MatDialogRef } from "@angular/material";

@Component({
	selector: "app-new-game-dialog",
	templateUrl: "./new-game-dialog.component.html",
	styleUrls: ["./new-game-dialog.component.less"]
})
export class NewGameDialogComponent implements OnInit {
	@Output() onSettingsDefined: EventEmitter<GameSettings> = new EventEmitter<GameSettings>(null);
	public selectedTab: number = 0;
	
	private boardColors: BoardColor[];
	private boardColor: BoardColor = new BoardColor();
	private pieceStyles: PieceStyle[];
	private pieceStyle: PieceStyle = new PieceStyle();
	private isEnPassantOn: boolean = true;
	private side: GameSide = GameSide.Random;
	private opponentType: OpponentType = OpponentType.Computer;
	private opponent: User;

	private currentUser: User;

	constructor(
		private dialogRef: MatDialogRef<NewGameDialogComponent>,
		private appStateService: AppStateService
	) {}

	ngOnInit() {
		this.appStateService.getCurrentUserObs().subscribe((user) => {
			this.currentUser = user;
		});
		let tabHeader = document.getElementsByClassName("new-game-dialog__container")[0].getElementsByClassName("mat-tab-header")[0];
		tabHeader.classList.add("hidden");
		let keys = Object.keys(BoardTextureType);
		this.boardColors = Array(keys.length)
			.fill({})
			.map((x, i) => {
				return new BoardColor(BoardTextureType[keys[i]], keys[i]);
			});

		keys = Object.keys(PiecesTextureType);
		this.pieceStyles = Array(keys.length)
			.fill({})
			.map((x, i) => {
				return new PieceStyle(PiecesTextureType[keys[i]], keys[i]);
			});
	}

	submit() {
		this.onSettingsDefined.emit(
			new GameSettings(
				new StyleOptions(this.boardColor.value, this.pieceStyle.value),
				new GameOptions(
					this.isEnPassantOn,
					this.side,
					this.opponentType,
					this.opponent
				)
			)
		);
		this.dialogRef.close();
	}

	back() {
		if (this.selectedTab === 0) {
			this.onSettingsDefined.emit(undefined);
			this.dialogRef.close();
		} else {
			this.selectedTab = 0;
		}
	}

	getBoardUrlPrev() {
		return `${imgsUrl}${this.boardColor.value}/prev.jpg`;
	}

	getPieceUrlPrev() {
		return `${imgsUrl}${this.pieceStyle.value}/prev.png`;
	}

	removeOpponent() {
		this.opponent = undefined;
		this.opponentType = OpponentType.Computer;
	}


	selectUser(user: User) {
		debugger;
		if (user) {
			this.opponent = user;
			this.opponentType = OpponentType.Friend;
			this.selectedTab = 0;
		}
	}
}

const imgsUrl = "../../../../assets/images/Chess";
