import { Component, OnInit, Input, SimpleChange, Output, EventEmitter } from '@angular/core';
import { Move,User, ChessGameService } from '../../../core';

@Component({
  selector: 'app-move-tracker',
  templateUrl: './move-tracker.component.html',
  styleUrls: ['./move-tracker.component.less']
})
export class MoveTrackerComponent implements OnInit {
  @Input() moves: Move[];
  @Input() opponent: User;
  @Input() isMovesSwitchOn: boolean;
  @Output() onResign: EventEmitter<boolean> = new EventEmitter<boolean>(null);
  @Output() onDraw: EventEmitter<boolean> = new EventEmitter<boolean>(null);
  private isOpponentTurn: boolean;
  public fullMoves: FullMove[];
  constructor(
    private chessGameService: ChessGameService
  ) { }

  ngOnInit() {
    this.chessGameService.isMyTurnObs.subscribe(isMyTurn => {
      this.isOpponentTurn = !isMyTurn;
    });
  }

  ngOnChanges(changes: SimpleChange) {
    for (let propName in changes) {
      if (propName === 'moves') {
        if (!this.moves) {
          this.fullMoves = [];
          return;
        }
        
        let castlingMoveIndex = this.moves.findIndex(m => m.moveNext === "Ke1g1");
        if (castlingMoveIndex > -1)
          this.moves[castlingMoveIndex].moveNext = "0-0";
        castlingMoveIndex = this.moves.findIndex(m => m.moveNext === "ke8g8");
        if (castlingMoveIndex > -1)
          this.moves[castlingMoveIndex].moveNext = "0-0";
        castlingMoveIndex = this.moves.findIndex(m => m.moveNext === "Ke1c1");
        if (castlingMoveIndex > -1)
          this.moves[castlingMoveIndex].moveNext = "0-0-0";
        castlingMoveIndex = this.moves.findIndex(m => m.moveNext === "ke8c8");
        if (castlingMoveIndex > -1)
          this.moves[castlingMoveIndex].moveNext = "0-0-0";

        this.fullMoves = [].concat.apply([],
          this.moves.map((move, index, moves) => {
            return index % 2 ? [] : new FullMove(moves[index], moves[index + 1])
          }));
      }
    }
  }

  getMovesCount() {
    if(this.moves) {
      return new Array(Math.round(this.moves.length / 2));
    }
  }

  getOpponentAvatarUrl() {
    return (this.opponent.avatarUrl) ? this.opponent.avatarUrl : '../../../../assets/images/anonAvatar.png' ;
  }

  public draw() {
    this.onDraw.emit(true);
  }

  public resign() {
    this.onResign.emit(true);
  }
}

export class FullMove {
  constructor(public ply: Move, public ply2?: Move) {}
}