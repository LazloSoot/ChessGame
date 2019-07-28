import { Component, OnInit, Input, Output, EventEmitter, ViewChild, ElementRef } from '@angular/core';
import { User, PagedResult, UserService, AppStateService, Page } from '../../../core';
import { MatPaginator } from '@angular/material';
import { Observable, fromEvent, interval } from 'rxjs';
import { scan, buffer, debounce, map, debounceTime } from 'rxjs/operators';
import { PresenceService } from '../../../core/services/presence.service';

@Component({
  selector: 'app-users-table',
  templateUrl: './users-table.component.html',
  styleUrls: ['./users-table.component.less']
})
export class UsersTableComponent implements OnInit {
  @ViewChild('searchInput') searchInput: ElementRef;
  @Output() onUserSelected: EventEmitter<User> = new EventEmitter<User>(null); 
  public namePart: string ="";
  public timeOutSearch: boolean = false;
  public isUsersLoading: boolean = true;
  public users: User[] = [];
  public displayedColumns: string[] = ['online', 'name', 'invite'];
  private filterInput: string;
  private isOnlineFilter: boolean;
  private loadedPages: PagedResult<User>[] = [];
  private prevPageSize: number;
  private isOnlineUserFilterEnabled: boolean = false;
	private isSearchMode: boolean = false;
  private totalUsersCount: number = 0;
  private pageSizeOptions = [5, 10, 20, 50];
  private currentUid: string;
  private searchStream: Observable<any>;
  constructor(
    public userService: UserService,
    public appStateService: AppStateService,
    public presenceService: PresenceService
    ) { }

  ngOnInit() {
    this.getUsers(null);
  }

  ngAfterViewInit() {

    this.searchStream = fromEvent(this.searchInput.nativeElement, 'input');
    
    this.searchStream.pipe(
      debounceTime(500))
      .subscribe(() => {
        this.getUsers(this.namePart);
      });
  }

  resetSearchInput() {
    this.users = [];
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
        if(filter) {
          this.userService
          .getUsersByNameStartsWith(filter, this.isOnlineUserFilterEnabled, new Page(0, 100))
          .subscribe(async (users: PagedResult<User>) => {
            this.users = [];
            if(users) {
              const currentId = this.appStateService.getCurrentUser().id;
              this.users = users.dataRows.filter(u => u.id !== currentId);
              await this.users.forEach(async user => {
                const u = await this.presenceService.getUserSnapshot(user.uid);
                if(u)
                  user.isOnline = u.isOnline;
              });
              this.timeOutSearch = false;
              this.isUsersLoading = false;
              console.log(`elapsed milliseconds :  ${users.elapsedMilliseconds}`);
            }
          });
        } else {
          this.presenceService
          .getFirstUsersSnapshots(10)
            .then((users: PagedResult<User>) => {
              this.users = [];
              if(users) {
                const currentUid = this.appStateService.getCurrentUser().uid;
                this.users = users.dataRows.filter(u => u.uid !== currentUid);
                this.timeOutSearch = false;
                this.isUsersLoading = false;
              }
            });
        }
        
      }, 1200);
    }
  }

  trySelectUser(user: User) {
    if(user.isOnline) {
      this.onUserSelected.emit(user);
    }
  }

  getColor(isUserOnline: boolean): string {
    return isUserOnline ? '#4EBE7D' : '#BF081B';
  }
}
