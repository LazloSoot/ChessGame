import { Component, OnInit, EventEmitter, Output } from '@angular/core';
import { AuthService } from '../../../core';
import { MatDialogRef } from '@angular/material';
import { AuthProviderType } from '../../../core';

@Component({
  selector: 'app-sign-up-dialog',
  templateUrl: './sign-up-dialog.component.html',
  styleUrls: ['./sign-up-dialog.component.less']
})
export class SignUpDialogComponent implements OnInit {
  @Output() onSuccessSignUp = new EventEmitter<any>();
  public firebaseError: string;
  private user: any; 
  private hide = true;
  
  constructor(
    private dialogRef: MatDialogRef<SignUpDialogComponent>,
    private authService: AuthService
    ) { }

  ngOnInit() {
    this.user = {
      nickname: '',
      login: '',
      password: '',
      repeatPass: ''
    };
  }

  signUpFormSubmit(user, form) {
    if(form.valid){
      this.authService.signUpRegular(user.login, user.nickname, user.password)
      .then(info => {
        this.firebaseError = info;
      });
    }
  }

  signUpWithGoogle() {
    this.authService.signIn(AuthProviderType.Google).then(error => {
			if (error) {
				this.firebaseError = (error.message) ? error.message : error;
			} else {
				this.onSuccessSignUp.emit(true);
				this.dialogRef.close();
			}
		});
  }

  signUpWithFacebook() {
    this.authService.signIn(AuthProviderType.Google).then(error => {
			if (error) {
				this.firebaseError = (error.message) ? error.message : error;
			} else {
				this.onSuccessSignUp.emit(true);
				this.dialogRef.close();
			}
		});
  }
}
