import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-confirmation-dialog',
  templateUrl: './confirmation-dialog.component.html',
  styleUrls: ['./confirmation-dialog.component.less']
})
export class ConfirmationDialogComponent implements OnInit {
  
  constructor(@Inject(MAT_DIALOG_DATA) public message: string) { }

  ngOnInit() {
  }

}
