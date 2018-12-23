import { Component, OnInit, Input, Output, SimpleChange, EventEmitter } from '@angular/core';
import { PieceType, PiecesTextureType, BoardTextureType, Square, Move, GameSettings } from '../../../core';
import { BehaviorSubject } from 'rxjs';

@Component({
	selector: 'app-chess-board',
	templateUrl: './chess-board.component.html',
	styleUrls: ['./chess-board.component.less']
})
export class ChessBoardComponent implements OnInit {
	@Input() gameSettings: GameSettings;// = new GameSettings();
	@Input() fen: string = 'rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1';
	@Output() error: EventEmitter<Error> = new EventEmitter<Error>(null);
	@Output() move: EventEmitter<Move> = new EventEmitter<Move>(null);
	private baseBoardPath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	private basePiecePath: BehaviorSubject<string> = new BehaviorSubject<string>(null);
	private bgColor = "#813A0D";
	private squares: Square[];
	private selectedSquare: Square;

	constructor() 
	{
	}

	ngOnInit() {
		//this.basePiecePath.next(this.getPieceBasePath());
		//this.baseBoardPath.next(imgsUrl + this.boardTextureType);
		//this.initSquares();
		//this.initBoard(this.fen);
	}

	getSquaresCount() {
		return this.squares;
	}

	ngOnChanges(changes: SimpleChange) {
		for (let propName in changes) {
			if(propName === 'fen') {
				this.initBoard(changes[propName].currentValue);
			}
			if (propName === 'gameSettings') {
				this.initSquares();
				this.initBoard(this.gameSettings.startFen);
				this.baseBoardPath.next(imgsUrl + this.gameSettings.style.boardColor);
				this.basePiecePath.next(this.getPieceBasePath());
				let colorKey = Object.keys(BoardTextureType).find(key => BoardTextureType[key] === this.gameSettings.style.boardColor);
				if (colorKey && colors[colorKey]) {
					this.bgColor = colors[colorKey];
				}
			}
		}

	}

	initSquares() {
		let currentIndex = 0;
		let increment;
		let currentRow;
		let correspondingCharCode;
		if(this.gameSettings.options.isWhiteSide) {
			currentRow = 8;
			increment = -1;
			correspondingCharCode = 97;
		} else {
			currentRow = 1;
			increment = 1;
			correspondingCharCode = 104;
		}

		this.squares = Array(64).fill({}).map((square, i) => {
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

	getSquareImgUrlExpression(square: Square) {
		let pieceUrl = (square.piece) ? `url(${this.getPiecePath(square.piece)}),` : '';
		let squareUrl = `url(${this.baseBoardPath.value}/${square.name}.png)`
		return `${pieceUrl}${squareUrl}`;
	}

	getPiecePath(piece: PieceType) {
		return this.basePiecePath.value + '/' + piece;
	}

	initBoard(fen: string) {
		let parts = fen.split(' ');
		if (parts.length < 6) {
			this.error.emit(new SyntaxError("Fen is not valid!"));
			return;
		}

		let lines = parts[0].split('/');
		let currentSkipCount: number;
		for (let y = 0; y < 8; y++) {
			for (let x = 0, currentFenX = 0; x < 8; x++) {
				currentSkipCount = Number(lines[y][currentFenX]);
				if (currentSkipCount) {
					currentFenX++;
					x += currentSkipCount - 1;
				}
				else {
					let a: keyof typeof PieceType = lines[y][currentFenX] as keyof typeof PieceType;
					if (!a) {
						this.error.emit(new SyntaxError(`Fen is not valid! '${lines[y][currentFenX]}' is not a valid piece notation. `));
						return;
					}
					currentFenX++;
					this.squares[y * 8 + x].piece = PieceType[a];
				}
			}
		}
	}

	selectSquare(square: Square) {
		if (!this.selectedSquare) {
			if (square.piece) {
				this.selectedSquare = square;
			}
			return;
		} else if (this.selectedSquare) {
			if (this.selectedSquare.name == square.name)
				{
					return;
				}

			let move = Object.keys(PieceType).find(key => PieceType[key] === this.selectedSquare.piece)[0]
			+ this.selectedSquare.name + square.name;
			this.move.emit(new Move(move));

			square.piece = this.selectedSquare.piece;
			this.selectedSquare.piece = undefined;
		}
		this.selectedSquare = null;
	}

	private getPieceBasePath(): string {
		return `${imgsUrl}${this.gameSettings.style.piecesStyle}` +
			((this.gameSettings.style.boardColor == BoardTextureType.Wood) ? '/Wood' : '/Stone')
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