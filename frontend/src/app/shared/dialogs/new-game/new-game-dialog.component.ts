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
	private boardColors: BoardColor[];
	private boardColor: BoardColor = new BoardColor();
	private pieceStyles: PieceStyle[];
	private pieceStyle: PieceStyle = new PieceStyle();
	private isEnPassantOn: boolean = true;
	private side: GameSide = GameSide.Random;
	private opponentType: OpponentType = OpponentType.Computer;
	private selectedTab: number = 0;
	private opponent: User;
	private timeOutSearch: boolean = false;
	private isSearchMode: boolean = false;
	private filterInput: string;
	private isOnlineUserFilterEnabled = false;
	private users: User[] = [];
	private currentUser: User;

	constructor(
		private dialogRef: MatDialogRef<NewGameDialogComponent>,
		private userService: UserService,
		private appStateService: AppStateService
	) {}

	ngOnInit() {
		this.appStateService.getCurrentUserObs().subscribe((user) => {
			this.currentUser = user;
		});
		let tabHeader = document.getElementsByClassName("mat-tab-header")[0];
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
		this.users = [];
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

	filterChange(event) {
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
							.getUsersByNameStartsWith(this.filterInput, this.isOnlineUserFilterEnabled, new Page(0, 10))
							.subscribe(users => {
								this.users = users.filter(u => u.uid !== this.currentUser.uid);
								this.timeOutSearch = false;
							});
					}
				}, 1000);
			}
		} else {
			this.isSearchMode = false;
		}
	}

	resetSearchInput() {
		this.isSearchMode = false;
		this.users = [];
	}

	selectUser(user: User) {
		if (user) {
			this.users = [];
			this.opponent = user;
			this.opponentType = OpponentType.Friend;
			this.selectedTab = 0;
		}
	}
}

const imgsUrl = "../../../../assets/images/Chess";
