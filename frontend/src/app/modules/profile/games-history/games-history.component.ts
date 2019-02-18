import { Component, OnInit, Input, SimpleChanges, ViewChild, Output, EventEmitter, AfterViewInit } from "@angular/core";
import { GameWithConclution, PagedResult, Game, UserService, Page } from "../../../core";
import { MatPaginator, MatTableDataSource } from "@angular/material";
import { merge, of, BehaviorSubject, from } from "rxjs";
import { startWith, switchMap, map, catchError, concatMap, flatMap } from "rxjs/operators";

@Component({
	selector: "app-games-history",
	templateUrl: "./games-history.component.html",
	styleUrls: ["./games-history.component.less"]
})
export class GamesHistoryComponent implements OnInit {
  @Input() targetUserId: number;
  @ViewChild(MatPaginator) paginator:MatPaginator;
  private currentPageGames: GameWithConclution[] = [];
  private loadedPages: PagedResult<GameWithConclution>[] = [];
  private prevPageSize: number;

  private isGamesLoading: boolean = true;
  private isInitialized: boolean = false;
  private isNoGamesToShow: boolean = true;
  private totalGamesCount: number = 0;
  private displayedColumns: string[] = ['date'];
  private pageSizeOptions = [5, 10, 20, 50];
  
  constructor(public userService: UserService) {}
  
  ngOnInit() {
    this.prevPageSize = this.pageSizeOptions[0];
  }

  ngOnChanges(changes: SimpleChanges) {
    for(let propName in changes) {
      if (propName === 'targetUserId')
      {
        this.currentPageGames = [];
        this.loadedPages = [];
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
      this.loadedPages = [new PagedResult<GameWithConclution>(gamesPage.pageIndex, gamesPage.pageCount, gamesPage.pageSize, gamesPage.totalDataRowsCount, newGamesData)];
      this.currentPageGames = newGamesData;
      this.totalGamesCount = gamesPage.totalDataRowsCount;
      this.isGamesLoading = false;
      this.isNoGamesToShow = !(gamesPage && gamesPage.dataRows.length > 0);
  })
  }

  handleTableEvents() {
    merge(this.paginator.page)
      .pipe(
        switchMap(() => {
          this.isGamesLoading = true;
          if(this.paginator.pageSize === this.prevPageSize || this.paginator.pageSize > this.prevPageSize) {
            return this.userService.getUserGames(this.targetUserId, new Page(this.paginator.pageIndex, this.paginator.pageSize))
            .pipe(map((gamesPage: PagedResult<Game>) => {
              const newGamesData = gamesPage.dataRows.map((game) => new GameWithConclution(game, this.targetUserId));
              this.totalGamesCount = gamesPage.totalDataRowsCount;
              //this.dataSource.data = this.games;
              this.isGamesLoading = false;
              this.isNoGamesToShow = !(gamesPage && gamesPage.dataRows.length > 0);
              return [new PagedResult<GameWithConclution>(gamesPage.pageIndex, gamesPage.pageCount, gamesPage.pageSize, gamesPage.totalDataRowsCount, newGamesData)];
            }),
            catchError(() => {
              this.isGamesLoading = false;
              return of([]);
            }));
          } else {
            const factor = Math.floor(this.prevPageSize / this.paginator.pageSize);
            const pagesCount = Math.ceil(this.totalGamesCount / this.paginator.pageSize);

            return from(this.loadedPages)
            .pipe(map((page, index) => {
              let pages: PagedResult<GameWithConclution>[] = [];
              let currentGamesPart: GameWithConclution[];
              for (let i = 0, sliceStartIndex = 0, sliceEndIndex = 0; i <  factor && sliceEndIndex < page.dataRows.length; i++) {
                sliceEndIndex = (page.dataRows.length >= sliceEndIndex + this.paginator.pageSize) 
                ? sliceEndIndex + this.paginator.pageSize 
                : page.dataRows.length;  
                currentGamesPart = page.dataRows.slice(sliceStartIndex, sliceEndIndex);
                sliceStartIndex += this.paginator.pageSize;
                pages.push(new PagedResult<GameWithConclution>(index + i, pagesCount, this.paginator.pageSize, page.totalDataRowsCount, currentGamesPart));
              }
              this.isGamesLoading = false;  
              return pages;
            }),
            catchError(() => {
              this.isGamesLoading = false;
              return of([]);
            }));
          }
        })
      )
      .subscribe((newPages: PagedResult<GameWithConclution>[]) => {
        if(newPages)
        {
          this.currentPageGames = newPages.find(p => p.pageIndex === this.paginator.pageIndex).dataRows;
          // pageSize was changed => old pages should be removed
          if(this.paginator.pageSize !== this.prevPageSize) {
            this.loadedPages = [];
          }
          this.loadedPages = [...this.loadedPages, ...newPages];
        }
        
        this.prevPageSize = this.paginator.pageSize;
      });
      //.subscribe((newGamesData: GameWithConclution[]) => {
      //  if(newGamesData && newGamesData.length > 0) {
      //    // сохранять скачанные данные
      //    this.games = newGamesData;
//
      //    //this.games = [...this.games, ...newGamesData]
      //  }
      //});
  }
}
