import { Component, OnInit } from '@angular/core';
import { HomeEventService } from './home-event.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  constructor(private eventService: HomeEventService) { }

  ngOnInit() {
    document.body.classList.add('bg-image');
  }

  onSignUpClick() {
    this.eventService.filter('signUp');
  }

  onLoginClick() {
    this.eventService.filter('login');
  }

}
