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
	User,
	UserService
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
	private timeOutSearch: boolean = false;
	private isSearchMode: boolean = false;
	private filterInput: string;
	private users: User[] = [];
	
	constructor(
		private dialogRef: MatDialogRef<NewGameDialogComponent>,
		private userService: UserService
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
		this.users = [];
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

	filterChange(event){
		if (event.target.value.length > 0) {
            this.isSearchMode = true;
            this.users = [];
            if (!this.timeOutSearch) {
                this.timeOutSearch = true;
                setTimeout(() => {
                    this.filterInput = event.target.value;
                    this.timeOutSearch = false;
                    if (this.filterInput.length > 0) {

						this.userService
                            .getOnlineUsersByNameStartsWith(this.filterInput)
                            .then(users => {
                                this.users = users;
                                this.timeOutSearch = false;
                            });
                    }
                }, 1000);
            }
        } else {
            this.isSearchMode = false;
        }
	}

	resetFilterInput() {
		this.isSearchMode = false;
		this.users = [];
	}
	
	selectUser(user: User){
		if(user)
		{
			this.users = [];
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