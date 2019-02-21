import { Component, OnInit, Input, Output, EventEmitter, ViewChild } from '@angular/core';
import { User, PagedResult, UserService, AppStateService, Page } from '../../../core';
import { MatPaginator } from '@angular/material';

@Component({
  selector: 'app-users-table',
  templateUrl: './users-table.component.html',
  styleUrls: ['./users-table.component.less']
})
export class UsersTableComponent implements OnInit {
  @ViewChild(MatPaginator) paginator:MatPaginator;
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
  private displayedColumns: string[] = ['name'];
  private pageSizeOptions = [5, 10, 20, 50];
  private currentUid: string;
  constructor(
    public userService: UserService,
    public appStateService: AppStateService
    ) { }

  ngOnInit() {
    this.getUsers(null);
  }

  resetSearchInput() {
		this.isSearchMode = false;
		this.users = [];
  }
  
  
	filterChange(event) {
    this.filterInput = event.target.value;
    this.getUsers(this.filterInput);
  }
  
  getUsers(filter: string) {
    this.isUsersLoading = true;
    this.isSearchMode = true;
    this.users = [];
    if (!this.timeOutSearch) {
      this.timeOutSearch = true;
      setTimeout(() => {
        this.timeOutSearch = false;
        this.userService
            .getUsersByNameStartsWith(filter, this.isOnlineUserFilterEnabled, new Page(0, 10))
            .subscribe((users: PagedResult<User>) => {
              debugger;
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
