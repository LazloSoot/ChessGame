import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInDialogComponent } from './';
import { MaterialModule } from './material/material.module';
import { SignUpDialogComponent } from './dialogs/sign-up-dialog/sign-up-dialog.component';
import { FormsModule } from '@angular/forms';
import { ConfirmEqualityValidatorDirective } from './directives/confirm-equality-validator.directive';
import { ForgotPasswordDialogComponent } from './dialogs/forgot-password-dialog/forgot-password-dialog.component';
import { GameSettingsDialogComponent } from './dialogs/game-settings/game-settings-dialog.component';

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
    GameSettingsDialogComponent,
    ],
    
  entryComponents: [
    SignInDialogComponent,
    SignUpDialogComponent,
    ForgotPasswordDialogComponent,
    GameSettingsDialogComponent
  ]
})
export class SharedModule { }
