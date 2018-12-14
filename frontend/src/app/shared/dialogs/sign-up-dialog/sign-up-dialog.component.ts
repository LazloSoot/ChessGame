import { Component, OnInit, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-sign-up-dialog',
  templateUrl: './sign-up-dialog.component.html',
  styleUrls: ['./sign-up-dialog.component.less']
})
export class SignUpDialogComponent implements OnInit {
  @Output() successSignUp = new EventEmitter<any>();
  private repeatPass: string;
  private user: any; 
  private hide = true;
  
  constructor() { }

  ngOnInit() {
    this.user = {
      nickname: '',
      login: '',
      password: '',
      repeatPass: ''
    };
  }

  onSignUpFormSubmit(user, form) {

  }

  onGoogleClick() {

  }

  onFacebookClick() {

  }
}
