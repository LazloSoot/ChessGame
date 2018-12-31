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

@NgModule({
  imports: [
    CommonModule,
    MaterialModule,
    FormsModule
  ],
  
  exports: [
    MaterialModule,
    FormsModule
  ],

  declarations: [
    SignInDialogComponent,
    SignUpDialogComponent,
    ForgotPasswordDialogComponent,
    ConfirmEqualityValidatorDirective,
    NewGameDialogComponent,
    InvitationDialogComponent,
    WaitingDialogComponent,
    ],
    
  entryComponents: [
    SignInDialogComponent,
    SignUpDialogComponent,
    ForgotPasswordDialogComponent,
    NewGameDialogComponent,
    InvitationDialogComponent,
    WaitingDialogComponent
  ]
})
export class SharedModule { }
