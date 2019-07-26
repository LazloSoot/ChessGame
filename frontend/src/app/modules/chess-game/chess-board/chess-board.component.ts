import { Component, OnInit, Input, Output, SimpleChange, EventEmitter, OnDestroy, ViewChild, ElementRef } from '@angular/core';
import { PieceType, BoardTextureType, Square, Move, GameSettings, GameSide, MoveRequest, MovesService, ChessGameService, SquareCoord, AppStateService, ClientEvent, SignalRService, UserConnection, Group, Hub, Game, Side } from '../../../core';
import { BehaviorSubject, timer } from 'rxjs';

@Component({
	selector: 'app-chess-board',
	templateUrl: './chess-board.component.html',
	styleUrls: ['./chess-board.component.less']
})
export class ChessBoardComponent implements OnInit {
	@Input() gameSettings: GameSettings;
	@Input() boardSize: number;
	@Input() boardFlipped: boolean;
	@Output() error: EventEmitter<Error> = new EventEmitter<Error>(null);
	@Output() moveRequest: EventEmitter<Move> = new EventEmitter<Move>(null);
	@Output() check: EventEmitter<GameSide> = new EventEmitter<GameSide>(null);
	@Output() checkmate: EventEmitter<GameSide> = new EventEmitter<GameSide>(null);
	public baseBoardPath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	public basePiecePath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	private _signalRConnection: UserConnection;
	private currentFen: string;// = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
	private previousFen: string;
	public bgColor = "#813A0D";
	private _squares: Square[];
	private selectedSquare: Square;
	private availableMoves: string[] = [];
	private lastMove: LastMove;
	private readonly minBoardSize = 310;
	private readonly maxBoardSize = 500;

	get squares(): Square[] {
		return this._squares;
	}

	constructor(
		public signalRService: SignalRService,
		public appStateService: AppStateService,
		public chessGameService: ChessGameService
	) { }

	ngOnInit() {
	}

	ngOnDestroy() {
	}

	ngOnChanges(changes: SimpleChange) {
		for (let propName in changes) {
			if (propName === 'gameSettings') {
				if(changes[propName].previousValue) {
					const isOptionsChanged: boolean = changes[propName].currentValue.options && changes[propName].previousValue.options !== changes[propName].currentValue.options;
					const isFenChanged: boolean = changes[propName].currentValue.startFen && changes[propName].previousValue.startFen !== changes[propName].currentValue.startFen;
					const isStylesChanged: boolean = changes[propName].currentValue.style;
					if (isOptionsChanged || isFenChanged) {
						this.initNewGame(changes[propName].previousValue.gameId);
					}
					if (isStylesChanged) {
						this.initNewBoardStyles();
					}
				} else {
					this.initNewGame();
					this.initNewBoardStyles();
				}

			} else if (propName === "boardSize") {
				if (changes[propName].currentValue < this.minBoardSize) {
					this.boardSize = this.minBoardSize;
				}
				else if (changes[propName].currentValue > this.maxBoardSize) {
					this.boardSize = this.maxBoardSize;
				}
			} else if (propName === "boardFlipped" && !changes[propName].firstChange) {
				this.initSquares();
				// initBoard push new prevFen and currentFen values, so we need avoid it
				const prevFenTemp = this.previousFen;
				const currentFenTemp = this.currentFen;
				// getChangedLinesIndexes cheks if fen was changed, so we need change currentFen
				this.currentFen = "";
				this.initBoard(currentFenTemp);
				this.previousFen = prevFenTemp;
			}
		}

	}

	initNewGame(previousGameId?: number) {
		this.previousFen = "";
		this.currentFen = "";
		this.lastMove = null;
		this.initSquares();
		this.initBoard(this.gameSettings.startFen);
		this.previousFen = this.gameSettings.startFen;
		let signalRPipelinePromise: Promise<void>;
		if (previousGameId && this._signalRConnection) {
			signalRPipelinePromise = new Promise((resolve, reject) => {
				setTimeout(() => {
					this._signalRConnection.offAll();
					this._signalRConnection.leaveGroup(`${Group.Game}${previousGameId}`);
					resolve();
				}, 100);
			});
		} else {
			signalRPipelinePromise = new Promise((resolve, reject) => {
				setTimeout(() => {
					resolve();
				}, 100);
			});
		}
		this.chessGameService.initializeGame(this.gameSettings);
		signalRPipelinePromise.then(() => {
			this._signalRConnection = this.signalRService.connect(
				`${Group.Game}${this.gameSettings.gameId}`,
				Hub.ChessGame,
				this.appStateService.token
			);
			this.subscribeSignalREvents();
		},
			e => {
				debugger;
			});

		signalRPipelinePromise.catch((e) => {
			debugger;
		});
	}

