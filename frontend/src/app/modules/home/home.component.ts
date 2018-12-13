import { Component, OnInit } from '@angular/core';
import { EventService } from '../../shared';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.less']
})
export class HomeComponent implements OnInit {

  constructor(private eventService: EventService) { }

  ngOnInit() {
    document.body.classList.add('bg-image');
  }

  onSignInClick() {
    this.eventService.filter('login');
  }

}
