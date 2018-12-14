import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { MatDialogRef } from '@angular/material';
import { EventService } from '../../helpers';

@Component({
  selector: 'app-sign-in-dialog',
  templateUrl: './sign-in-dialog.component.html',
  styleUrls: ['./sign-in-dialog.component.less']
})
export class SignInDialogComponent implements OnInit {
  @Output() onSucceessLogin = new EventEmitter<any>();
  @Output() onRegister = new EventEmitter<any>();
  public firebaseError: string;
  public hide = true;
  public user: any;

  constructor(
    private dialogRef: MatDialogRef<SignInDialogComponent>,
    private e: EventService
  ) { }

  ngOnInit() {
    this.user = {
      login: '',
      password: ''
    };
  }
  
  onForgotPasswordClick() {
    this.dialogRef.close();
  }

  onLoginFormSubmit(user, form) {
    debugger;
  }

  onSignUp() {
    this.dialogRef.close();
    setTimeout(() => {
      this.e.filter("signUp");
    }, 50);
  }
}