	initNewBoardStyles() {
		this.baseBoardPath.next(imgsUrl + this.gameSettings.style.boardColor);
		this.basePiecePath.next(this.getPieceBasePath());
		let colorKey = Object.keys(BoardTextureType).find(key => BoardTextureType[key] === this.gameSettings.style.boardColor);
		if (colorKey && colors[colorKey]) {
			this.bgColor = colors[colorKey];
		}
	}

	initSquares() {
		let currentIndex = 0;
		let increment;
		let currentRow;
		let correspondingCharCode;
		if ((this.gameSettings.options.selectedSide === GameSide.White && !this.boardFlipped) || (this.gameSettings.options.selectedSide === GameSide.Black && this.boardFlipped)) {
			currentRow = 8;
			increment = -1;
			correspondingCharCode = 97;
		} else {
			currentRow = 1;
			increment = 1;
			correspondingCharCode = 104;
		}

		this._squares = Array(64).fill({}).map((square, i) => {
			currentIndex = i % 8;
			square = {
				name: String.fromCharCode(correspondingCharCode - currentIndex * increment) + currentRow,
				piece: undefined
			};
			if (currentIndex === 7) {
				currentRow += increment;
			}
			return square;
		}
		);
	}

	initBoard(fen: string) {
		const parts = fen.split(' ');
		if (parts.length < 6) {
			this.error.emit(new SyntaxError("Fen is not valid!"));
			return;
		}

		const lines = parts[0].split('/');
		let baseNum;
		if ((this.gameSettings.options.selectedSide === GameSide.White && !this.boardFlipped) || (this.gameSettings.options.selectedSide === GameSide.Black && this.boardFlipped)) {
			baseNum = 0;
		}
		else {
			baseNum = 63;
		}
		const changedLinesIndexes = this.getChangedLinesIndexes(lines);
		for (let y = 0, currentIndex = 0; y < changedLinesIndexes.length; y++) {
			currentIndex = changedLinesIndexes[y];
			this.initLine(currentIndex, lines[currentIndex], baseNum);
		}
		this.previousFen = this.currentFen;
		this.currentFen = fen;
		const currentTurnSide = parts[1].trim().toLowerCase();
		if ((currentTurnSide === 'w' && this.gameSettings.options.selectedSide === GameSide.White) ||
			currentTurnSide === 'b' && this.gameSettings.options.selectedSide === GameSide.Black) {
			this.chessGameService.isMyTurn = true;
		}
		else {
			this.chessGameService.isMyTurn = false;
		}
	}

	initLine(lineIndex: number, fenPart: string, baseNum: number) {
		console.log("Line " + lineIndex + " initing");
		let currentSkipCount: number;
		for (let x = 0, currentFenX = 0; x < 8; x++) {
			currentSkipCount = Number(fenPart[currentFenX]);
			if (currentSkipCount) {
				currentFenX++;

				for (; currentSkipCount > 0; currentSkipCount--) {
					this._squares[Math.abs(baseNum - (lineIndex * 8 + x))].piece = undefined;
					x++;
				}
				x--;
				//x += currentSkipCount - 1;
			}
			else {
				let pieceKey: keyof typeof PieceType = fenPart[currentFenX] as keyof typeof PieceType;
				if (!pieceKey) {
					this.error.emit(new SyntaxError(`Fen is not valid! '${fenPart[currentFenX]}' is not a valid piece notation. `));
					return;
				}
				currentFenX++;
				this._squares[Math.abs(baseNum - (lineIndex * 8 + x))].piece = PieceType[pieceKey];
			}
		}
	}

	getChangedLinesIndexes(lines: string[]): number[] {
		if (this.currentFen) {
			const currentLines = this.currentFen.split(' ')[0].split('/');
			let changedLinesIndexes: number[] = [];
			for (let i = 0; i < currentLines.length; i++) {
				if (currentLines[i] !== lines[i]) {
					// console.log("Line " + i + " changed");
					// console.log("Was " + currentLines[i]);
					// console.log("Is " + lines[i]);
					changedLinesIndexes.push(i);
				}
			}
			return changedLinesIndexes;
		} else {
			return [0, 1, 2, 3, 4, 5, 6, 7];
		}
	}

	async selectSquare(square: Square) {
		console.log("GAMEID  " + this.gameSettings.gameId);
		if (!this.selectedSquare) {
			if (square.piece && this.chessGameService.canISelectPiece(square.piece)) {
				this.selectedSquare = square;
				this.chessGameService.GetAllValidMovesForFigureAt(square.name)
					.subscribe((availableMoves) => this.highlightMoves(this.selectedSquare, availableMoves), error => { });
			}
			return;
		} else {
			if (this.selectedSquare.name == square.name) {
				this.selectedSquare = null;
				this.availableMoves = [];
				return;
			}

			const move = Object.keys(PieceType).find(key => PieceType[key] === this.selectedSquare.piece)[0]
				+ this.selectedSquare.name + square.name;
			//this.moveRequest.emit(new MoveRequest(move, this.gameSettings.gameId));

			// let fenParts = this.fen.split(' ');
			// let fenLines = fenParts[0].split('/');
			// debugger;
			// let i = Math.abs(8 - Number(this.selectedSquare.name[1]));
			// fenLines[i] = '-';
			// i = Math.abs(8 - Number(square.name[1]));
			// fenLines[i] = '-';
			// fenParts[0] = fenLines.join('/');
			// this.previousFen = fenParts.join(' ');

			await this.tryMove(new MoveRequest(move, this.gameSettings.gameId));
			this.availableMoves = [];
			//square.piece = this.selectedSquare.piece;
			//this.selectedSquare.piece = undefined;
		}
		this.selectedSquare = null;
	}

