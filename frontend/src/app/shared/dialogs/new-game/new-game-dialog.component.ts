import { Component, OnInit, Output, ViewChild } from "@angular/core";
import {
	BoardTextureType,
	PiecesTextureType,
	GameSettings,
	StyleOptions,
	BoardColor,
	PieceStyle,
	GameSide,
	OpponentType,
	User
} from "../../../core";
import { EventEmitter } from "@angular/core";
import { GameOptions } from "../../../core/models/chess/gameSettings/gameOptions";
import { MatDialogRef, MatTableDataSource, MatPaginator } from "@angular/material";

@Component({
	selector: "app-new-game-dialog",
	templateUrl: "./new-game-dialog.component.html",
	styleUrls: ["./new-game-dialog.component.less"]
})
export class NewGameDialogComponent implements OnInit {
	@Output() onSettingsDefined: EventEmitter<GameSettings> = new EventEmitter<GameSettings>(null);
	private boardColors: BoardColor[];
	private boardColor: BoardColor = new BoardColor;
	private pieceStyles: PieceStyle[];
	private pieceStyle: PieceStyle = new PieceStyle;
	private isEnPassantOn: boolean = true;
	private isWhiteSide: boolean = true;
	private side: GameSide = GameSide.Random;
	private opponentType: OpponentType = OpponentType.Computer;
	private selectedTab: number = 0;
	private opponent: User;

	private users = new MatTableDataSource<User>(MOCK_USERS);
	
	constructor(
		private dialogRef: MatDialogRef<NewGameDialogComponent>,
	) {}

	ngOnInit() {
		let tabHeader = document.getElementsByClassName('mat-tab-header')[0];
		tabHeader.classList.add('hidden');
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
				new GameOptions(this.isEnPassantOn, this.isWhiteSide)
			)
		);
		this.dialogRef.close();
	}

	back() {
		if(this.selectedTab === 0) {
			this.onSettingsDefined.emit(undefined);
			this.dialogRef.close();
		}
		else {
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

	selectUser(user: User){
		if(user)
		{
			this.opponent = user;
			this.opponentType = OpponentType.Friend;
			this.selectedTab = 0;
		}
	}
}

const imgsUrl = "../../../../assets/images/Chess";

const MOCK_USERS: User[] = [
	{
		id: 0,
		uid: "",
		avatarUrl: "",
		name: "Grisha"
	},
	{
		id: 0,
		uid: "",
		avatarUrl: "",
		name: "Misha"
	},
	{
		id: 0,
		uid: "",
		avatarUrl: "",
		name: "Sasha"
	},
	{
		id: 0,
		uid: "",
		avatarUrl: "",
		name: "Pasha"
	}
] 