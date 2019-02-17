import { Component, OnInit, Input, SimpleChanges, ViewChild, Output, EventEmitter, AfterViewInit } from "@angular/core";
import { GameWithConclution, PagedResult, Game, UserService, Page } from "../../../core";
import { MatPaginator, MatTableDataSource } from "@angular/material";
import { merge, of, BehaviorSubject } from "rxjs";
import { startWith, switchMap, map, catchError } from "rxjs/operators";

@Component({
	selector: "app-games-history",
	templateUrl: "./games-history.component.html",
	styleUrls: ["./games-history.component.less"]
})
export class GamesHistoryComponent implements OnInit {
  @Input() targetUserId: number;
  @ViewChild(MatPaginator) paginator:MatPaginator;
  private isGamesLoading: boolean = true;
  private isInitialized: boolean = false;
  private isNoGamesToShow: boolean = true;
  private games: GameWithConclution[] = [];
  private totalGamesCount: number = 0;
  private displayedColumns: string[] = ['date'];
  
  constructor(public userService: UserService) {}
  
  ngOnInit() {
  }

  ngOnChanges(changes: SimpleChanges) {
    for(let propName in changes) {
      if (propName === 'targetUserId')
      {
        this.games = [];
        this.totalGamesCount = 0;
        this.isNoGamesToShow = true;
        this.isGamesLoading = false;
        if(this.targetUserId)
        {
          this.initializeGameData()
          .then(()=> {
            if(changes[propName].firstChange) {
              this.isInitialized = true;
              // AVOID THIS!!!1111111111111111111111111111111111111111111111111111111111
              setTimeout(() => {
                this.handleTableEvents();
              }, 200);
            }
          });
        }
      }
    }
  }

  async initializeGameData(): Promise<void> {
		this.isGamesLoading = true;
    return await this.userService.getUserGames(this.targetUserId, new Page(0, 5))
    .toPromise()
    .then((gamesPage: PagedResult<Game>) => {
      const newGamesData = gamesPage.dataRows.map((game) => new GameWithConclution(game, this.targetUserId));
      this.games = newGamesData;
      this.totalGamesCount = gamesPage.totalDataRowsCount;
      this.isGamesLoading = false;
      this.isNoGamesToShow = !(gamesPage && gamesPage.dataRows.length > 0);
  })
  }

  handleTableEvents() {
    merge(this.paginator.page)
      .pipe(
        switchMap(() => {
          debugger;
          this.isGamesLoading = true;
          return this.userService.getUserGames(this.targetUserId, new Page(this.paginator.pageIndex, this.paginator.pageSize));
        }),
        map((gamesPage: PagedResult<Game>) => {
          const newGamesData = gamesPage.dataRows.map((game) => new GameWithConclution(game, this.targetUserId));
          this.totalGamesCount = gamesPage.totalDataRowsCount;
          //this.dataSource.data = this.games;
          this.isGamesLoading = false;
          this.isNoGamesToShow = !(gamesPage && gamesPage.dataRows.length > 0);
          return newGamesData;
        }),
        catchError(() => {
          this.isGamesLoading = false;
          return of([]);
        })
      )
      .subscribe((newGamesData: GameWithConclution[]) => {
        if(newGamesData && newGamesData.length > 0) {
          // сохранять скачанные данные
          this.games = newGamesData;
          //this.games = [...this.games, ...newGamesData]
        }
      });
  }

}
