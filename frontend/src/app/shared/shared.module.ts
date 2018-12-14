import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { SignInDialogComponent } from './';
import { MaterialModule } from './material/material.module';
import { SignUpDialogComponent } from './dialogs/sign-up-dialog/sign-up-dialog.component';
import { FormsModule } from '@angular/forms';

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
    SignUpDialogComponent
    ],
    
  entryComponents: [
    SignInDialogComponent,
    SignUpDialogComponent
  ]
})
export class SharedModule { }
