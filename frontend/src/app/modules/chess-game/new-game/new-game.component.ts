import { Component, OnInit, Output, EventEmitter } from '@angular/core';
import { User, GameSide, OpponentType, GameOptions } from '../../../core';

@Component({
  selector: 'app-new-game',
  templateUrl: './new-game.component.html',
  styleUrls: ['./new-game.component.less']
})
export class NewGameComponent implements OnInit {
  @Output() onStart: EventEmitter<GameOptions> = new EventEmitter();
  public selectedTab: number = 0;
  public isEnPassantOn = true;
  private opponent: User;
  private side: GameSide = GameSide.Random;
  private opponentType: OpponentType = OpponentType.Computer;
  constructor() { }

  ngOnInit() {
  }

  ngAfterViewChecked() {
    let tabHeader = document
    .getElementsByClassName("new-game__container")[0]
    .getElementsByClassName("mat-tab-header")[0];
		tabHeader.classList.add("hidden");
  }

  selectUser(user: User) {
		if (user) {
			this.opponent = user;
			this.selectedTab = 0;
		}
  }

  removeOpponent() {
		this.opponent = undefined;
		this.opponentType = OpponentType.Computer;
  }
  
  start() {
    this.onStart.emit(new GameOptions(
      this.isEnPassantOn,
      this.side,
      this.opponentType,
      this.opponent
    ));
  }
}
