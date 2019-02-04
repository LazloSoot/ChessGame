import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material';

@Component({
  selector: 'app-checkmate-dialog',
  templateUrl: './checkmate-dialog.component.html',
  styleUrls: ['./checkmate-dialog.component.less']
})
export class CheckmateDialogComponent implements OnInit {
  constructor(
    @Inject(MAT_DIALOG_DATA) public checkMateData
  ) { }

  ngOnInit() {
  }

}
