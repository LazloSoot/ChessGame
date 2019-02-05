import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInDialogComponent } from './';
import { MaterialModule } from './material/material.module';
import { SignUpDialogComponent } from './dialogs/sign-up-dialog/sign-up-dialog.component';
import { FormsModule } from '@angular/forms';
import { ConfirmEqualityValidatorDirective } from './directives/confirm-equality-validator.directive';
import { ForgotPasswordDialogComponent } from './dialogs/forgot-password-dialog/forgot-password-dialog.component';
import { NewGameDialogComponent } from './dialogs/new-game/new-game-dialog.component';
import { InvitationDialogComponent } from './dialogs/invitation-dialog/invitation-dialog.component';
import { WaitingDialogComponent } from './dialogs/waiting-dialog/waiting-dialog.component';
import { SpinnerRectComponent } from './layout/spinners/spinner-rect/spinner-rect.component';
import { SpinnerCircleComponent } from './layout/spinners/spinner-circle/spinner-circle.component';
import { CheckmateDialogComponent } from './dialogs/checkmate-dialog/checkmate-dialog.component';
import { SpinnerColorDirective } from './layout/spinners/directives/spinner-color.directive';

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule
  ],
  
  exports: [
    MaterialModule,
    FormsModule,
    SpinnerRectComponent,
    SpinnerColorDirective
  ],

  declarations: [
    SignInDialogComponent,
    SignUpDialogComponent,
    ForgotPasswordDialogComponent,
    ConfirmEqualityValidatorDirective,
    NewGameDialogComponent,
    InvitationDialogComponent,
    WaitingDialogComponent,
    SpinnerRectComponent,
    SpinnerCircleComponent,
    CheckmateDialogComponent,
    SpinnerColorDirective
    ],
    
  entryComponents: [
    SignInDialogComponent,
    SignUpDialogComponent,
    ForgotPasswordDialogComponent,
    NewGameDialogComponent,
    InvitationDialogComponent,
    WaitingDialogComponent,
    CheckmateDialogComponent
  ]
})
export class SharedModule { }
