import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-info-dialog',
  templateUrl: './info-dialog.component.html',
  styleUrls: ['./info-dialog.component.less']
})
export class InfoDialogComponent implements OnInit {

  constructor(
    @Inject(MAT_DIALOG_DATA) public info: string
  ) { }

  ngOnInit() {
  }

}
