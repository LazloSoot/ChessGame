import { Component, OnInit, EventEmitter, Output } from '@angular/core';

@Component({
  selector: 'app-sign-up-dialog',
  templateUrl: './sign-up-dialog.component.html',
  styleUrls: ['./sign-up-dialog.component.less']
})
export class SignUpDialogComponent implements OnInit {
  @Output() successSignUp = new EventEmitter<any>();
  constructor() { }

  ngOnInit() {
  }

}
