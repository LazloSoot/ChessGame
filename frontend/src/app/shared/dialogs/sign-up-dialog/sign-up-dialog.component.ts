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
  @Output() onVerificationEmailSent = new EventEmitter<string>();
  public firebaseError: string;
  public repeatPass;
  public user: any; 
  public hide = true;
  
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
      .then(() => {
          this.authService.sendEmailVerification()
          .then(
            (email) => {
              this.onVerificationEmailSent.emit(email);
              this.dialogRef.close();
          }, 
          error => {
            this.firebaseError = error;
          })
				},
				error => {
					return error;
				}
			)
			.catch(error => {
				return error;
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
