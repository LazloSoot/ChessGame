import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { User, PagedResult, UserService, AppStateService, Page } from '../../../core';
import { MatPaginator } from '@angular/material';
import { Observable, fromEvent, interval } from 'rxjs';
import { scan, buffer, debounce, map, debounceTime } from 'rxjs/operators';

@Component({
  selector: 'app-users-table',
  templateUrl: './users-table.component.html',
  styleUrls: ['./users-table.component.less']
})
export class UsersTableComponent implements OnInit {
  @ViewChild('searchInput') searchInput: ElementRef;
  @Output() onUserSelected: EventEmitter<User> = new EventEmitter<User>(null); 
  private filterInput: string;
  private isOnlineFilter: boolean;
  private users: User[] = [];
  private loadedPages: PagedResult<User>[] = [];
  private prevPageSize: number;
  private timeOutSearch: boolean = false;
  private isOnlineUserFilterEnabled: boolean = false;
	private isSearchMode: boolean = false;
  private isUsersLoading: boolean = true;
  private totalUsersCount: number = 0;
  private displayedColumns: string[] = ['online', 'name'];
  private pageSizeOptions = [5, 10, 20, 50];
  private currentUid: string;
  private searchStream: Observable<any>;
  constructor(
    public userService: UserService,
    public appStateService: AppStateService
    ) { }

  ngOnInit() {
    this.getUsers(null);
  }

  ngAfterViewInit() {

    this.searchStream = fromEvent(this.searchInput.nativeElement, 'input').pipe(
      map(i => i.currentTarget.value));
    
    this.searchStream.pipe(
      scan((acc, crr) => acc = crr),
      debounceTime(500))
      .forEach((searchInputText) => {
        this.getUsers(searchInputText);
      })
  }

  resetSearchInput() {
		this.isSearchMode = false;
		this.getUsers('');
  }
  
  
  getUsers(filter: string) {
    this.isUsersLoading = true;
    this.isSearchMode = true;
    if (!this.timeOutSearch) {
      this.timeOutSearch = true;
      setTimeout(() => {
        this.timeOutSearch = false;
        this.userService
            .getUsersByNameStartsWith(filter, this.isOnlineUserFilterEnabled, new Page(0, 10))
            .subscribe((users: PagedResult<User>) => {
              this.users = [];
              if(users) {
                const currentId = this.appStateService.getCurrentUser().id;
                this.users = users.dataRows.filter(u => u.id !== currentId);
                this.timeOutSearch = false;
                this.isUsersLoading = false;
              }
            });
      }, 1000);
    }
  }
}
