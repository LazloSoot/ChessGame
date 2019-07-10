import { Component, OnInit, Input, SimpleChanges, ViewChild, Output, EventEmitter, AfterViewInit } from "@angular/core";
import { GameWithConclution, PagedResult, Game, UserService, Page, PageReformationType, PageReformationOptions, GameStatus, GameSide } from "../../../core";
import { MatPaginator, MatTableDataSource } from "@angular/material";
import { merge, of, BehaviorSubject, from, Observable, pipe } from "rxjs";
import { startWith, switchMap, map, catchError, concatMap, flatMap } from "rxjs/operators";

@Component({
	selector: "app-games-history",
	templateUrl: "./games-history.component.html",
	styleUrls: ["./games-history.component.less"]
})
export class GamesHistoryComponent implements OnInit {
  @Input() targetUserId: number;
  @ViewChild(MatPaginator) paginator:MatPaginator;
  public isGamesLoading: boolean = true;
  public isInitialized: boolean = false;
  private currentPageGames: GameWithConclution[] = [];
  private loadedPages: PagedResult<GameWithConclution>[] = [];
  private prevPageSize: number;
  private isNoGamesToShow: boolean = true;
  private totalGamesCount: number = 0;
  private displayedColumns: string[] = ['date', 'side', 'status', 'result', 'opponent'];
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
      debugger;
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
          let pagesReformationType = PageReformationType.None;
          if(this.paginator.pageSize !== this.prevPageSize) {
            if(this.paginator.pageSize > this.prevPageSize) {

            } else {
              pagesReformationType = PageReformationType.Split;
            }
          }

          this.reformPages(pagesReformationType);
          return this.getCurrentPage();
        })
      )
      .subscribe((page: PagedResult<GameWithConclution>) => {
        this.isGamesLoading = false;
        if(!this.loadedPages.find(p => p.pageIndex === page.pageIndex))
        {
          this.loadedPages.push(page);
        }
        this.currentPageGames = page.dataRows;
        this.prevPageSize = this.paginator.pageSize;
      });
  }

  getCurrentPage(): Observable<PagedResult<GameWithConclution>> {
    const loadedPage = this.loadedPages.find(p => p.pageIndex === this.paginator.pageIndex && p.pageSize === this.paginator.pageSize);
    if (loadedPage) {
      return of(loadedPage);
    } else {
      return this.loadPage();
    }
  }

  loadPage(): Observable<PagedResult<GameWithConclution>> {
    return this.userService.getUserGames(this.targetUserId, new Page(this.paginator.pageIndex, this.paginator.pageSize))
      .pipe(map((gamesPage: PagedResult<Game>) => {
        const newGamesData = gamesPage.dataRows.map((game) => new GameWithConclution(game, this.targetUserId));
        this.totalGamesCount = gamesPage.totalDataRowsCount;
        this.isNoGamesToShow = !(gamesPage && gamesPage.dataRows.length > 0);
        return new PagedResult<GameWithConclution>(gamesPage.pageIndex, gamesPage.pageCount, gamesPage.pageSize, gamesPage.totalDataRowsCount, newGamesData);
      }),
        catchError(() => {
          return of(new PagedResult<GameWithConclution>());
        }));
  }

  reformPages(reformatioType: PageReformationType): void
  {
    let reformedPages = this.loadedPages;
    switch (reformatioType) {
      case PageReformationType.Merge:
        {
          
          break;
        }
        case PageReformationType.Split: 
        {
          const splitOptions = new PageReformationOptions(
            Math.floor(this.prevPageSize / this.paginator.pageSize),
            Math.ceil(this.totalGamesCount / this.paginator.pageSize),
            this.paginator.pageSize
          )
          const splittedPages2d =  this.loadedPages.map((page, index, arr) => {
            return this.splitPage(page, index, splitOptions);
          });
          reformedPages = [].concat(...splittedPages2d).map((page: PagedResult<GameWithConclution>, index, arr) => {
            page.pageIndex = index;
            return page;
          });
          break;
        }
      default:
        {
          reformedPages = reformedPages.filter(p => p.pageSize === this.paginator.pageSize);
          break;
        }
    }
    this.loadedPages = reformedPages;
  }

  private splitPage(page: PagedResult<GameWithConclution>, index: number, options: PageReformationOptions): PagedResult<GameWithConclution>[] {
    let pages: PagedResult<GameWithConclution>[] = [];
    let currentGamesPart: GameWithConclution[];
    for (let i = 0, sliceStartIndex = 0, sliceEndIndex = 0; i <  options.reformFactor && sliceEndIndex < page.dataRows.length; i++) {
      sliceEndIndex = (page.dataRows.length >= sliceEndIndex + options.pageSize) 
      ? sliceEndIndex + options.pageSize 
      : page.dataRows.length;  
      currentGamesPart = page.dataRows.slice(sliceStartIndex, sliceEndIndex);
      sliceStartIndex += options.pageSize ;
      pages.push(new PagedResult<GameWithConclution>(0, options.totalPagesCount, options.pageSize, page.totalDataRowsCount, currentGamesPart));
    }
    return pages;
  }

  private mergePages(pages: PagedResult<GameWithConclution>[], index: number, options: PageReformationOptions): PagedResult<GameWithConclution> {
    return null;
  }

  getGameSide(game: GameWithConclution) {
    return GameSide[game.side];
  }

  getGameStatus(game: GameWithConclution) {
    switch (game.status) {
      case GameStatus.Going:
        return "play_arrow";
      case GameStatus.Completed:
        return "done";
      case GameStatus.Waiting:
        return "alarm";
      case GameStatus.Suspended: 
        return "pause";
      default:
        return "not_interested";
    }
  }

  getGameResultValue(game: GameWithConclution) {
    if(game.status === GameStatus.Completed) {
      if(game.isWin) {
        return "Win"
      } else if(game.isDraw) {
        return "Draw";
      } else {
        return "Lose"
      }
    } else {
      return "-";
    }
  }
}
