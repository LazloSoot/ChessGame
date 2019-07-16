import { Component, OnInit } from '@angular/core';
import { User, GameSide, OpponentType } from '../../../core';

@Component({
  selector: 'app-new-game',
  templateUrl: './new-game.component.html',
  styleUrls: ['./new-game.component.less']
})
export class NewGameComponent implements OnInit {
  public selectedTab: number = 0;
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
}
