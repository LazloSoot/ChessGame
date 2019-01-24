import { Component, OnInit, Inject } from '@angular/core';
import { User } from '../../../core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-waiting-dialog',
  templateUrl: './waiting-dialog.component.html',
  styleUrls: ['./waiting-dialog.component.less']
})
export class WaitingDialogComponent implements OnInit {
  constructor(
    private dialogRef: MatDialogRef<WaitingDialogComponent>,
		@Inject(MAT_DIALOG_DATA) public user: User
  ) { }

  ngOnInit() {
  }

  getOpponentAvatarUrl() {
    return (this.user.avatarUrl) ? this.user.avatarUrl : '../../../../assets/images/anonAvatar.png' ;
  }
}