	async tryMove(moveRequest: MoveRequest) {
		await this.chessGameService.commitMove(moveRequest)
			.toPromise()
			.then((move) => {
				if (move) {
					this.initBoard(move.fenAfterMove);
					this.availableMoves = [];
					this.lastMove = new LastMove(move.moveNext.slice(1, 3), move.moveNext.slice(3, 5));
					this.moveRequest.emit(move);
				}
			}, error => {

			});
	}

	highlightMoves(targetSquare: Square, availableMoves: string[]) {
		if (this.selectedSquare === targetSquare) {
			this.availableMoves = availableMoves;
		} else {
			this.availableMoves = [];
		}
	}

	getSquareImgUrlExpression(square: Square) {
		const pieceUrl = (square.piece) ? `url(${this.getPiecePath(square.piece)}),` : '';
		const squareUrl = `url(${this.baseBoardPath.value}/${square.name}.png)`
		return `${pieceUrl}${this.getSquareMask(square)}${squareUrl}`;
	}

	getSquareMask(square: Square): string {
		if (this.lastMove) {
			if (square.name === this.lastMove.to) {
				return `url(${this.getLastMoveToMaskUrl()}),`;
			} else if (square.name === this.lastMove.from) {
				return `url(${this.getLastMoveFromMaskUrl()}),`;
			}
		}
		if (this.availableMoves.find(m => m === square.name)) {
			return (square.piece) ? `url(${this.getAvailableKillMaskUrl()}),` : `url(${this.getAvailableMoveMaskUrl()}),`;
		} else {
			return (this.selectedSquare && (square.name === this.selectedSquare.name)) ? `url(${this.getSquareSelectionMaskUrl()}),` : '';
		}
	}

	getPiecePath(piece: PieceType) {
		return this.basePiecePath.value + '/' + piece;
	}

	private getSquareSelectionMaskUrl(): string {
		return `${imgsUrl}/Effects/SelectedSquare/triangle_in.png`;
	}

	private getAvailableMoveMaskUrl(): string {
		return `${imgsUrl}/Effects/SelectedSquare/full_green.png`;
	}

	private getAvailableKillMaskUrl(): string {
		return `${imgsUrl}/Effects/SelectedSquare/full_red.png`;
	}

	private getLastMoveFromMaskUrl(): string {
		return `${imgsUrl}/Effects/SelectedSquare/full_blue.png`;
	}

	private getLastMoveToMaskUrl(): string {
		return `${imgsUrl}/Effects/SelectedSquare/full_blue-bright.png`;
	}

	private getPieceBasePath(): string {
		return `${imgsUrl}${this.gameSettings.style.piecesStyle}` +
			((this.gameSettings.style.boardColor == BoardTextureType.Wood) ? '/Wood' : '/Stone')
	}

	private subscribeSignalREvents() {
		this._signalRConnection.onMoveComitted(
			async () => {
				await this.refreshBoard();
			}
		);

		this._signalRConnection.onCheck(
			(checkTo: GameSide) => {
				this.doCheck(checkTo);
			}
		);

		this._signalRConnection.onMate(
			(mateTo: GameSide) => {
				this.doMate(mateTo);
			}
		)
	}

	private async refreshBoard() {
		await this.chessGameService.get(this.gameSettings.gameId)
			.toPromise()
			.then((game: Game) => {
				if (game) {
					if (this.currentFen !== game.fen) {
						const lastMove = game.moves.sort((a, b) => b.id - a.id)[0];
						this.lastMove = new LastMove(lastMove.moveNext.slice(1, 3), lastMove.moveNext.slice(3, 5));
						this.moveRequest.emit(lastMove);
						this.initBoard(game.fen);
					}
				}
			});
	}

	private doCheck(checkTo: GameSide) {
		if (this.gameSettings.options.selectedSide === checkTo) {
			this.check.emit(checkTo);
			// highlight king square
		}
	}

	private doMate(mateTo: GameSide) {
		this.checkmate.emit(mateTo);
		// highlight king square
	}
}

const imgsUrl = '../../../../assets/images/Chess';
const colors =
{
	"Wood": "#813A0D",
	"StoneBlack": "#181818",
	"StoneBlue": "#43849D",
	"StoneGrey": "#535352"
}

export class LastMove {
	constructor(public from: string, public to: string) { }
}